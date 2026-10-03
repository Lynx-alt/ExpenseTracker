using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Scegli un nome utente.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Il nome utente deve avere tra 3 e 50 caratteri.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inserisci l'email.")]
    [EmailAddress(ErrorMessage = "Email non valida.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Scegli una password.")]
    [MinLength(8, ErrorMessage = "La password deve avere almeno 8 caratteri.")]
    public string Password { get; set; } = string.Empty;
}