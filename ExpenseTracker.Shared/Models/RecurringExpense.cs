namespace ExpenseTracker.Shared.Models
{
    public class RecurringExpense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public DateTime NextDueDate { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        // Tutte le Expense generate nel tempo da questa regola ricorrente
        public List<Expense> GeneratedExpenses { get; set; } = new();
    }
}
