using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Username == username)) throw new Exception("Username already exists");
            if (await _db.Users.AnyAsync(u => u.Email == email)) throw new Exception("Email already exists");

            var user = new User
            {
                Username = username,
                Email = email,
                Role = "Member"
            };
            user.PasswordHash = _hasher.HashPassword(user, password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;
            var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (res == PasswordVerificationResult.Failed) return null;

            // Generate JWT
            var jwt = _config.GetSection("JwtSettings");
            var secret = jwt["Secret"] ?? throw new Exception("JWT Secret not configured");
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var expiryMinutes = int.Parse(jwt["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "Member")
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiryMinutes), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
