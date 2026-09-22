namespace ExpenseTracker.Api.DTOs;

public class RecurringExpenseDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public DateTime NextDueDate { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}