using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Shared.DTOs;
using ExpenseTracker.Shared.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(ExpenseTrackerDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);

        if (emailExists)
        {
            return BadRequest(new { error = "Email già registrata." });
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return Ok(new { token });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Email o password non corretti." });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new { token });
    }
}