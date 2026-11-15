using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Shared.DTOs;
using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecurringExpensesController : BaseApiController
{
    private readonly ExpenseTrackerDbContext _context;

    public RecurringExpensesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/recurringexpenses
    [HttpGet]
    [ProducesResponseType(typeof(List<RecurringExpenseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RecurringExpenseDto>>> GetRecurringExpenses()
    {
        var currentUserId = CurrentUserId;

        var items = await _context.RecurringExpenses
            .Where(r => r.UserId == currentUserId)
            .Select(r => new RecurringExpenseDto
            {
                Id = r.Id,
                Description = r.Description,
                Amount = r.Amount,
                Frequency = r.Frequency.ToString(),
                NextDueDate = r.NextDueDate,
                CategoryId = r.CategoryId,
                CategoryName = r.Category!.Name
            })
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/recurringexpenses/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RecurringExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecurringExpenseDto>> GetRecurringExpense(int id)
    {
        var currentUserId = CurrentUserId;

        var item = await _context.RecurringExpenses
            .Where(r => r.Id == id && r.UserId == currentUserId)
            .Select(r => new RecurringExpenseDto
            {
                Id = r.Id,
                Description = r.Description,
                Amount = r.Amount,
                Frequency = r.Frequency.ToString(),
                NextDueDate = r.NextDueDate,
                CategoryId = r.CategoryId,
                CategoryName = r.Category!.Name
            })
            .FirstOrDefaultAsync();

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    // POST: api/recurringexpenses
    [HttpPost]
    [ProducesResponseType(typeof(RecurringExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecurringExpenseDto>> CreateRecurringExpense(CreateRecurringExpenseDto dto)
    {
        var currentUserId = CurrentUserId;

        if (!await CategoryBelongsToUserAsync(dto.CategoryId, currentUserId))
        {
            return BadRequest(new { error = "Categoria non valida." });
        }

        var item = new RecurringExpense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Frequency = dto.Frequency.ToString(),
            NextDueDate = dto.NextDueDate,
            CategoryId = dto.CategoryId,
            UserId = currentUserId
        };

        _context.RecurringExpenses.Add(item);
        await _context.SaveChangesAsync();

        await _context.Entry(item).Reference(r => r.Category).LoadAsync();

        var result = new RecurringExpenseDto
        {
            Id = item.Id,
            Description = item.Description,
            Amount = item.Amount,
            Frequency = item.Frequency.ToString(),
            NextDueDate = item.NextDueDate,
            CategoryId = item.CategoryId,
            CategoryName = item.Category!.Name
        };

        return CreatedAtAction(nameof(GetRecurringExpense), new { id = item.Id }, result);
    }

    // PUT: api/recurringexpenses/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecurringExpense(int id, CreateRecurringExpenseDto dto)
    {
        var currentUserId = CurrentUserId;

        var item = await _context.RecurringExpenses
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == currentUserId);

        if (item == null)
        {
            return NotFound();
        }

        if (!await CategoryBelongsToUserAsync(dto.CategoryId, currentUserId))
        {
            return BadRequest(new { error = "Categoria non valida." });
        }

        item.Description = dto.Description;
        item.Amount = dto.Amount;
        item.Frequency = dto.Frequency.ToString();
        item.NextDueDate = dto.NextDueDate;
        item.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/recurringexpenses/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecurringExpense(int id)
    {
        var currentUserId = CurrentUserId;

        var item = await _context.RecurringExpenses
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == currentUserId);

        if (item == null)
        {
            return NotFound();
        }

        _context.RecurringExpenses.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Task<bool> CategoryBelongsToUserAsync(int categoryId, int userId) =>
        _context.Categories.AnyAsync(c => c.Id == categoryId && c.UserId == userId);
}