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
using Microsoft.AspNetCore.Hosting;
using QRCoder;
using System.DrawingCore;

namespace PersonalCard.Controllers
{
    public class MedicalController : Controller
    {
        private mysqlContext _context;
        BlockchainService blockchainService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public MedicalController(mysqlContext context, BlockchainService service, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            blockchainService = service;
            blockchainService.Initialize();
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
			List<Medical> medicals = new List<Medical>();
			User user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            List<Block> blocks = _context.Block.Where(u => u.wallet_hash == user.Hash).ToList();

            foreach (var bloks in blocks)
			{
				medicals.Add(JsonConvert.DeserializeObject<Medical>(bloks.data));

			}


			return View(medicals);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Add([FromQuery]string token)
        {
            if (token!=null)
            {
                User user = await _context.User.FirstOrDefaultAsync(u => u.token == token);
                MedicalModel model = new MedicalModel();
                model.Hash = user.Hash;

                user.token = await ShaEncoder.GenerateSHA256String(user.Login + user.Password + DateTime.Now.ToString());
                _context.User.Update(user);
                await _context.SaveChangesAsync();
                string webRootPath = _hostingEnvironment.WebRootPath;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode("http://blockchain.whisperq.ru/medical/add?token=" + user.token, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(10, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(webRootPath + "/images/piedPiper.png"));
                qrCodeImage.Save(webRootPath + "/images/QR/" + user.Login + ".jpg");
                return View(model);//RedirectToAction("Medical", "AddFinaly", model);
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromQuery]string token, string Hash,
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


                return RedirectToAction("Index", "Home");

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