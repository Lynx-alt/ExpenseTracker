using ExpenseTracker.Shared.Models;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracker.Api.Data;

public class ExpenseTrackerDbContext : DbContext
{
    public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<RecurringExpense> RecurringExpenses => Set<RecurringExpense>();
    public DbSet<Budget> Budgets => Set<Budget>();
}