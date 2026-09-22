namespace ExpenseTracker.Shared.DTOs
{
    public class MonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
