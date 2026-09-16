using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public List<Expense> Expenses { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<RecurringExpense> RecurringExpenses { get; set; } = new();
        public List<Budget> Budgets { get; set; } = new();
    }
}
