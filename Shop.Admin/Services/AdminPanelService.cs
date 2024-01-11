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
            //return await httpClient.PostAsJsonAsync<ResponseModel>("api/admin/AdminLogin", loginModel);
            
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<LoginModel>("api/admin/AdminLogin", loginModel);
            ResponseModel result = await response.Content.ReadFromJsonAsync<ResponseModel>();
            
            return result;

        }
    }
}
