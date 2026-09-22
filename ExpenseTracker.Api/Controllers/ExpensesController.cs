using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : BaseApiController
{
    private readonly ExpenseTrackerDbContext _context;

    public ExpensesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/expenses
    [HttpGet]
    [ProducesResponseType(typeof(List<ExpenseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ExpenseDto>>> GetExpenses()
    {
        var expenses = await _context.Expenses
            .Include(e => e.Category)
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.CategoryId,
                CategoryName = e.Category!.Name
            })
            .ToListAsync();

        return Ok(expenses);
    }

    // GET: api/expenses/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpenseDto>> GetExpense(int id)
    {
        var currentUserId = CurrentUserId;

        var expense = await _context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == currentUserId);

        if (expense == null)
        {
            return NotFound();
        }

        return Ok(new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            CategoryId = expense.CategoryId,
            CategoryName = expense.Category!.Name
        });
    }

    // POST: api/expenses
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            CategoryId = dto.CategoryId,
            UserId = CurrentUserId
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        await _context.Entry(expense).Reference(e => e.Category).LoadAsync();

        var result = new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            CategoryId = expense.CategoryId,
            CategoryName = expense.Category!.Name
        };

        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, result);
    }

    // PUT: api/expenses/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpense(int id, CreateExpenseDto dto)
    {
   
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);

        if (expense == null)
        {
            return NotFound();
        }

        expense.Description = dto.Description;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;
        expense.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/expenses/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            return NotFound();
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/expenses/summary-by-month
    [HttpGet("summary-by-month")]
    [ProducesResponseType(typeof(List<MonthlySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MonthlySummaryDto>>> GetSummaryByMonth()
    {
        var summary = await _context.Expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new MonthlySummaryDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .OrderByDescending(s => s.Year).ThenByDescending(s => s.Month)
            .ToListAsync();


        return Ok(summary);
    }

    // GET: api/expenses/month-comparison
    [HttpGet("month-comparison")]
    [ProducesResponseType(typeof(MonthComparisonDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<MonthComparisonDto>> GetMonthComparison()
    {
        var now = DateTime.Now;
        var previousMonthDate = now.AddMonths(-1);

        var currentMonthTotal = await _context.Expenses
            .Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month)
            .SumAsync(e => e.Amount);

        var previousMonthTotal = await _context.Expenses
            .Where(e => e.Date.Year == previousMonthDate.Year && e.Date.Month == previousMonthDate.Month)
            .SumAsync(e => e.Amount);

        decimal? percentageChange = previousMonthTotal == 0
            ? null
            : ((currentMonthTotal - previousMonthTotal) / previousMonthTotal) * 100;

        return Ok(new MonthComparisonDto
        {
            CurrentMonthTotal = currentMonthTotal,
            PreviousMonthTotal = previousMonthTotal,
            PercentageChange = percentageChange
        });
    }

    // GET: api/expenses/summary-by-category
    [HttpGet("summary-by-category")]
    [ProducesResponseType(typeof(List<CategorySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategorySummaryDto>>> GetSummaryByCategory()
    {
        var now = DateTime.Now;

        var summary = await _context.Expenses
            .Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month)
            .GroupBy(e => new { e.CategoryId, e.Category!.Name })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToListAsync();

        return Ok(summary);
    }

}