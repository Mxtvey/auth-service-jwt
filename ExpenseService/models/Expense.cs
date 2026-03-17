

namespace ExpenseService.models;

public class Expense
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}