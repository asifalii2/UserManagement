using System.Net;
using UserManagement.Web.Models;

namespace UserManagement.Web.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserViewModel>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<UserViewModel>>(
                "api/users") ?? new List<UserViewModel>();
        }

        public async Task<UserViewModel?> GetByIdAsync(int id)
        {
            using var response = await _httpClient.GetAsync($"api/users/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserViewModel>();
        }

        public async Task<HttpResponseMessage> CreateAsync(
            CreateUserViewModel model)
        {
            return await _httpClient.PostAsJsonAsync("api/users", model);
        }

        public async Task<HttpResponseMessage> UpdateAsync(
            UpdateUserViewModel model)
        {
            return await _httpClient.PutAsJsonAsync(
                $"api/users/{model.Id}", model);
        }

        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            return await _httpClient.DeleteAsync($"api/users/{id}");
        }
    }
}
