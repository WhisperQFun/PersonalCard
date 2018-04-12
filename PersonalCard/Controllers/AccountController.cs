using Microsoft.AspNetCore.Mvc;
using PersonalCard.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using PersonalCard.Services;
using PersonalCard.Encrypt;
using PersonalCard.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using QRCoder;
using System.DrawingCore;

using Microsoft.AspNetCore.Hosting;

namespace PersonalCard.Controllers
{
    public class AccountController : Controller
    {
        private  mysqlContext _context;
        BlockchainService blockchainService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public AccountController(mysqlContext context, BlockchainService service, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            blockchainService = service;
            _context = context;
        }

        //[Authorize(Roles = "admin, user")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.User.FirstOrDefaultAsync(u => u.Login == model.login);
                if (user == null)
                {
                    
                    user = new User { Login = model.login, Password = await ShaEncoder.GenerateSHA256String( model.password),
                        type_of_bloud =model.type_of_blood,Hash = await ShaEncoder.GenerateSHA256String(model.login+model.password+model.code_phrase),
                        token = await ShaEncoder.GenerateSHA256String(model.login + model.password + DateTime.Now.ToString())
                        
                };
                    Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                    if (userRole != null)
                        user.Role = userRole;

                    _context.User.Add(user);
                    await _context.SaveChangesAsync();
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("http://blockchain.whisperq.ru/medical/add?token="+user.token, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(10, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(webRootPath + "/images/piedPiper.png"));
                    qrCodeImage.Save(webRootPath+"/images/QR/" +user.Login+".jpg");

                    QRCodeGenerator qrGenerator1 = new QRCodeGenerator();
                    QRCodeData qrCodeData1 = qrGenerator1.CreateQrCode("http://blockchain.whisperq.ru/medical/Emergency?token=" + user.token, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode1 = new QRCode(qrCodeData1);
                    Bitmap qrCodeImage1 = qrCode1.GetGraphic(10, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(webRootPath + "/images/piedPiper.png"));
                    qrCodeImage1.Save(webRootPath + "/images/QR/" + user.Login + "_emerg.jpg");
                    await Authenticate(user); 

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string pass = await ShaEncoder.GenerateSHA256String(model.password);
                User user = await _context.User
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Home()
        {
            User user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            HomeModel model = new HomeModel();
            model.user_login = user.Login;
            model.user_link = user.Login + ".jpg";
            model.user_link_emerg = user.Login + "_emerg.jpg";
            model.user_balance = user.balance.ToString();
            return View(model);
        }


        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}

    