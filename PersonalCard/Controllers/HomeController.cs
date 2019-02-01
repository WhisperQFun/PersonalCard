using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalCard.Models;
using PersonalCard.Services;

namespace PersonalCard.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlockchainService _blockchainService;

        public HomeController(BlockchainService service)
        {
            _blockchainService = service;
            _blockchainService.Initialize();
        }

        public IActionResult Index(int id) => View();

        public IActionResult Error() => 
            View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
    }
}

