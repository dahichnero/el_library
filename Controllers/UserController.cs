using ElLibrary.Domain.Entities;
using ElLibrary.Domain.Services;
using ElLibrary.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Security.Principal;

namespace ElLibrary.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;
        private const int adminRoleId = 2;
        private const int clientRoleId = 1;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration (RegistrationViewModel registration)
        {
            logger.LogInformation("Происходит регистрация");
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Состояние модели не валидное");
                return View(registration);
            }
            if (await userService.IsUserExistsAsync(registration.Username))
            {
                logger.LogWarning("Пользователь существует!!!");
                ModelState.AddModelError("user_exists", $"Имя пользователя {registration.Username} уже существует!");
                return View(registration);
            }
            try
            {
                await userService.RegistrationAsync(registration.Fullname, registration.Username,registration.Password);
                return RedirectToAction("RegistrationSuccess", "User");
            } 
            catch
            {
                logger.LogWarning("Не удалось зарегистрироваться");
                ModelState.AddModelError("reg_error","Не удалось зарегистрироваться, попробуйте попытку регистрации позже");
                return View(registration);
            }
        }

        public IActionResult RegistrationSuccess ()
        {
            logger.LogInformation("Регистрация успешна!");
            return View();
        }

        private async Task SignIn(User user)
        {
            string role = user.RoleId switch
            {
                adminRoleId=>"admin",
                clientRoleId=>"client",
                _=>throw new ApplicationException("invalid user role")
            };
            List<Claim> claims = new List<Claim>
            {
                new Claim("fullname",user.Fullname),
                new Claim("id",user.Id.ToString()),
                new Claim("role",role),
                new Claim("username",user.Login)
            };
            string authType = CookieAuthenticationDefaults.AuthenticationScheme;
            IIdentity identity = new ClaimsIdentity(claims, authType, "username", "role");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            logger.LogInformation("Происходит вход");
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Состояние модели не валидное");
                return View(loginViewModel);
            }
            User user = await userService.GetUserAsync(loginViewModel.Username, loginViewModel.Password);
            if (user is null)
            {
                logger.LogWarning("Неверные данные дя входа");
                ModelState.AddModelError("log_notuser", "Неверный логин или пароль");
                return View(loginViewModel);
            }
            await SignIn(user);
            logger.LogInformation("Вход успешен");
            return RedirectToAction("Index","Books");
        }

        public async Task<IActionResult> Logout ()
        {
            logger.LogInformation("Выход из системы");
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login","User");
        }

        public IActionResult AccessDenied ()
        {
            logger.LogWarning("Доступа нет");
            return View();
        }
    }
}
