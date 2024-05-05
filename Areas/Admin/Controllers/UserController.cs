using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PharmacityStore.Models;
using PharmacityStore.Repository;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace PharmacityStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult ViewAllUser()
        {
           
            //1 get data
            List<User> lst = _userRepository.GetAll();
            //2 data to view
            return View("ViewAllUser", lst);
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
           

            return View("CreateUser", new User());
        }
        [HttpPost]
        public IActionResult SaveUser(User user)
        {
            if (ModelState.IsValid)
            {
                bool isUserNameExist = _userRepository.checkname(user.UserName);
                if (isUserNameExist)
                {
                    ModelState.AddModelError(string.Empty, "tên đã có");
                    return View("CreateUser");
                }
                _userRepository.Create(user);

                return RedirectToAction("ViewAllUser");

            }
            else
            {
                return View("CreateUser");
            }
        }
        public IActionResult EditUser(Int16 id)
        {
            

            return View("EditUser", _userRepository.FindById(id));
        }
        [HttpPost]
        public IActionResult UpdateUser(User user)
        {

            if (ModelState.IsValid)
            {
                bool isUserNameExist = _userRepository.checkname(user.UserName);
                if (isUserNameExist)
                {
                    ModelState.AddModelError(string.Empty, "tên đã có");
                    return View("EditUser");


                }
                _userRepository.Update(user);

                return RedirectToAction("ViewAllUser");

            }
            else
            {
                return View("EditUser");
            }
        }
        
        public IActionResult Delete(Int16 userId)
        {
           
            _userRepository.Delete(userId);
            return RedirectToAction("ViewAllUser");
        }
        public IActionResult DeleteUser(Int16 id)
        {
            return View("DeleteUser", _userRepository.FindById(id));
        }
      
        [HttpGet]
        public IActionResult Logout()
        {
           
            HttpContext.SignOutAsync(); // Đăng xuất người dùng

            return RedirectToAction("Login", "Home");
        }

    }
}
