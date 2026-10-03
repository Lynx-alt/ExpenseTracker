using System.Net.Http.Json;
using ExpenseTracker.Shared.DTOs;

namespace ExpenseTracker.Client.Services;

public class ExpenseService
{
    private readonly HttpClient _httpClient;

    public ExpenseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ExpenseDto>> GetExpensesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ExpenseDto>>("api/expenses");
        return result ?? new List<ExpenseDto>();
    }

    public async Task<bool> CreateExpenseAsync(CreateExpenseDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/expenses", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/expenses/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CategorySummaryDto>> GetSummaryByCategoryAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<CategorySummaryDto>>("api/expenses/summary-by-category");
        return result ?? new List<CategorySummaryDto>();
    }

    public async Task<List<MonthlySummaryDto>> GetSummaryByMonthAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<MonthlySummaryDto>>("api/expenses/summary-by-month");
        return result ?? new List<MonthlySummaryDto>();
    }

    public async Task<MonthComparisonDto?> GetMonthComparisonAsync()
    {
        return await _httpClient.GetFromJsonAsync<MonthComparisonDto>("api/expenses/month-comparison");
    }
}