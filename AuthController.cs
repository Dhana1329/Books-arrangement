using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) { _auth = auth; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                var user = await _auth.RegisterAsync(req.Username, req.Email, req.Password);
                return CreatedAtAction(null, new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var token = await _auth.LoginAsync(req.Username, req.Password);
            if (token == null) return Unauthorized();
            return Ok(new { token });
        }
    }

    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Username, string Password);
}
