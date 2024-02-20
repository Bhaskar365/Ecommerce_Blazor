using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Services
{
    public interface IAdminService
    {
        ResponseModel AdminLogin(LoginModel loginModel);
        CategoryModel SaveCategory(CategoryModel newCategory);
        List<CategoryModel> GetCategories();
        bool UpdateCategory(CategoryModel categoryToUpdate);
        bool DeleteCategory(CategoryModel categoryToDelete);
        Category GetCategoryById(int id);

        //product service
        List<ProductModel> GetProducts();
        bool DeleteProduct(ProductModel productToDelete);
        int GetNewProductId();
        ProductModel SaveProduct(ProductModel newProduct);
    }
}
