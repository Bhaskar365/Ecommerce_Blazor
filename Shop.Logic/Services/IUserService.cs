﻿using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Services
{
    public interface IUserService
    {
        List<CategoryModel> GetCategories();
        List<ProductModel> GetProductByCategoryId(int categoryId);
        ResponseModel RegisterUser(RegisterModel registerModel);
        ResponseModel LoginUser(LoginModel loginModel);
        ResponseModel Checkout(List<CartModel> cartItems);
        List<CustomerOrder> GetOrdersByCustomerId(int customerId);
        List<CartModel> GetOrderDetailForCustomer(int customerId, string order_number);
        ResponseModel ChangePassword(PasswordModel passwordModel);
        List<string> GetShippingStatusForOrder(string order_number);
        Task<string> MakePaymentStripe(string cardNumber, int expMonth, int expYear, string cvc, decimal value);
        Task<string> MakePaymentPaypal(double total);
    }
}
