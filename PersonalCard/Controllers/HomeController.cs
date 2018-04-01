using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalCard.Blockchain;
using PersonalCard.Models;
using PersonalCard.Services;

namespace PersonalCard.Controllers
{
        public class HomeController : Controller
        {
            BlockchainService blockchainService;
            public HomeController(BlockchainService service)
            {
                blockchainService = service;
                blockchainService.Initialize();
            }
            public async Task<IActionResult> Index(int id)
            {
            /* blockchainService.AddBlockAsync(await blockchainService.generateNextBlockAsync("Test"));
             Block block = await blockchainService.getLatestBlockAsync();
             if (block != null)
                 return Content($"Block: {block.data}" + $" Hash: {block.hash}" + $" PrevHash: {block.previousHash}");
             return Content("Product not found");*/
            return View();
            }
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
    }

