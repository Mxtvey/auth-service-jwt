using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseService.TokenApp;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_super_long_secret_key_123456";   // ключ для шифрации
    public const int LIFETIME = 60; // время жизни токена - 1 минута
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}