using Shop.DataModels.CustomModels;

namespace Shop.Web.Services
{
    public class UserPanelService : IUserPanelService
    {
        private readonly HttpClient httpClient;

        public UserPanelService(HttpClient _httpClient) 
        {
            httpClient = _httpClient;
        }
        public async Task<List<CategoryModel>> GetCategories() 
        {
            return await httpClient.GetFromJsonAsync<List<CategoryModel>>("api/user/GetCategories");
        }

    }
}
