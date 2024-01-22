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
            //try
            //{
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
           //}
            //catch (Exception)
            //{
            //    throw;
            //}
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
    }
}
