using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Inserisci l'email.")]
    [EmailAddress(ErrorMessage = "Email non valida.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inserisci la password.")]
    public string Password { get; set; } = string.Empty;
}