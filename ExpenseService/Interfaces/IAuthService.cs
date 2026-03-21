using ExpenseService.DTO;

namespace ExpenseService.Interfaces;

public interface IAuthService
{
   public Task RegisterAsync(RegisterDTO registerDto, CancellationToken ct);
   public Task<AuthResponseDTO> LoginAsync(LoginDTO dto, CancellationToken ct);
}