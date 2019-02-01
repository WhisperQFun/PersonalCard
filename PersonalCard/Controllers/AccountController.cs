using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalCard.Context;
using PersonalCard.Encrypt;
using PersonalCard.Models;
using PersonalCard.Services;
using QRCoder;

namespace PersonalCard.Controllers
{
    public class AccountController : Controller
    {
        private readonly MySQLContext _context;
        private readonly BlockchainService _blockchainService;

        private readonly IHostingEnvironment _hostingEnvironment;

        public AccountController(MySQLContext context, BlockchainService service,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _blockchainService = service;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize]
        public IActionResult Index() => View();

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Login == model.login);
                if (user == null)
                {
                    user = new User
                    {
                        Login = model.login,
                        Password = await ShaEncoder.GenerateSHA256String(model.password),
                        type_of_bloud = model.type_of_blood,
                        Hash = await ShaEncoder.GenerateSHA256String($"{model.login}{model.password}{model.code_phrase}"),
                        token = await ShaEncoder.GenerateSHA256String($"{model.login}{model.password}{DateTime.Now}")
                    };

                    var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                    if (userRole != null)
                        user.Role = userRole;

                    _context.User.Add(user);
                    await _context.SaveChangesAsync();

                    GenerateUserQRCodes(user);

                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var pass = await ShaEncoder.GenerateSHA256String(model.password);

                var user = await _context.User
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.email && u.Password == pass);

                if (user != null)
                {
                    await Authenticate(user); 
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Home()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            var model = new HomeModel();
            model.user_login = user.Login;
            model.user_link = user.Login + ".jpg";
            model.user_link_emerg = user.Login + "_emerg.jpg";
            model.user_balance = Convert.ToString(user.balance);

            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // Create claim list
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };

            // Create object ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            // Set up authentication cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        private void GenerateUserQRCodes(User user)
        {
            var codeGenerator = new QRCodeGenerator();
            var webRootPath = _hostingEnvironment.WebRootPath;

            var medicalQRCode = new QRCode(codeGenerator.CreateQrCode(
                $"http://blockchain.whisperq.ru/medical/add?token={user.token}",
                QRCodeGenerator.ECCLevel.Q));
            var emergencyQRCode = new QRCode(codeGenerator.CreateQrCode(
                $"http://blockchain.whisperq.ru/medical/Emergency?token={user.token}",
                QRCodeGenerator.ECCLevel.Q));

            var medicalQRImage = medicalQRCode.GetGraphic(10, Color.Black, Color.White,
                (Bitmap)Image.FromFile($"{webRootPath}/images/piedPiper.png"));
            var emergencyQRImage = emergencyQRCode.GetGraphic(10, Color.Black, Color.White,
                (Bitmap)Image.FromFile($"{webRootPath}/images/piedPiper.png"));

            medicalQRImage.Save($"{webRootPath}/images/QR/{user.Login}.jpg");
            emergencyQRImage.Save($"{webRootPath}/images/QR/{user.Login}_emerg.jpg");
        }
    }
}
