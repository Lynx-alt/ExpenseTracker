using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : BaseApiController
{
    private readonly ExpenseTrackerDbContext _context;

    public BudgetsController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/budgets
    [HttpGet]
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BudgetDto>>> GetBudgets()
    {
        var currentUserId = CurrentUserId;

        var budgets = await _context.Budgets
            .Where(b => b.UserId == currentUserId)
            .Include(b => b.Category)
            .Select(b => new BudgetDto
            {
                Id = b.Id,
                MonthlyLimit = b.MonthlyLimit,
                CategoryId = b.CategoryId,
                CategoryName = b.Category!.Name
            })
            .ToListAsync();

        return Ok(budgets);
    }

    // GET: api/budgets/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetDto>> GetBudget(int id)
    {
        var currentUserId = CurrentUserId;

        var budget = await _context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUserId);

        if (budget == null)
        {
            return NotFound();
        }

        return Ok(new BudgetDto
        {
            Id = budget.Id,
            MonthlyLimit = budget.MonthlyLimit,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category!.Name
        });
    }

    // POST: api/budgets
    [HttpPost]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetDto>> CreateBudget(CreateBudgetDto dto)
    {
        var currentUserId = CurrentUserId;

        var alreadyExists = await _context.Budgets
            .AnyAsync(b => b.CategoryId == dto.CategoryId && b.UserId == currentUserId);

        if (alreadyExists)
        {
            return BadRequest(new { error = "Esiste già un budget per questa categoria." });
        }

        var budget = new Budget
        {
            MonthlyLimit = dto.MonthlyLimit,
            CategoryId = dto.CategoryId,
            UserId = currentUserId
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        await _context.Entry(budget).Reference(b => b.Category).LoadAsync();

        var result = new BudgetDto
        {
            Id = budget.Id,
            MonthlyLimit = budget.MonthlyLimit,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category!.Name
        };

        return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, result);
    }

    // PUT: api/budgets/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBudget(int id, CreateBudgetDto dto)
    {
        var currentUserId = CurrentUserId;

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUserId);

        if (budget == null)
        {
            return NotFound();
        }

        budget.MonthlyLimit = dto.MonthlyLimit;
        budget.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/budgets/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        var currentUserId = CurrentUserId;

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUserId);

        if (budget == null)
        {
            return NotFound();
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}