namespace ExpenseTracker.Shared.DTOs;

public class BudgetDto
{
    public int Id { get; set; }
    public decimal MonthlyLimit { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}