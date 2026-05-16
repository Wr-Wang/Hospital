using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace Hospital.Infrastructure.ExternalServices;

public sealed class JwtTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationHours;

    public JwtTokenService(string secretKey, string issuer, string audience, int expirationHours)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
        _expirationHours = expirationHours;
    }

    public string GenerateToken(UserInfo userInfo, string[] permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
            new(ClaimTypes.Name, userInfo.DisplayName),
            new(JwtClaims.CampusName, userInfo.CampusName),
            new(JwtClaims.Permissions, string.Join(",", permissions)),
        };

        if (userInfo.Roles is not null)
        {
            foreach (var role in userInfo.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
