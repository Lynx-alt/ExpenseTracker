namespace ExpenseTracker.Api.DTOs;

public class CreateBudgetDto
{
    public decimal MonthlyLimit { get; set; }
    public int CategoryId { get; set; }
}