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
                return View(model);
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Add(MedicalModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.User.FirstOrDefaultAsync(u => u.Hash == model.Hash);
                Medical medical = new Medical() { diagnosis = model.diagnosis, diagnosis_fully = model.diagnosis_fully, first_aid = model.first_aid, drugs = model.drugs, is_important = model.is_important };

                string json = JsonConvert.SerializeObject(medical);
                blockchainService.AddBlockAsync(await blockchainService.generateNextBlockAsync(json, model.Hash));


                return RedirectToAction("Index", "Account");
            }
            return View(model);
        }

        public async Task<IActionResult> Emergency([FromQuery]string Hash)
        {
            return View();
        }
    }
}