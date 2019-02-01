using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PersonalCard.Blockchain;
using PersonalCard.Context;
using PersonalCard.Encrypt;
using PersonalCard.Models;
using PersonalCard.Services;
using QRCoder;

namespace PersonalCard.Controllers
{
    public class MedicalController : Controller
    {
        private readonly MySQLContext _context;
        private readonly BlockchainService _blockchainService;

        private readonly IHostingEnvironment _hostingEnvironment;

        public MedicalController(MySQLContext context, BlockchainService service,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _blockchainService = service;

            _blockchainService.Initialize();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
			var user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            var blocks = _context.Block.Where(u => u.wallet_hash == user.Hash).ToList();
            var medicals = new List<Medical>();

            foreach (var block in blocks)
				medicals.Add(JsonConvert.DeserializeObject<Medical>(block.data));

			return View(medicals);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Add([FromQuery]string token)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.token == token);
            if (user != null)
            {
                var model = new MedicalModel();
                model.Hash = user.Hash;
                user.token = await ShaEncoder.GenerateSHA256String($"{user.Login}{user.Password}{DateTime.Now}");

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                var codeGenerator = new QRCodeGenerator();
                var webRootPath = _hostingEnvironment.WebRootPath;

                var medicalQRCode = new QRCode(codeGenerator.CreateQrCode(
                    $"http://blockchain.whisperq.ru/medical/add?token={user.token}",
                    QRCodeGenerator.ECCLevel.Q));

                var medicalQRImage = medicalQRCode.GetGraphic(10, Color.Black, Color.White,
                    (Bitmap)Image.FromFile($"{webRootPath}/images/piedPiper.png"));

                medicalQRImage.Save($"{webRootPath}/images/QR/{user.Login}.jpg");

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromQuery]string token, string Hash, string diagnosis,
            string diagnosis_fully, string first_aid, string drugs, string is_important)
        {
            bool boolean;
            try
            {
                if (is_important == "true")
                    boolean = true;
                else
                    boolean = false;

                var user = await _context.User.FirstOrDefaultAsync(u => u.Hash == Hash);
                var medical = new Medical
                {
                    diagnosis = diagnosis,
                    diagnosis_fully = diagnosis_fully,
                    first_aid = first_aid,
                    drugs = drugs,
                    is_important = boolean
                };

                string json = JsonConvert.SerializeObject(medical);
                _blockchainService.AddBlockAsync(await _blockchainService.generateNextBlockAsync(json, Hash));

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

		public async Task<IActionResult> Emergency([FromQuery]string token)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.token == token);
            if (user != null)
            {
                user.token = await ShaEncoder.GenerateSHA256String($"{user.Login}{user.Password}{DateTime.Now}");

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                var codeGenerator = new QRCodeGenerator();
                var webRootPath = _hostingEnvironment.WebRootPath;

                var emergencyQRCode = new QRCode(codeGenerator.CreateQrCode(
                    $"http://blockchain.whisperq.ru/medical/Emergency?token={user.token}",
                    QRCodeGenerator.ECCLevel.Q));

                var emergencyQRImage = emergencyQRCode.GetGraphic(10, Color.Black, Color.White,
                    (Bitmap)Image.FromFile($"{webRootPath}/images/piedPiper.png"));

                emergencyQRImage.Save($"{webRootPath}/images/QR/{user.Login}_emerg.jpg");

                List<Medical> medicals = new List<Medical>();
                List<Block> blocks = _context.Block.Where(u => u.wallet_hash == user.Hash).ToList();

                foreach (var block in blocks)
                    medicals.Add(JsonConvert.DeserializeObject<Medical>(block.data));

                return View(medicals);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}