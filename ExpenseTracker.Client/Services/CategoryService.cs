using System.Net.Http.Json;
using ExpenseTracker.Shared.DTOs;

namespace ExpenseTracker.Client.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        return result ?? new List<CategoryDto>();
    }

    public async Task<bool> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", dto);
        return response.IsSuccessStatusCode;
    }
}
