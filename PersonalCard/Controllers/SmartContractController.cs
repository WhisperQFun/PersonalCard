using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalCard.Models;

namespace PersonalCard.Controllers
{
    public class SmartContractController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Add()
        {

            return View();
        }
    }
}