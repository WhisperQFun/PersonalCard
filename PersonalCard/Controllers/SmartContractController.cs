using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalCard.Context;
using PersonalCard.Models;
using PersonalCard.Blockchain;
using PersonalCard.Encrypt;
using PersonalCard.Services;
using Newtonsoft.Json;

namespace PersonalCard.Controllers
{
    public class SmartContractController : Controller
    {

        private mysqlContext _context;
        BlockchainService blockchainService;
        public SmartContractController(mysqlContext context, BlockchainService service)
        {
            _context = context;
            blockchainService = service;

        }
        
        public IActionResult Index()
        {
            return View();
        }
		[Authorize]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        
		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(SmartCcontractModel model)
        {
            User user = null;
            Contract contract = null;
            Transactions transactions = null;
            
            if (ModelState.IsValid)
            {
                user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                if (user != null)
                {
                    string contract_hash = await ShaEncoder.GenerateSHA256String(model.order_sum + model.prepaid_expense+DateTime.Now);
                    contract = new Contract {hash_сustomer = user.Hash, hash_еxecutor = model.hash_еxecutor,
                        order_sum =Convert.ToInt32( model.order_sum)- Convert.ToInt32(model.prepaid_expense),
                        prepaid_expense = model.prepaid_expense,is_freze=true,is_Done=false, contractID = contract_hash };
                    //,Hash = await ShaEncoder.GenerateSHA256String(model.login + model.password + model.code_phrase
                    string json = JsonConvert.SerializeObject(contract);
                    blockchainService.AddBlockAsync(await blockchainService.generateNextBlockAsync(json,user.Hash,model.hash_еxecutor));
                    transactions = new Transactions {original_wallet = user.Hash
                        ,destination_wallet = model.hash_еxecutor,info = json,timestamp = DateTime.Now.ToString() };
                    await _context.Transactions.AddAsync(transactions);
                    await _context.SaveChangesAsync();
					user.balance = Convert.ToInt16(model.prepaid_expense);
					_context.User.Update(user);
					await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);

            //User user = await _context.User.FirstOrDefaultAsync(u => u.Hash == Hash);
            //return View();
        }
    }
}