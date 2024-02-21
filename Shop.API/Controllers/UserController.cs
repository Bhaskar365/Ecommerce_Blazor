using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Logic.Services;

namespace Shop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("GetCategories")]
        public IActionResult GetCategories()
        {
            var data = _userService.GetCategories();
            return Ok(data);
        }
    }
}
