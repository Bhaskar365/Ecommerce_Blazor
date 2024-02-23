using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Shop.DataModels.CustomModels;

namespace Shop.Web.Services
{
    public class UserPanelService : IUserPanelService
    {
        private readonly HttpClient httpClient;
        private readonly ProtectedSessionStorage sessionStorage;
        public UserPanelService(HttpClient _httpClient) 
        {
            httpClient = _httpClient;
        }
        public async Task<bool> IsUserLoggedIn()
        {
            bool flag = false;
            var result = await sessionStorage.GetAsync<string>("userKey");
            if (result.Success)
            {
                flag = true;
            }
            return flag;
        }
        public async Task<List<CategoryModel>> GetCategories() 
        {
            return await httpClient.GetFromJsonAsync<List<CategoryModel>>("api/user/GetCategories");
        }
        public async Task<List<ProductModel>> GetProductByCategoryId(int categoryId)
        {
            return await httpClient.GetFromJsonAsync<List<ProductModel>>("api/user/GetProductByCategoryId/?categoryId=" + categoryId);
        }
    }
}
