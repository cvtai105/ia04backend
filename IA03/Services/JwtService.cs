using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IA03.Config;
using Microsoft.IdentityModel.Tokens;

namespace IA03.Services
{
    public class JwtService
    {
        private readonly JwtSettings? _jwtSettings;

        public JwtService(IConfiguration configuration)
        {
            _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (_jwtSettings == null)
            {
                throw new ConfigurationErrorsException("JwtSettings not found in configuration");
            }
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        private string _secret => _jwtSettings.Secret ?? throw new ConfigurationErrorsException("JwtSettings.Secret not found in configuration");
        private string? _issuer => _jwtSettings.Issuer;
        private string? _audience => _jwtSettings.Audience;   
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        private readonly TimeSpan _expiration = TimeSpan.FromHours(24);

        public string GenerateToken(string email, Guid id, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Sub, email),
                new(ClaimTypes.Role, role),
                new Claim("UserId", id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
        }
        
    }
}