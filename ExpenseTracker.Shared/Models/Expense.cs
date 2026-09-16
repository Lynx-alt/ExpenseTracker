using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Shared.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        // Valorizzato solo se questa spesa è stata generata automaticamente da una ricorrenza
        public int? RecurringExpenseId { get; set; }
        public RecurringExpense? RecurringExpense { get; set; }
    }
}
