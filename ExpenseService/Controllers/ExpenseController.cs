using System.Security.Claims;
using ExpenseService.DTO;
using ExpenseService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseService.Controllers;

[ApiController]
[Authorize]
[Route("expenses")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _expenseService.GetExpensesAsync(ct, userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _expenseService.CreateExpenseAsync(dto, ct, userId);
        return Ok(result);
    }

    [HttpPut("{expenseId:int}")]
    public async Task<IActionResult> UpdateExpense([FromBody] UpdateExpenseDTO dto, [FromRoute] int expenseId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _expenseService.UpdateExpenseAsync(dto, ct, userId, expenseId);
        return Ok(result);
    }

    [HttpDelete("{expenseId:int}")]
    public async Task<IActionResult> DeleteExpense([FromRoute] int expenseId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _expenseService.DeleteExpenseAsync(ct, userId, expenseId);
        return NoContent();
    }

    private bool TryGetUserId(out int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out userId);
    }
}
