using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnetapp.Constants;
using dotnetapp.DTOs;
using dotnetapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dotnetapp.Services
{
    public class AuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, string? Token)>
            RegisterAsync(RegisterDTO dto)
        {
            var validRoles = new[]
            {
                UserRoles.Customer,
                UserRoles.Organizer
            };

            if (!validRoles.Contains(dto.Role))
                return (false, "Invalid role.", null);

            var existing =
                await _userManager.FindByEmailAsync(dto.Email);

            if (existing != null)
                return (false, "Email already registered.", null);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                ProfileImage = dto.ProfileImage,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            var result =
                await _userManager.CreateAsync(
                    user,
                    dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                return (false, errors, null);
            }

            await _userManager.AddToRoleAsync(
                user,
                dto.Role);

            var token = await GenerateTokenAsync(user);

            return (
                true,
                "Registration successful.",
                token
            );
        }

        public async Task<(bool Success, string Message, string? Token)>
            LoginAsync(LoginDTO dto)
        {
            var user =
                await _userManager.FindByEmailAsync(
                    dto.Email);

            if (user == null)
                return (
                    false,
                    "Invalid email or password.",
                    null
                );

            if (!user.IsActive)
                return (
                    false,
                    "Account is inactive.",
                    null
                );

            var valid =
                await _userManager.CheckPasswordAsync(
                    user,
                    dto.Password);

            if (!valid)
                return (
                    false,
                    "Invalid email or password.",
                    null
                );

            var token =
                await GenerateTokenAsync(user);

            return (
                true,
                "Login successful.",
                token
            );
        }

        private async Task<string> GenerateTokenAsync(
            ApplicationUser user)
        {
            var roles =
                await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new Claim(
                    ClaimTypes.Name,
                    user.Name),

                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? "")
            };

            claims.AddRange(
                roles.Select(role =>
                    new Claim(
                        ClaimTypes.Role,
                        role)));

            var key =
                _configuration["Jwt:Key"]
                ?? "EventSphereSuperSecretKeyForDevelopmentOnly123";

            var issuer =
                _configuration["Jwt:Issuer"]
                ?? "EventSphere";

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}