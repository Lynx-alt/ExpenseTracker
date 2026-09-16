using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Shared.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User? User { get; set; }

        public List<Expense> Expenses { get; set; } = new();
        public Budget? Budget { get; set; }
    }
}
