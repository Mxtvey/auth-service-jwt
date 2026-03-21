using System.ComponentModel.DataAnnotations;

namespace ExpenseService.DTO;

public class CreateExpenseDTO
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}
