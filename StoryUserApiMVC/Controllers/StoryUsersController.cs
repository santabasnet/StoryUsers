using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryUserApi.Data;
using StoryUserApi.Models;
using StoryUserApi.Services;
using StoryUserApi.Utils;

namespace StoryUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IPasswordHasher<StoryUser> _passwordHasher;

        private readonly JwtService _jwtService;


        public StoryUsersController(AppDbContext context, IPasswordHasher<StoryUser> passwordHasher, JwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        // GET: api/StoryUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryUser>>> GetStoryUsers()
        {
            var users = await _context.StoryUsers
                .Select(user => new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Address = user.Address
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/StoryUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetStoryUser(int id)
        {
            var user = await _context.StoryUsers.FindAsync(id);
            if (user == null) return NotFound();
            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Address = user.Address
            };

            return userResponse;
        }

        // POST: api/StoryUsers
        //[Authorize]
        [HttpPost("register")]
        public async Task<ActionResult<StoryUser>> RegisterUser(StoryUser user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _context.StoryUsers
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null) return Conflict(ResponseUtils.emailAlreadyExists());

            // Hash the password
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            _context.StoryUsers.Add(user);
            await _context.SaveChangesAsync();

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Address = user.Address
            };

            return CreatedAtAction(nameof(GetStoryUser), new { id = user.Id }, userResponse);
        }


        // PUT: api/StoryUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStoryUser(int id, StoryUser user)
        {
            if (id != user.Id) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/StoryUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoryUser(int id)
        {
            var user = await _context.StoryUsers.FindAsync(id);
            if (user == null) return NotFound();
            _context.StoryUsers.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.StoryUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return Unauthorized(ResponseUtils.invalidEmailOrPassword());

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result != PasswordVerificationResult.Success) return Unauthorized(ResponseUtils.invalidEmailOrPassword());

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return Ok(new
            {
                accessToken,
                refreshToken
            });
        }

    }
}
