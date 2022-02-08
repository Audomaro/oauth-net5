using BO;
using Microsoft.AspNetCore.Mvc;
using SVC;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [AuthorizeCustom]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            System.Collections.Generic.IEnumerable<UserEntity> users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            UserEntity user = _userService.GetById(id);
            return Ok(user);
        }
    }
}
