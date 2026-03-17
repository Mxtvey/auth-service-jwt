namespace ExpenseService.models;



public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public List<Expense> Expenses { get; set; } = new();
}