using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;

    public CategoriesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        return Ok(categories);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        var category = new Category { Name = dto.Name, UserId = 1 }; // temporaneo, hardcoded

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var result = new CategoryDto { Id = category.Id, Name = category.Name };

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, result);
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(new CategoryDto { Id = category.Id, Name = category.Name });
    }
}