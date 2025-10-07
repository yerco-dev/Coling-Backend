using Coling.Application.Interfaces.Services;
using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Entities.UsersManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Coling.Infrastructure.Services;

public class JwtService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ActionResponse<string>> GenerateTokenAsync(User user, IEnumerable<Claim> claims)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada.");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ColingAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "ColingClient";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Person!.FirstNames  ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "" ),
            new Claim("PersonId", user.PersonId.ToString()),
            new Claim("FullName", user.Person.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        tokenClaims.AddRange(claims);

        var expiration = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: tokenClaims,
            expires: expiration,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new ActionResponse<string>
        {
            WasSuccessful = true,
            Result = tokenString,
            ResultCode = ResultCode.Ok
        });
    }
}

