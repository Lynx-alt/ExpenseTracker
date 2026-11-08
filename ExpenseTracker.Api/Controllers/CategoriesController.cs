using ExpenseTracker.Api.Data;
using ExpenseTracker.Shared.DTOs;
using ExpenseTracker.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : BaseApiController
{
    private readonly ExpenseTrackerDbContext _context;

    public CategoriesController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var currentUserId = CurrentUserId;                                  

        var categories = await _context.Categories
            .Where(c => c.UserId == currentUserId)                          
            .OrderBy(c => c.Name)                                       
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })    
            .ToListAsync();                                                 

        return Ok(categories);
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var currentUserId = CurrentUserId;

        var category = await _context.Categories
            .Where(c => c.Id == id && c.UserId == currentUserId)           
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .FirstOrDefaultAsync();                                         

        if (category == null)
        {
            return NotFound();                                              
        }

        return Ok(category);
    }

    // POST: api/categories
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        var currentUserId = CurrentUserId;
        var name = dto.Name.Trim();                                       

        if (await CategoryNameExistsAsync(name, currentUserId))             
        {
            return Conflict(new { error = "Esiste già una categoria con questo nome." });
        }

        var category = new Category { Name = name, UserId = currentUserId };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var result = new CategoryDto { Id = category.Id, Name = category.Name };

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, result);
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto dto)
    {
        var currentUserId = CurrentUserId;

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUserId); 

        if (category == null)
        {
            return NotFound();
        }

        var name = dto.Name.Trim();

        if (await CategoryNameExistsAsync(name, currentUserId, excludeId: id)) 
        {
            return Conflict(new { error = "Esiste già una categoria con questo nome." });
        }

        category.Name = name;
        await _context.SaveChangesAsync();                               

        return NoContent();
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var currentUserId = CurrentUserId;

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUserId);

        if (category == null)
        {
            return NotFound();
        }

        if (await CategoryIsInUseAsync(id))                                  
        {
            return Conflict(new { error = "La categoria è usata da spese, budget o spese ricorrenti e non può essere eliminata." });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Task<bool> CategoryNameExistsAsync(string name, int userId, int? excludeId = null)
    {
        var normalized = name.ToLower();                                    

        return _context.Categories.AnyAsync(c =>
            c.UserId == userId &&
            c.Name.ToLower() == normalized &&                              
            (excludeId == null || c.Id != excludeId));                     
    }

 
    private async Task<bool> CategoryIsInUseAsync(int categoryId) =>
        await _context.Expenses.AnyAsync(e => e.CategoryId == categoryId)
        || await _context.Budgets.AnyAsync(b => b.CategoryId == categoryId)
        || await _context.RecurringExpenses.AnyAsync(r => r.CategoryId == categoryId);
}