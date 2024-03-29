﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using Shop.Logic.PayPal;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ShoppingCartDBContext _context;
        public IConfiguration _configuration { get; }
        public UserService(ShoppingCartDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; 
        }
        public List<CategoryModel> GetCategories()
        {
            var data = _context.Categories.ToList();
            List<CategoryModel> _categoryList = new List<CategoryModel>();
            foreach (var c in data)
            {
                CategoryModel _categoryModel = new CategoryModel();
                _categoryModel.Id = c.Id;
                _categoryModel.Name = c.Name;
                _categoryList.Add(_categoryModel);
            }
            return _categoryList;
        }
        public List<ProductModel> GetProductByCategoryId(int categoryId)
        {
            var data = _context.Products.Where(x => x.CategoryId == categoryId).ToList();
            List<ProductModel> _productList = new List<ProductModel>();
            foreach (var p in data)
            {
                ProductModel _productModel = new ProductModel();
                _productModel.Id = p.Id;
                _productModel.Name = p.Name;
                _productModel.Price = (int)p.Price;
                _productModel.ImageUrl = p.ImageUrl;
                _productModel.Stock = (int)p.Stock;
                _productModel.CategoryId = p.Id;
                _productList.Add(_productModel);
            }
            return _productList;
        }
        public ResponseModel RegisterUser(RegisterModel registerModel)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var exist_check = _context.Customers.Where(x => x.Email == registerModel.EmailId).Any();

                if (!exist_check)
                {
                    DataModels.Models.Customer _customer = new DataModels.Models.Customer();
                    _customer.Name = registerModel.Name;
                    _customer.MobileNo = registerModel.MobileNo;
                    _customer.Email = registerModel.EmailId;
                    _customer.Password = registerModel.Password;
                    _context.Add(_customer);
                    _context.SaveChanges();

                    LoginModel loginModel = new LoginModel()
                    {
                        EmailId = registerModel.EmailId,
                        Password = registerModel.Password,
                    };

                    response = LoginUser(loginModel);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Email already registered";
                };

                return response;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An error has occured. Please try again!";
                return response;
            }
        }
        public ResponseModel LoginUser(LoginModel loginModel)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var userData = _context.Customers.Where(val => val.Email == loginModel.EmailId).FirstOrDefault();
                if (userData != null)
                {
                    if (userData.Password == loginModel.Password)
                    {
                        response.Status = true;
                        response.Message = Convert.ToString(userData.Id) + "|" + userData.Name + "|" + userData.Email;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Your password is incorrect";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Email not registered. Please check your email ID";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An error has occured. Please try again";

                return response;
            }
        }
        public ResponseModel Checkout(List<CartModel> cartItems) 
        {
            string OrderId = GenerateOrderId();
            var prods = _context.Products.ToList();

            try 
            {
                var detail = cartItems.FirstOrDefault();
                CustomerOrder _customerOrder = new CustomerOrder();
                _customerOrder.CustomerId = detail.UserId;
                _customerOrder.OrderId = OrderId;
                _customerOrder.PaymentMode = detail.PaymentMode;
                _customerOrder.ShippingAddress = detail.ShippingAddress;
                _customerOrder.ShippingCharges = detail.ShippingCharges;
                _customerOrder.SubTotal = detail.SubTotal;
                _customerOrder.Total = detail.ShippingCharges + detail.SubTotal;
                _customerOrder.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                _customerOrder.UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                _context.CustomerOrders.Add(_customerOrder);

                foreach (var items in cartItems) 
                {
                    OrderDetail _orderDetail = new OrderDetail();
                    _orderDetail.OrderId = OrderId;
                    _orderDetail.ProductId = items.ProductId;
                    _orderDetail.Quantity = items.Quantity;
                    _orderDetail.Price = items.Price;
                    _orderDetail.SubTotal = items.Price * items.Quantity;
                    _orderDetail.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                    _orderDetail.UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                    _context.OrderDetails.Add(_orderDetail);

                    var selected_product = prods.Where(x => x.Id == items.ProductId).FirstOrDefault();
                    selected_product.Stock = selected_product.Stock - items.Quantity;
                    _context.Products.Update(selected_product);
                }

                _context.SaveChanges();

                ResponseModel response = new ResponseModel();
                response.Message = OrderId;

                return response;
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        private string GenerateOrderId() 
        {
            string OrderId = string.Empty;
            Random generator = null;
            for(int i=0;i<1000;i++) 
            {
                generator = new Random();
                OrderId = "P" + generator.Next(1,10000000).ToString("D8");
                if(!_context.CustomerOrders.Where(x => x.OrderId == OrderId).Any()) 
                {
                    break;
                }
            }
            return OrderId;
        }
        public List<CustomerOrder> GetOrdersByCustomerId(int customerId) 
        {
            var _customerOrders = _context.CustomerOrders.Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.Id).ToList();
            return _customerOrders;
        }
        public List<CartModel> GetOrderDetailForCustomer(int customerId, string order_number) 
        {
            List<CartModel> cart_details = new List<CartModel>();

            var customer_order = _context.CustomerOrders.Where(x => x.CustomerId == customerId && x.OrderId == order_number).FirstOrDefault();

            if(customer_order!=null) 
            {
                var order_detail = _context.OrderDetails.Where(x => x.OrderId == order_number).ToList();
                var product_list = _context.Products.ToList();

                foreach (var order in order_detail) 
                {
                    var prod = product_list.Where(x => x.Id == order.ProductId).FirstOrDefault();
                    CartModel _cartModel = new CartModel();
                    _cartModel.ProductName = prod.Name;
                    _cartModel.Price = Convert.ToInt32(order.Price);
                    _cartModel.ProductImage = prod.ImageUrl;
                    _cartModel.Quantity = Convert.ToInt32(order.Quantity);
                    cart_details.Add(_cartModel);
                }

                cart_details.FirstOrDefault().ShippingAddress = customer_order.ShippingAddress;
                cart_details.FirstOrDefault().ShippingCharges = Convert.ToInt32(customer_order.ShippingCharges);
                cart_details.FirstOrDefault().SubTotal = Convert.ToInt32(customer_order.SubTotal);
                cart_details.FirstOrDefault().Total = Convert.ToInt32(customer_order.Total);
                cart_details.FirstOrDefault().PaymentMode = customer_order.PaymentMode;
            }
            return cart_details;
        } 
        public ResponseModel ChangePassword(PasswordModel passwordModel) 
        {
            ResponseModel response = new ResponseModel();
            response.Status = true;

            var _customer = _context.Customers.Where(x => x.Id == passwordModel.UserKey).FirstOrDefault();
            if (_customer != null) 
            {
                _customer.Password = passwordModel.Password;
                _context.Customers.Update(_customer);
                _context.SaveChanges();

                response.Message = "Password updated successfully !!";
            }
            else 
            {
                response.Message = "User ID does not exist. Try again !!";
            }
            return response;
        }
        public List<string> GetShippingStatusForOrder(string order_number) 
        {
            List<string> shipping_status = new List<string>();
            var order = _context.CustomerOrders.Where(x => x.OrderId == order_number).FirstOrDefault();
            if(order != null && order.ShippingStatus != null) 
            {
                shipping_status = order.ShippingStatus.Split("|").ToList();
            }
            return shipping_status;
        }

        public async Task<string> MakePaymentStripe(string cardNumber, int expMonth, int expYear, string cvc, decimal value) 
        {
            try 
            {
                string response = string.Empty;
                StripeConfiguration.ApiKey = "the secret stripe key";
                var optionToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions 
                    {
                        Number = cardNumber,
                        ExpMonth = expMonth.ToString(),
                        ExpYear = expYear.ToString(),
                        Cvc = cvc
                    }
                };

                var serviceToken = new TokenService();
                Token stripetoken = await serviceToken.CreateAsync(optionToken);

                var customer = new Stripe.Customer
                {
                    Name = "Jackson",
                    Address = new Address
                    {
                        Country = "India",
                        City = "Mumbai",
                        Line1 = "304 - Villa Road, Mumbai",
                        PostalCode = "400503"
                    }
                };

                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(value),
                    Currency = "INR",
                    Description = "Test",
                    Source = stripetoken.Id
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                if(charge.Paid) 
                {
                    response = "Success" + "=" + charge.BalanceTransactionId;
                }
                else 
                {
                    return "Fail";
                }
                return response;
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        
        public async Task<string> MakePaymentPaypal(double total) 
        {
            try
            {
                var payPalAPI = new PayPalAPI(_configuration);
                string url = await payPalAPI.getRedirectURLToPayPal(total, "USD");
                return url;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
