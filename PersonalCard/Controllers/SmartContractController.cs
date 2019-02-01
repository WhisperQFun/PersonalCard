using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PersonalCard.Blockchain;
using PersonalCard.Context;
using PersonalCard.Encrypt;
using PersonalCard.Models;
using PersonalCard.Services;

namespace PersonalCard.Controllers
{
    public class SmartContractController : Controller
    {
        private readonly MySQLContext _context;
        private readonly BlockchainService _blockchainService;

        public SmartContractController(MySQLContext context, BlockchainService service)
        {
            _context = context;
            _blockchainService = service;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            var blocks = _context.Block.Where(u => u.wallet_hash == user.Hash
                && u.destination_wallet_hash != null).ToList();
            var contracts = new List<Contract>();

            foreach (var block in blocks)
                contracts.Add(JsonConvert.DeserializeObject<Contract>(block.data));

            return View(contracts);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            var model = new SmartContractModel();

            model.hash_сustomer = user.Hash;
            return View(model);
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(string hash_сustomer, string hash_executor,
            string order_sum, string prepaid_expense, string condition, string is_Done, string is_freze)
        {
            var is_Done_bool = String2Bool.Convert(is_Done);
            var is_freze_bool = String2Bool.Convert(is_Done);

            User user = null;
            Transactions transactions = null;
            
            if (ModelState.IsValid)
            {
                user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                if (user != null)
                {
                    if (user.balance >= Convert.ToDouble(order_sum))
                    {
                        string contract_hash = await ShaEncoder.GenerateSHA256String($"{order_sum}{prepaid_expense}{DateTime.Now}");
                        var contract = new Contract
                        {
                            hash_сustomer = user.Hash,
                            hash_еxecutor = hash_executor,
                            order_sum = Convert.ToInt32(order_sum) - Convert.ToInt32(prepaid_expense),
                            prepaid_expense = prepaid_expense,
                            is_freze = true,
                            is_Done = false,
                            contractID = contract_hash
                        };

                        var json = JsonConvert.SerializeObject(contract);

                        await _blockchainService.AddBlockAsync(await _blockchainService.generateNextBlockAsync(json, user.Hash, hash_executor));

                        transactions = new Transactions
                        {
                            original_wallet = user.Hash,
                            destination_wallet = hash_executor,
                            info = json,
                            timestamp = DateTime.Now.ToString()
                        };

                        user.balance = Convert.ToInt16(prepaid_expense);

                        _context.User.Update(user);
                        await _context.Transactions.AddAsync(transactions);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Home");
                    }

                    return Content("Недостаточно средств");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View();
        }
    }
}
