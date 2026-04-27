using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GoodHamburger.API.Controllers;

/// <summary>Issues JWT tokens for API access.</summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[Produces("application/json")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    /// <summary>Authenticates a user and returns a JWT token.</summary>
    /// <remarks>Use the credentials configured in appsettings.json (username / password).</remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var users = configuration.GetSection("Users").Get<List<UserConfig>>() ?? [];
        var match = users.FirstOrDefault(u =>
            u.Username == request.Username && u.Password == request.Password);

        if (match is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title  = "Unauthorized",
                Detail = "Invalid username or password."
            });

        var token = GenerateToken(match.Username);
        return Ok(new TokenResponse(token));
    }

    private string GenerateToken(string username)
    {
        var jwt      = configuration.GetSection("JwtSettings");
        var key      = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Secret"]!));
        var creds    = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry   = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiryMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer:             jwt["Issuer"],
            audience:           jwt["Audience"],
            claims:             [new Claim(ClaimTypes.Name, username)],
            expires:            expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed record LoginRequest(string Username, string Password);
public sealed record TokenResponse(string Token);
internal sealed record UserConfig { public string Username { get; init; } = ""; public string Password { get; init; } = ""; }
