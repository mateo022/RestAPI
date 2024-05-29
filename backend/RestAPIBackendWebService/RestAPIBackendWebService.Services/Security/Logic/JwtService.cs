using RestAPIBackendWebService.Business.Auth.Logic;
using RestAPIBackendWebService.Domain.Auth.Constants;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using RestAPIBackendWebService.Services.Security.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestAPIBackendWebService.Domain.Auth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestAPIBackendWebService.Services.Security.Logic
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthTokenModel BuildToken(LoginRequestDTO userCredentials, List<string> userIds)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, userCredentials.Email)
            };

            //Add roles to claims
            foreach (var userId in userIds)
            {
                claims.Add(new Claim(CustomClaimTypes.UserId, userId));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var applicationCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:SessionDuration"]));

            var securityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], 
                audience: null, 
                claims: claims, 
                expires: expiration, 
                signingCredentials: 
                applicationCredentials);

            return new AuthTokenModel
            {
                TokenValue = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }
    }
}
