using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ExpenseService.DataBase;
using ExpenseService.DTO;
using ExpenseService.Interfaces;
using ExpenseService.models;
using ExpenseService.TokenApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseService.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthOptions _authOptions;

    public AuthService(AppDbContext db, IPasswordHasher<User> passwordHasher, IOptions<AuthOptions> authOptions)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _authOptions = authOptions.Value;
    }

    public async Task RegisterAsync(RegisterDTO registerDto, CancellationToken ct)
    {
        var email = registerDto.Email.Trim();

        var exists = await _db.users.AnyAsync(u => u.Email == email, ct);
        if (exists)
        {
            throw new ArgumentException("User with this email already exists");
        }

        var user = new User
        {
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

        await _db.users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto, CancellationToken ct)
    {
        var user = await _db.users.FirstOrDefaultAsync(u => u.Email == loginDto.Email, ct);
        if (user == null)
        {
            throw new ArgumentException("Invalid email or password");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            if (user.PasswordHash != loginDto.Password)
            {
                throw new ArgumentException("Invalid email or password");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, loginDto.Password);
            await _db.SaveChangesAsync(ct);
        }
        else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, loginDto.Password);
            await _db.SaveChangesAsync(ct);
        }

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var jwt = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            notBefore: now,
            claims: claims,
            expires: now.AddMinutes(_authOptions.LifetimeMinutes),
            signingCredentials: new SigningCredentials(
                _authOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new AuthResponseDTO
        {
            Token = token
        };
    }
}
