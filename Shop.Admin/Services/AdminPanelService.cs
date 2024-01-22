using Shop.DataModels.CustomModels;
using System.Net.Http.Json;
using System.Net.Http;

namespace Shop.Admin.Services
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly HttpClient httpClient;
        public AdminPanelService(HttpClient _httpClient)
        {
            this.httpClient = _httpClient;
        }
        public async Task<ResponseModel> AdminLogin(LoginModel loginModel)
        { 
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<LoginModel>("api/admin/AdminLogin", loginModel);
            ResponseModel result = await response.Content.ReadFromJsonAsync<ResponseModel>();
            
            return result;
        }

        public async Task<CategoryModel> SaveCategory(CategoryModel newCategory) 
        {
            //return await httpClient.postasjsonasync<CategoryModel>("api/admin/SaveCategory", newCategory);

            HttpResponseMessage response = await httpClient.PostAsJsonAsync<CategoryModel>("api/admin/SaveCategory", newCategory);
            CategoryModel result = await response.Content.ReadFromJsonAsync<CategoryModel>();

            return result;
        }
    }
}
