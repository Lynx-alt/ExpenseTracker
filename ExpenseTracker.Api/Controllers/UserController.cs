// UsersController.cs (temporaneo, solo per test)
using ExpenseTracker.Api.Data;
using ExpenseTracker.Shared.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;

    public UsersController(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    [HttpPost("test-user")]
    public async Task<ActionResult> CreateTestUser()
    {
        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "temp" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new { user.Id });
    }
}