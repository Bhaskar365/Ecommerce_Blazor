using Microsoft.EntityFrameworkCore;
using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
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
        public UserService(ShoppingCartDBContext context)
        {
            _context = context;
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
                    Customer _customer = new Customer();
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
                OrderId = "p" + generator.Next(1,10000000).ToString("D8");
                if(!_context.CustomerOrders.Where(x => x.OrderId == OrderId).Any()) 
                {
                    break;
                }
            }
            return OrderId;
        }
    }
}
