using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Controllers;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Shared.Models;
using Xunit;

namespace ExpenseTracker.Tests;

public class CategoriesControllerTests
{
    private ExpenseTrackerDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ExpenseTrackerDbContext(options);
    }

    [Fact]
    public async Task GetCategories_WithNoCategories_ReturnsEmptyList()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CategoriesController(context);

        // Act
        var result = await controller.GetCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var categories = Assert.IsType<List<CategoryDto>>(okResult.Value);
        Assert.Empty(categories);
    }

    [Fact]
    public async Task GetCategory_WithExistingId_ReturnsCategory()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = new Category { Name = "Bar", UserId = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var controller = new CategoriesController(context);

        // Act
        var result = await controller.GetCategory(category.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<CategoryDto>(okResult.Value);
        Assert.Equal("Bar", dto.Name);
    }

    [Fact]
    public async Task GetCategory_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CategoriesController(context);

        // Act
        var result = await controller.GetCategory(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreatedWithId()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CategoriesController(context);
        var dto = new CreateCategoryDto { Name = "Affitto" };

        // Act
        var result = await controller.CreateCategory(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var categoryDto = Assert.IsType<CategoryDto>(createdResult.Value);
        Assert.Equal("Affitto", categoryDto.Name);
        Assert.True(categoryDto.Id > 0);
    }

    [Fact]
    public async Task UpdateCategory_WithExistingId_UpdatesNameAndReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = new Category { Name = "Bar", UserId = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var controller = new CategoriesController(context);
        var dto = new CreateCategoryDto { Name = "Bar & Ristoranti" };

        // Act
        var result = await controller.UpdateCategory(category.Id, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var updated = await context.Categories.FindAsync(category.Id);
        Assert.Equal("Bar & Ristoranti", updated!.Name);
    }

    [Fact]
    public async Task UpdateCategory_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CategoriesController(context);
        var dto = new CreateCategoryDto { Name = "Qualsiasi" };

        // Act
        var result = await controller.UpdateCategory(999, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_WithExistingId_RemovesCategoryAndReturnsNoContent()
    {
        // Arrange
        var context = GetInMemoryContext();
        var category = new Category { Name = "Da cancellare", UserId = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var controller = new CategoriesController(context);

        // Act
        var result = await controller.DeleteCategory(category.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var deleted = await context.Categories.FindAsync(category.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteCategory_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CategoriesController(context);

        // Act
        var result = await controller.DeleteCategory(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}