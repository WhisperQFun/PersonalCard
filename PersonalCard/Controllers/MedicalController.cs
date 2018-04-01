using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalCard.Context;
using PersonalCard.Models;
using PersonalCard.Encrypt;
using PersonalCard.Services;
using Newtonsoft.Json;
using PersonalCard.Blockchain;

namespace PersonalCard.Controllers
{
    public class MedicalController : Controller
    {
        private mysqlContext _context;
        BlockchainService blockchainService;
        public MedicalController(mysqlContext context, BlockchainService service)
        {
            blockchainService = service;
            blockchainService.Initialize();
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Add([FromQuery]string hash, [FromQuery]string Key)
        {
            User user = await _context.User.FirstOrDefaultAsync(u => u.Hash == hash);
            MedicalModel model = new MedicalModel();
            

            if (await ShaEncoder.GenerateSHA256String(user.Login+user.Password+Key) == hash)
            {
                model.Hash = hash.ToString();
                model.key_frase = Key;
                return //RedirectToAction("Medical", "AddFinaly", model);
                    View(model);
            }

            return View();
        }

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


                return RedirectToAction("Index", "Account");

            }
            catch
            {
                return View();
            }
               
            /*return Content($"Hash: {Hash}" + $" diagnosis: {diagnosis}" + $" Важно: {is_important}" + $" Диагноз полностью: {diagnosis_fully}"
                 + $" Первая помощь: {first_aid}" + $" Лекарства: {drugs}");*/
        }

        public async Task<IActionResult> Emergency([FromQuery]string Hash)
        {
            return View();
        }
    }
}