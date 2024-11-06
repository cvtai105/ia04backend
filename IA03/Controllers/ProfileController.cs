using IA03.Data;
using IA03.DTO;
using IA03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IA03.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        public ProfileController(AppDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<ActionResult<User>> GetProfile()
        {
            // Get the user ID from the authorization token
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
            {
                return BadRequest("user Id not found in token");
            }
            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            
            if (user == null)
            {
                return NotFound(); 
            }
            
            return Ok(new { user.Id, user.Email, user.Name }); 
        }
        
    }
}
