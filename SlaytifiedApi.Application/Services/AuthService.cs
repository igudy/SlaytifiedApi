using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SlaytifiedApi.Domain.Entities;
using SlaytifiedApi.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using SlaytifiedApi.Application.Interfaces;

namespace SlaytifiedApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(IAppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.UserName == request.UserName);

            if (existingUser != null)
                throw new InvalidOperationException("User already exists with this email or username.");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GenerateTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserName.ToLower() == request.UserNameOrEmail.ToLower() ||
                    u.Email.ToLower() == request.UserNameOrEmail.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return await GenerateTokensAsync(user);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token");

            return await GenerateTokensAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokensAsync(User user)
        {
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]);

            // var tokenDescriptor = new SecurityTokenDescriptor
            // {
            //     Subject = new ClaimsIdentity(new[]
            //     {
            //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //         new Claim(ClaimTypes.Name, user.UserName),
            //         new Claim(ClaimTypes.Email, user.Email)
            //     }),
            //     Expires = DateTime.UtcNow.AddMinutes(15),
            //     SigningCredentials = new SigningCredentials(
            //         new SymmetricSecurityKey(key),
            //         SecurityAlgorithms.HmacSha256Signature)
            // };

            // var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            // persist refresh token update
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = tokenDescriptor.Expires!.Value,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

    }
}
