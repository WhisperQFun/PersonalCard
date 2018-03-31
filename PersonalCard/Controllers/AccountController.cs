using Microsoft.AspNetCore.Mvc;
using PersonalCard.Models;

using System.Threading.Tasks;
using PersonalCard.Encrypt;
using Microsoft.AspNetCore.Identity;

namespace PersonalCard.Controllers
{
    public class AccountController : Controller
        {
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;

            public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }
            [HttpGet]
            public IActionResult Register()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Register(RegisterModel model)
            {
                if (ModelState.IsValid)
                {
                    User user = new User { Login = model.login, Password = model.password, Hash = await ShaEncoder.GenerateSHA256String(model.login + model.password + model.code_phrase) };//model.code_phrase };
                    // добавляем пользователя
                    var result = await _userManager.CreateAsync(user, model.password);
                    if (result.Succeeded)
                    {
                        // установка куки
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return View(model);
            }
        }
    }


    /*public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                using (mysqlContext db = new mysqlContext())
                {
                    user = db.User.FirstOrDefault(u => u.Login == model.email && u.Password == model.password);
                    

                }
                if (user != null)
                {
                    
                    FormsAuthentication.SetAuthCookie(model.email, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using ( db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.email == model.email);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (UserContext db = new UserContext())
                    {
                        Tags tg = new Tags();
                        db.Users.Add(new User { email = model.email, password = model.password, first_name = model.first_name,
                            last_name = model.last_name, middle_name = model.middle_name, UserGender = model.UserGender, about_me = model.about_me,
                            birthday_date = model.birthday_date,role_id = 2, tags= tg.Parse(model.about_me) });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.email == model.email && u.password == model.password).FirstOrDefault();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.email, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
        [Authorize]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Resume()
        {
            ResumeModel model = new ResumeModel();
            User user = null;
            Resume resume = null;
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.email == User.Identity.Name);
                resume = db.Resume.FirstOrDefault(u => u.user_id == user.id);
            }
            if (resume != null)
            {

                model.name = resume.name;
                model.description = resume.description;
                model.wanted_salary = resume.wanted_salary;
                return View(model);
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Resume(ResumeModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                Resume resume = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.email == User.Identity.Name);
                    resume = db.Resume.FirstOrDefault(u => u.user_id == user.id);
                }
                if (resume == null)
                {
                    // создаем нового пользователя
                    using (UserContext db = new UserContext())
                    {
                        db.Resume.Add(new Resume
                        {
                            name = model.name,
                            description = model.description,
                            wanted_salary = model.wanted_salary,
                            user_id = user.id

                        });
                        db.SaveChanges();

                        resume = db.Resume.Where(u => u.id == user.id && u.name == model.name).FirstOrDefault();
                    }
                    // если резюме сохранилось
                    if (resume != null)
                    {
                        
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    using (UserContext db = new UserContext())
                    {
                        resume.name = model.name;
                        resume.description = model.description;
                        resume.wanted_salary = model.wanted_salary;
                        resume.user_id = user.id;

                        db.Resume.AddOrUpdate(resume);
                        db.SaveChanges();

                        
                    }
                    ModelState.AddModelError("", "Резюме обновленно");
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit()
        {
            return View();
        }

    }
    
}

*/
  