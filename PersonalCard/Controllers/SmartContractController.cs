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
		public async Task<IActionResult> Add(string hash_сustomer,string hash_executor,string order_sum,string prepaid_expense, string condition,string is_Done,string is_freze)
        {
            bool is_Done_bool = String2bool.convert(is_Done);
            bool is_freze_bool = String2bool.convert(is_Done);
            User user = null;
            Contract contract = null;
            Transactions transactions = null;
            
            if (ModelState.IsValid)
            {
                user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                if (user != null)
                {
                    string contract_hash = await ShaEncoder.GenerateSHA256String(order_sum + prepaid_expense+DateTime.Now);
                    contract = new Contract {hash_сustomer = user.Hash, hash_еxecutor = hash_executor,
                        order_sum =Convert.ToInt32( order_sum)- Convert.ToInt32(prepaid_expense),
                        prepaid_expense = prepaid_expense,is_freze=true,is_Done=false, contractID = contract_hash };
                    //,Hash = await ShaEncoder.GenerateSHA256String(model.login + model.password + model.code_phrase
                    string json = JsonConvert.SerializeObject(contract);
                    blockchainService.AddBlockAsync(await blockchainService.generateNextBlockAsync(json,user.Hash, hash_executor));
                    transactions = new Transactions {original_wallet = user.Hash
                        ,destination_wallet = hash_executor,
                        info = json,timestamp = DateTime.Now.ToString() };
                    await _context.Transactions.AddAsync(transactions);
                    await _context.SaveChangesAsync();
					user.balance = Convert.ToInt16(prepaid_expense);
					_context.User.Update(user);
					await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View();

            //User user = await _context.User.FirstOrDefaultAsync(u => u.Hash == Hash);
            //return View();
        }
    }
}
/*
 
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Add([FromQuery]string hash, [FromQuery]string Key, string Hash,
            string diagnosis, string diagnosis_fully, 
            string first_aid, string drugs, string is_important)
        {
            bool boolean;
            try
            {
                if (is_important == "true")
                {
                    boolean = true; 

                }
                else
                {
                    boolean = false;
                }

                User user = await _context.User.FirstOrDefaultAsync(u => u.Hash == Hash);
                Medical medical = new Medical() { diagnosis = diagnosis, diagnosis_fully = diagnosis_fully, first_aid = first_aid, drugs = drugs, is_important = boolean };

                string json = JsonConvert.SerializeObject(medical);
                blockchainService.AddBlockAsync(await blockchainService.generateNextBlockAsync(json, Hash));


                return RedirectToAction("Index", "Medical");

            }
            catch
            {
                return View();
            }
               
            /*return Content($"Hash: {Hash}" + $" diagnosis: {diagnosis}" + $" Важно: {is_important}" + $" Диагноз полностью: {diagnosis_fully}"
                 + $" Первая помощь: {first_aid}" + $" Лекарства: {drugs}");*/
        
     