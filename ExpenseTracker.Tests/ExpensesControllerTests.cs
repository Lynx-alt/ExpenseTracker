using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Controllers;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Shared.Models;
using Xunit;

namespace ExpenseTracker.Tests;

public class ExpensesControllerTests
{
    private ExpenseTrackerDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ExpenseTrackerDbContext(options);
    }

    private async Task<Category> SeedCategory(ExpenseTrackerDbContext context, string name = "Bar")
    {
        var category = new Category { Name = name, UserId = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task GetExpenses_WithNoExpenses_ReturnsEmptyList()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetExpenses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var expenses = Assert.IsType<List<ExpenseDto>>(okResult.Value);
        Assert.Empty(expenses);
    }

    [Fact]
    public async Task GetExpenses_ReturnsExpenseWithCategoryName()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context, "Bar");

        context.Expenses.Add(new Expense
        {
            Description = "Caffè",
            Amount = 1.50m,
            Date = DateTime.Now,
            CategoryId = category.Id,
            UserId = 1
        });
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetExpenses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var expenses = Assert.IsType<List<ExpenseDto>>(okResult.Value);
        Assert.Single(expenses);
        Assert.Equal("Bar", expenses[0].CategoryName);
    }

    [Fact]
    public async Task GetExpense_WithExistingId_ReturnsExpense()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context);

        var expense = new Expense
        {
            Description = "Caffè",
            Amount = 1.50m,
            Date = DateTime.Now,
            CategoryId = category.Id,
            UserId = 1
        };
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetExpense(expense.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<ExpenseDto>(okResult.Value);
        Assert.Equal("Caffè", dto.Description);
    }

    [Fact]
    public async Task GetExpense_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetExpense(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateExpense_WithValidData_ReturnsCreatedWithCategoryName()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context, "Trasporti");

        var controller = new ExpensesController(context);
        var dto = new CreateExpenseDto
        {
            Description = "Biglietto bus",
            Amount = 2.00m,
            Date = DateTime.Now,
            CategoryId = category.Id
        };

        // Act
        var result = await controller.CreateExpense(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var expenseDto = Assert.IsType<ExpenseDto>(createdResult.Value);
        Assert.Equal("Trasporti", expenseDto.CategoryName);
        Assert.True(expenseDto.Id > 0);
    }

    [Fact]
    public async Task UpdateExpense_WithExistingId_UpdatesFieldsAndReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context);

        var expense = new Expense
        {
            Description = "Originale",
            Amount = 5.00m,
            Date = DateTime.Now,
            CategoryId = category.Id,
            UserId = 1
        };
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);
        var dto = new CreateExpenseDto
        {
            Description = "Modificato",
            Amount = 9.99m,
            Date = expense.Date,
            CategoryId = category.Id
        };

        // Act
        var result = await controller.UpdateExpense(expense.Id, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var updated = await context.Expenses.FindAsync(expense.Id);
        Assert.Equal("Modificato", updated!.Description);
        Assert.Equal(9.99m, updated.Amount);
    }

    [Fact]
    public async Task UpdateExpense_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new ExpensesController(context);
        var dto = new CreateExpenseDto { Description = "x", Amount = 1, Date = DateTime.Now, CategoryId = 1 };

        // Act
        var result = await controller.UpdateExpense(999, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteExpense_WithExistingId_RemovesExpenseAndReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context);

        var expense = new Expense
        {
            Description = "Da cancellare",
            Amount = 3.00m,
            Date = DateTime.Now,
            CategoryId = category.Id,
            UserId = 1
        };
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);

        // Act
        var result = await controller.DeleteExpense(expense.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var deleted = await context.Expenses.FindAsync(expense.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteExpense_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new ExpensesController(context);

        // Act
        var result = await controller.DeleteExpense(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetSummaryByCategory_WithExpensesInCurrentMonth_ReturnsCorrectTotal()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context, "Bar");

        context.Expenses.Add(new Expense { Description = "Caffè", Amount = 1.50m, Date = DateTime.Now, CategoryId = category.Id, UserId = 1 });
        context.Expenses.Add(new Expense { Description = "Cornetto", Amount = 2.00m, Date = DateTime.Now, CategoryId = category.Id, UserId = 1 });
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetSummaryByCategory();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<List<CategorySummaryDto>>(okResult.Value);
        Assert.Single(summary);
        Assert.Equal(3.50m, summary[0].TotalAmount);
    }

    [Fact]
    public async Task GetSummaryByCategory_ExcludesExpensesFromOtherMonths()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context, "Bar");

        context.Expenses.Add(new Expense { Description = "Questo mese", Amount = 10m, Date = DateTime.Now, CategoryId = category.Id, UserId = 1 });
        context.Expenses.Add(new Expense { Description = "Mese scorso", Amount = 999m, Date = DateTime.Now.AddMonths(-1), CategoryId = category.Id, UserId = 1 });
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetSummaryByCategory();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<List<CategorySummaryDto>>(okResult.Value);
        Assert.Equal(10m, summary[0].TotalAmount);
    }

    [Fact]
    public async Task GetMonthComparison_WithNoPreviousMonthExpenses_ReturnsNullPercentageChange()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = await SeedCategory(context);

        context.Expenses.Add(new Expense { Description = "Solo questo mese", Amount = 20m, Date = DateTime.Now, CategoryId = category.Id, UserId = 1 });
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context);

        // Act
        var result = await controller.GetMonthComparison();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var comparison = Assert.IsType<MonthComparisonDto>(okResult.Value);
        Assert.Equal(20m, comparison.CurrentMonthTotal);
        Assert.Equal(0m, comparison.PreviousMonthTotal);
        Assert.Null(comparison.PercentageChange);
    }
}