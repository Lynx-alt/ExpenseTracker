namespace ExpenseTracker.Shared.DTOs;

public class MonthComparisonDto
{
    public decimal CurrentMonthTotal { get; set; }
    public decimal PreviousMonthTotal { get; set; }
    public decimal? PercentageChange { get; set; }
}