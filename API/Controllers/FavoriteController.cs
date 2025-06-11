using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {

        private readonly IFavoriteRepo _favoriteRepo;
        private readonly UserManager<User> _userManager;



        public FavoriteController(IFavoriteRepo favoriteRepo, UserManager<User> userManager)
        {
            _favoriteRepo = favoriteRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateArtifact()
        {



            try
            {
                return Ok(await _favoriteRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCateArtifactById(int id)
        {

            try
            {
                return Ok(await _favoriteRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getByUserId")]
        public async Task<IActionResult> GetByUserId()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            try
            {
                return Ok(await _favoriteRepo.GetByUserId(user.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCateArtifact(int artifactId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);



            var favorite = new Favorite()
            {
                isSaveButton= true,
                ArtifactId= artifactId,

                UserId = user.Id,
                
            };


            await _favoriteRepo.Add(favorite);


            return Ok(favorite);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateCateArtifact(int id,int artifactId)
        {
            
            var favorite = await _favoriteRepo.GetById(id);
            if (favorite == null)
            {
                return NotFound();
            }
            favorite.isSaveButton = false;



            await _favoriteRepo.Update(favorite);

            return Ok(favorite);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteCateArtifact(int id)
        {


            var favorite = await _favoriteRepo.GetById(id);
            if (favorite == null)
            {

                return NotFound();
            }

            await _favoriteRepo.Delete(id);
            return Ok();


        }
    }
}
