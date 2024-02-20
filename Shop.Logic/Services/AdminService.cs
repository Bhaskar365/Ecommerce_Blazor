using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Services
{
    public class AdminService : IAdminService
    {
        private readonly ShoppingCartDBContext _context;
        public AdminService(ShoppingCartDBContext context)
        {
            _context = context;
        }
        public ResponseModel AdminLogin(LoginModel loginModel)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var userData = _context.AdminInfos.Where(x => x.Email == loginModel.EmailId).FirstOrDefault();
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
                    response.Message = "Email not registered. Please check your email id";
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
        public CategoryModel SaveCategory(CategoryModel newCategory)
        {
            try
            {
                Category _category = new Category();
                _category.Name = newCategory.Name;
                _context.Add(_category);
                _context.SaveChanges();
                return newCategory;
            }
            catch (Exception)
            {

                throw;
            }
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
        public bool UpdateCategory(CategoryModel categoryToUpdate)
        {
                bool flag = false;
                var _category = _context.Categories.Where(x => x.Id == categoryToUpdate.Id).First();
                if (_category != null)
                {
                    _category.Name = categoryToUpdate.Name;
                    _context.Categories.Update(_category);
                    _context.SaveChanges();
                    flag = true;
                }
                return flag;
        }

        public bool DeleteCategory(CategoryModel categoryToDelete) 
        {
            bool flag = false;
            var _category = _context.Categories.Where(x => x.Id == categoryToDelete.Id).First();
            if (_category != null) 
            {
                _context.Categories.Remove(_category);
                _context.SaveChanges();
                return true;
            }
            return flag;
        }

        //product methods
        public List<ProductModel> GetProducts() 
        {
            List<Category> categoryData = _context.Categories.ToList();
            List<Product> productData = _context.Products.ToList();
            List<ProductModel> _productList = new List<ProductModel>();
            foreach(var p in productData) 
            {
                ProductModel _productModel = new ProductModel();
                _productModel.Id = p.Id;
                _productModel.Name = p.Name;
                _productModel.Price = p.Price;
                _productModel.Stock = p.Stock;
                _productModel.ImageUrl = p.ImageUrl;
                _productModel.CategoryId = categoryData.Where(x => x.Id == p.CategoryId).Select(x => x.Id).FirstOrDefault();
                _productModel.CategoryName = categoryData.Where(x => x.Id == p.CategoryId).Select(x => x.Name).FirstOrDefault();
                _productList.Add(_productModel);
            }
            return _productList;
        }
        public bool DeleteProduct(ProductModel productToDelete) 
        {
            bool flag = false;
            var _product = _context.Products.Where(x => x.Id == productToDelete.Id).First();
            if(_product != null) 
            {
                _context.Products.Remove(_product);
                _context.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public int GetNewProductId() 
        {
            try 
            {
                int nextProductId = _context.Products.ToList().OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
                return nextProductId;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        public ProductModel SaveProduct(ProductModel newProduct) 
        {
            try 
            {
                Product _product = new Product();
                _product.Name = newProduct.Name;
                _product.Price = newProduct.Price;
                _product.ImageUrl = newProduct.ImageUrl;
                _product.CategoryId = newProduct.CategoryId;
                _product.Stock = newProduct.Stock;
                _context.Add(_product);
                _context.SaveChanges();
                return newProduct;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.Where(p => p.Id == id).FirstOrDefault();
            //Category _category = new Category();
            //var IDSearch = 
            //var ID = _context.Categories.Where(p => p.Id == id).FirstOrDefault();
            //return ID;
        }
    }
}
