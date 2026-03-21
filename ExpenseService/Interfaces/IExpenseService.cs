using ExpenseService.DTO;

namespace ExpenseService.Interfaces;

public interface IExpenseService
{
    public Task<ExpenseResponseDTO>  CreateExpenseAsync(CreateExpenseDTO expenseDto , CancellationToken ct,int userId);
    public Task<IReadOnlyList<ExpenseResponseDTO>> GetExpensesAsync(CancellationToken ct, int userId);
    public Task<ExpenseResponseDTO> UpdateExpenseAsync(UpdateExpenseDTO expenseDto , CancellationToken ct,int userId, int expenseId);
    public Task DeleteExpenseAsync(CancellationToken ct, int userId, int expenseId);
}
