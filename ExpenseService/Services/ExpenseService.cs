using ExpenseService.DataBase;
using ExpenseService.DTO;
using ExpenseService.Interfaces;
using ExpenseService.models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseService.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _db;

    public ExpenseService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ExpenseResponseDTO> CreateExpenseAsync(CreateExpenseDTO expenseDto, CancellationToken ct, int userId)
    {
        var userExists = await _db.users.AnyAsync(u => u.Id == userId, ct);
        if (!userExists)
        {
            throw new KeyNotFoundException("User not found");
        }

        var expense = new Expense
        {
            UserId = userId,
            Amount = expenseDto.Amount,
            Date = expenseDto.Date,
            Description = expenseDto.Description,
            Category = expenseDto.Category,
            CreatedAt = DateTime.UtcNow
        };

        await _db.expenses.AddAsync(expense, ct);
        await _db.SaveChangesAsync(ct);

        return MapExpense(expense);
    }

    public async Task<IReadOnlyList<ExpenseResponseDTO>> GetExpensesAsync(CancellationToken ct, int userId)
    {
        return await _db.expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .Select(e => new ExpenseResponseDTO
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Description = e.Description,
                Category = e.Category,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<ExpenseResponseDTO> UpdateExpenseAsync(UpdateExpenseDTO expenseDto, CancellationToken ct, int userId, int expenseId)
    {
        var expense = await _db.expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId, ct);

        if (expense == null)
        {
            throw new KeyNotFoundException("Expense not found");
        }

        expense.Amount = expenseDto.Amount;
        expense.Date = expenseDto.Date;
        expense.Description = expenseDto.Description;
        expense.Category = expenseDto.Category;

        await _db.SaveChangesAsync(ct);

        return MapExpense(expense);
    }

    public async Task DeleteExpenseAsync(CancellationToken ct, int userId, int expenseId)
    {
        var expense = await _db.expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId, ct);

        if (expense == null)
        {
            throw new KeyNotFoundException("Expense not found");
        }

        _db.expenses.Remove(expense);
        await _db.SaveChangesAsync(ct);
    }

    private static ExpenseResponseDTO MapExpense(Expense expense)
    {
        return new ExpenseResponseDTO
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Date = expense.Date,
            Description = expense.Description,
            Category = expense.Category,
            CreatedAt = expense.CreatedAt
        };
    }
}
