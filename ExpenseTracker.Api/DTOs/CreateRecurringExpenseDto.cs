using ExpenseTracker.Shared.Enums;

namespace ExpenseTracker.Api.DTOs;

public class CreateRecurringExpenseDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public RecurrenceFrequency Frequency { get; set; }
    public DateTime NextDueDate { get; set; }
    public int CategoryId { get; set; }
}