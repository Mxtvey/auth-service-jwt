namespace ExpenseService.DTO;

public class ExpenseResponseDTO
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }
}