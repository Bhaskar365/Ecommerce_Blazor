using Shop.DataModels.CustomModels;

namespace Shop.Admin.Services
{
    public interface IAdminPanelService
    {
        Task<ResponseModel> AdminLogin(LoginModel loginModel);
        Task<CategoryModel> SaveCategory(CategoryModel newCategory);
        Task<List<CategoryModel>> GetCategories();
        Task<bool> UpdateCategory(CategoryModel categoryToUpdate);
        Task<bool> DeleteCategory(CategoryModel categoryModel);
        
        //Product methods
        Task<List<ProductModel>> GetProducts();
        Task<bool> DeleteProduct(ProductModel productToDelete);
        Task<ProductModel> SaveProduct(ProductModel newProduct);
    }
}
