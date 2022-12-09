using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using fut5.Data;

namespace fut5.Authentication
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, string audience,  Atleta atleta);
    }
    public class TokenService : ITokenService
    {
        private TimeSpan ExpiryDuration = new TimeSpan(365, 0, 0, 0); // 1 year...
        public string BuildToken(string key, string issuer, string audience, Atleta atleta)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, atleta.Email),
                new Claim(ClaimTypes.Name, atleta.Nome),
                // new Claim(ClaimTypes.GivenName, atleta.Nome),
                new Claim(ClaimTypes.Role, atleta.IsAdmin ? "admin":"user" ),
                new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
            expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}