using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseService.TokenApp;

public class AuthOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public int LifetimeMinutes { get; set; } = 60;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
