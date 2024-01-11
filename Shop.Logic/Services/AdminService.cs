using Shop.DataModels.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Services
{
    public class AdminService : IAdminService
    {
        public ResponseModel AdminLogin(LoginModel loginModel)
        {
            ResponseModel responseModel = new ResponseModel();
            responseModel.Status = false;
            responseModel.Message = "Incorrect Id/Password";
            return responseModel;
        }
    }
}
