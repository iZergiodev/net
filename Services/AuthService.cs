using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using sisNet.models;
using sisNet.Repository; 


namespace sisNet.Services
{
    public class AuthService
    {
        private readonly MockUserRepository _userRepository;
        private readonly string _jwtSecret;

        public AuthService(MockUserRepository userRepository, string jwtSecret)
        {
            _userRepository = userRepository;
            _jwtSecret = jwtSecret;
        }

        public string? Authenticate(string username, string password)
        {
            var user = _userRepository.Authenticate(username, password);
            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateTokenForUser(User user, ClaimsPrincipal? impersonator = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (impersonator != null)
            {
                var originalUserId = impersonator.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var originalUserRole = impersonator.FindFirst(ClaimTypes.Role)?.Value;
                if (!string.IsNullOrEmpty(originalUserId))
                    claims.Add(new Claim("original_user_id", originalUserId));
                if (!string.IsNullOrEmpty(originalUserRole))
                    claims.Add(new Claim("original_user_role", originalUserRole));
                claims.Add(new Claim("is_impersonated", "true"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}