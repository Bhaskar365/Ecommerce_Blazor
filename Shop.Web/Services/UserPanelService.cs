using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System.Net.Http.Json;

namespace Shop.Web.Services
{
    public class UserPanelService : IUserPanelService
    {
        private readonly HttpClient httpClient;
        private readonly ProtectedSessionStorage sessionStorage;
        public UserPanelService(HttpClient _httpClient, ProtectedSessionStorage _sessionStorage) 
        {
            httpClient = _httpClient;
            sessionStorage = _sessionStorage;
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
        public async Task<ResponseModel> RegisterUser(RegisterModel registerModel)
        {
            var response = await httpClient.PostAsJsonAsync<RegisterModel>("api/user/RegisterUser", registerModel);
            ResponseModel result = await response.Content.ReadFromJsonAsync<ResponseModel>();

            return result;
        }
        public async Task<ResponseModel> LoginUser(LoginModel loginModel) 
        {
            var response = await httpClient.PostAsJsonAsync<LoginModel>("api/user/LoginUser", loginModel);
            ResponseModel result = await response.Content.ReadFromJsonAsync<ResponseModel>();

            return result;
        }
        public async Task<ResponseModel> Checkout(List<CartModel> cartItems) 
        {
            var response = await httpClient.PostAsJsonAsync<List<CartModel>>("api/user/Checkout", cartItems);
            ResponseModel result = await response.Content.ReadFromJsonAsync<ResponseModel>();

            return result;
        }
        public async Task<List<CustomerOrder>> GetOrdersByCustomerId(int customerId) 
        {
            return await httpClient.GetFromJsonAsync<List<CustomerOrder>>("api/user/GetOrdersByCustomerId/?customerId=" + customerId);
        }
        public async Task<List<CartModel>> GetOrderDetailForCustomer(int customerId, string order_number) 
        {
            return await httpClient.GetFromJsonAsync<List<CartModel>>("api/user/GetOrderDetailForCustomer/?customerId=" + customerId + "&order_number=" + order_number);
        }
        public async Task<List<string>> GetShippingStatusForOrder(string order_number) 
        {
            return await httpClient.GetFromJsonAsync<List<string>>("api/user/GetShippingStatusForOrder/?order_number=" + order_number);
        }
        public async Task<ResponseModel> ChangePassword(PasswordModel passwordModel) 
        {
            var response = await httpClient.PostAsJsonAsync<PasswordModel>("api/user/ChangePassword", passwordModel);
            ResponseModel result = await response.Content.ReadFromJsonAsync<ResponseModel>();

            return result;
        }
    }
}
