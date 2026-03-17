using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ExpenseService.DataBase;
using ExpenseService.DTO;
using ExpenseService.Interfaces;
using ExpenseService.models;
using ExpenseService.TokenApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;


namespace ExpenseService.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;


    public AuthService(AppDbContext db)
    {
        _db = db;
    }


    public async Task RegisterAsync(RegisterDTO registerDto, CancellationToken ct)
    {
        var email = registerDto.Email;

        var exists = await _db.users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            throw new Exception("User already exists");
        }

        var user = new User
        {
            Email = email,
            PasswordHash = registerDto.Password, // временно, потом заменим на нормальный хэш
            CreatedAt = DateTime.UtcNow
        };

        await _db.users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }


    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto, CancellationToken ct)
    {
        var user = await _db.users.FirstOrDefaultAsync(u => u.Email == loginDto.Email, ct);

        if (user == null)
        {
            throw new Exception("Invalid Email or Password");
        }

        if (user.PasswordHash != loginDto.Password)
        {
            throw new Exception("Invalid Email or Password");
        }

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new AuthResponseDTO
        {
            Token = token
        };
    }
}