using Amazon.Auth.AccessControlPolicy;
using Amazon.S3;
using API.Hubs;
using AutoMapper;
using Business;
using Business.Mapping;
using Business.Model;
using Business.Options;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(
    options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

PayOS payOS = new PayOS(configuration["Environment:ClientId"] ?? throw new Exception("Cannot find environment"),
                    configuration["Environment:ApiKey"] ?? throw new Exception("Cannot find environment"),
                    configuration["Environment:ChecksumKey"] ?? throw new Exception("Cannot find environment"));

builder.Services.AddSingleton<PayOS>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var clientId = configuration["Environment:ClientId"];
    var apiKey = configuration["Environment:ApiKey"];
    var checksumKey = configuration["Environment:ChecksumKey"];

    if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(checksumKey))
    {
        throw new InvalidOperationException("PayOS configuration is incomplete. Check PayOsSettings in appsettings.json.");
    }

    var logger = sp.GetRequiredService<ILogger<PayOS>>();
    logger.LogInformation("Initializing PayOS with ClientId={ClientId}, ApiKey={ApiKey}",
        clientId,
        apiKey.Length > 4 ? "****" + apiKey[^4..] : apiKey);

    return new PayOS(clientId, apiKey, checksumKey);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionstring));

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, Role>(options =>
{
    // ✅ Bắt buộc xác nhận email trước khi đăng nhập
    options.SignIn.RequireConfirmedEmail = true;

    

})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // 👈 Phải có để sinh token xác nhận email


var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);
var emailOptions = builder.Configuration.GetSection("SmtpSettings").Get<EmailOptions>();
builder.Services.AddSingleton(emailOptions);
builder.Services.AddAuthentication(options =>
{

    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Đặt JWT làm mặc định
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;


}
)
    .AddCookie()
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
    {
        option.SaveToken = true;
        option.TokenValidationParameters = new TokenValidationParameters
        {
            //ValidateIssuer = true,
            ValidateIssuer = false,
            ValidIssuer = jwtOptions?.Issuer,

            //ValidateAudience = true,			
            ValidateAudience = false,
            ValidAudience = jwtOptions?.Audience,



            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key)),
            ValidateIssuerSigningKey = true,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
        option.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                var user = await userManager.GetUserAsync(context.Principal);

                if (user != null)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    var claims = roles.Select(role => new Claim(ClaimTypes.Role, role));

                    var identity = (ClaimsIdentity)context.Principal.Identity;
                    identity.AddClaims(claims);
                }
            }
        };
    });
builder.Services.Configure<GhnSettings>(builder.Configuration.GetSection("GHN"));
builder.Services.AddHttpClient<GhnService>();

builder.Services.Configure<PayOsSettings>(
    builder.Configuration.GetSection("Environment"));
builder.Services.AddHttpClient(); // 💡 Bắt buộc phải có

builder.Services.AddScoped<GoogleService>();
builder.Services.AddScoped<GhnService>();
builder.Services.AddScoped<PayOsService>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<FilesService>();
builder.Services.AddScoped<GPTService>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IArtifactRepo, ArtifactRepo>();
builder.Services.AddScoped<IHistoricalRepo, HistoricalRepo>();
builder.Services.AddScoped<ICategoryArtifactRepo, CategoryArtifactRepo>();
builder.Services.AddScoped<ICategoryHistoricalRepo, CategoryHistoricalRepo>();
builder.Services.AddScoped<ICategoryFigureRepo, CategoryFigureRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<IRepliCommentRepo, RepliCommentRepo>();
builder.Services.AddScoped<IMuseumRepo, MuseumRepo>();
builder.Services.AddScoped<IEventRepo, EventRepo>();
builder.Services.AddScoped<IBlogRepo,BlogRepo>();
builder.Services.AddScoped<IFavoriteRepo,FavoriteRepo>();
builder.Services.AddScoped<IAppliRepo,AppliRepo>();
builder.Services.AddScoped<IReportRepo,ReportRepo>();
builder.Services.AddScoped<IFigureRepo,FigureRepo>();
builder.Services.AddScoped<IQuizRepo, QuizRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICategoryProductRepo, CategoryProductRepo>();
builder.Services.AddScoped<IAnswerRepo, AnswerRepo>();
builder.Services.AddScoped<IQuestionRepo, QuestionRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddScoped<ICategoryFigureRepo, CategoryFigureRepo>();
builder.Services.AddScoped<IRateRepo, RateRepo>();
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
builder.Services.AddScoped<IMessageRepo, MessageRepo>();
builder.Services.AddScoped<ICategoryBlogRepo, CategoryBlogRepo>();
builder.Services.AddScoped<IVideoRepo, VideoRepo>();
builder.Services.AddScoped<ICategoryVideoRepo, CategoryVideoRepo>();
builder.Services.AddScoped<ICommentVideo, CommentVideoRepo>();
builder.Services.AddScoped<IPaymentTransactionRepo, PaymentTransactionRepo>();
builder.Services.AddScoped<ICartRepo, CartRepo>();
builder.Services.AddScoped<ICartItemRepo, CartItemRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderItemRepo, OrderItemRepo>();
builder.Services.AddScoped<IPaymentTransactionProductRepo, PaymentTransactionProductRepo>();
builder.Services.AddScoped<IShippingInforRepo, ShippingInforRepo>();





builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins(
                "https://vnmu-three.vercel.app",
                "http://vnmu-three.vercel.app" // Add HTTP origin for development
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Optional: Include if SignalR or authenticated requests need credentials
    });
});

builder.Services.AddHttpContextAccessor();


var mapperConfig = new MapperConfiguration(mc => {
    mc.AddProfile(new HistoricalMapping());
    mc.AddProfile(new ArtifactMapping());
    mc.AddProfile(new MuseumMapping());
    mc.AddProfile(new EventMapping());
    mc.AddProfile(new BlogMapping());

});


builder.Services.AddSingleton(mapperConfig.CreateMapper());
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HistoryApi",
        Description = "Build API with ASP.Net Core (.Net 8) that secured with JWT.",
        Contact = new OpenApiContact
        {
            Name = "Anhtu",
            Email = "Anhtu200003@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/nada-mhmudd/")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });



    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddSingleton<Business.Options.WebSocketManager>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // Giới hạn 100MB
});
builder.Services.AddSignalR();
//builder.Services.Configure<KestrelServerOptions>(options => {
//    options.ConfigureHttpsDefaults(options =>
//        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate);
//});

var app = builder.Build();

// 1️⃣ Map Hub và WebSockets
app.MapHub<ChatHub>("api/chatHub");
app.UseWebSockets(); // Kích hoạt WebSocket
app.UseMiddleware<Business.Options.WebSocketMiddleware>();

// 2️⃣ Cấu hình StaticFiles, Swagger, Https

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// 3️⃣ Routing
app.UseRouting();

// 4️⃣ CORS (NÊN để sau UseRouting)
app.UseCors("AllowVercel");
app.UseStaticFiles();
// 5️⃣ Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6️⃣ Map Controllers
app.MapControllers();

app.Run();

