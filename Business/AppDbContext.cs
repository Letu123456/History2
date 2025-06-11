using Business.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Business
{
    public class AppDbContext: IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<EmailVerification> emailVerification { get; set; }
        public DbSet<PasswordVerification> passwordVerification { get; set; }
        public DbSet<Artifact> artifact { get; set; }
        public DbSet<Historical> historicals { get; set; }
        public DbSet<CategoryArtifact> categoryArtifacts { get; set; }
        public DbSet<CategoryHistorical> categoryHistoricals { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<RepliComment> replicomments { get; set; }
        public DbSet<Favorite> favorites { get; set; }
        public DbSet<Event> events { get; set; }
        public DbSet<Museum> museums { get; set; }
        public DbSet<Blog> blogs { get; set; }
        public DbSet<Appli> applis { get; set; }
        public DbSet<Report> reports { get; set; }
        public DbSet<Figure> figures { get; set; }
        public DbSet<CategoryFigure> categoryFigures { get; set; }
        public DbSet<Answer> answers { get; set; }
        public DbSet<Question> question { get; set; }
        public DbSet<Quiz> quiz { get; set; }
        public DbSet<UserQuizResult> userQuizResults { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<CategoryProduct> categoryProducts { get; set; }
        public DbSet<CategoryBlog> categoryBlogs { get; set; }
        public DbSet<RedemptionHistory> redemptionHistories { get; set; }
        public DbSet<TransactionHistory> transactionHistories { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Rate> rates { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<Video> videos { get; set; }
        public DbSet<CommentVideo> commentVideos { get; set; }
        public DbSet<CategoryVideo> categoryVideos { get; set; }
        public DbSet<PaymentTransaction> paymentTransactions { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<PaymentTransactionProduct> paymentTransactionProducts { get; set; }
        public DbSet<ShippingInfor> shippingInfors { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Artifact>().
                HasMany(x => x.SaveButtons).
                WithOne(y => y.Artifact).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
        .HasOne(m => m.SenderNoti)  // Một Message có một Sender
        .WithMany()              // Một User có nhiều tin nhắn gửi đi
        .HasForeignKey(m => m.SenderNotiId)
        .OnDelete(DeleteBehavior.Restrict); // Không xóa tin nhắn khi xóa người gửi

            modelBuilder.Entity<Notification>()
                .HasOne(m => m.ReceiverNoti) // Một Message có một Receiver
                .WithMany()              // Một User có nhiều tin nhắn nhận được
                .HasForeignKey(m => m.ReceiverNotiId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Message>()
        .HasOne(m => m.Sender)  // Một Message có một Sender
        .WithMany()              // Một User có nhiều tin nhắn gửi đi
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict); // Không xóa tin nhắn khi xóa người gửi

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver) // Một Message có một Receiver
                .WithMany()              // Một User có nhiều tin nhắn nhận được
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Historical>().
                HasMany(x => x.ArtifactHistoricals).
                WithOne(y => y.Historical).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Artifact>().
                HasMany(x => x.ArtifactHistoricals).
                WithOne(y => y.Artifact).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Historical>().
                HasMany(x => x.HistoricalFigures).
                WithOne(y => y.Historical).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Figure>()
        .HasMany(f => f.HistoricalFigures)
        .WithOne(h => h.Figure)
        .HasForeignKey(h => h.FigureId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
    .HasOne(u => u.Museum)
    .WithOne(m => m.User)
    .HasForeignKey<User>(u => u.MuseumId);

            //    modelBuilder.Entity<Event>().
            //        HasMany(x => x.Comments).
            //        WithOne(y => y.Event).
            //        OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Event>().
                HasMany(x => x.RepliComments).
                WithOne(y => y.Event).
                OnDelete(DeleteBehavior.SetNull);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                if (tableName != null)
                {
                    entityType.SetTableName(Char.ToUpper(tableName[0]) + tableName.Substring(1));
                }
            }


            modelBuilder.Entity<User>(entity => entity.ToTable("Users"));
            modelBuilder.Entity<Role>(entity => entity.ToTable("Roles"));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
        }
    }
}
