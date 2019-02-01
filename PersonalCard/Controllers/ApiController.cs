using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalCard.Models;
using Microsoft.AspNetCore.Mvc;
using PersonalCard.Blockchain;
using PersonalCard.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PersonalCard.Encrypt;
using PersonalCard.Services;

namespace PersonalCard.Controllers
{
    public class ApiController : Controller
    {
        private readonly MySQLContext _context;
        private readonly BlockchainService _blockchainService;

        // key api : ee85d34b-8443-4c8d-9369-0cfb04c2d79d
        // GET: Api
        // example string http://www.example.com/Api?key=ee85d34b-8443-4c8d-9369-0cfb04c2d79d&target=authorization&email=1@gmail.com&password=1

        public ApiController(MySQLContext context, BlockchainService service)
        {
            _context = context;
            _blockchainService = service;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(string site, string organisation)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            var api = new API
            {
                is_active = true,
                site = site,
                organisation = organisation,
                timestamp = DateTime.Now.ToString(),
                token = await ShaEncoder.GenerateSHA256String($"{site}{organisation}{DateTime.Now}")
            };

            await _context.Api.AddAsync(api);
            await _context.SaveChangesAsync();

            return Content("Token: " + api.token);
        }

        public async Task<IActionResult> login([FromQuery]string token, [FromQuery]string login, [FromQuery]string password)
        {
            if (await _context.Api.FirstOrDefaultAsync(u => u.token == token && u.is_active == true) != null)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
                if (user != null)
                {
                    if (user != null)
                    {
                        try
                        {
                            var data_response = new response_api();
                            data_response.request_Info = new request_info();
                            data_response.request_Info.answer = "OK";
                            data_response.request_Info.code = "200";
                            data_response.send_data = new send_data();
                            data_response.send_data.user = user;

                            var blocks = _context.Block.Where(u => u.wallet_hash == user.Hash).ToList();
                            var medicals = new List<Medical>();

                            foreach (var block in blocks)
                                medicals.Add(JsonConvert.DeserializeObject<Medical>(block.data));

                            data_response.send_data.medicals = medicals;

                            return Content(JsonConvert.SerializeObject(data_response), "application/json");
                        }
                        catch
                        {
                            var answ = new response_api();
                            answ.request_Info = new request_info();
                            answ.request_Info.code = "403";
                            answ.request_Info.answer = "BadInfo";

                            return Content(JsonConvert.SerializeObject(answ), "application/json");
                        }
                    }
                    else
                    {
                        var answ = new response_api();
                        answ.request_Info = new request_info();
                        answ.request_Info.code = "403";
                        answ.request_Info.answer = "BadInfo";

                        return Content(JsonConvert.SerializeObject(answ), "application/json");
                    }
                }
                else
                {
                    var answ = new response_api();
                    answ.request_Info = new request_info();
                    answ.request_Info.code = "400";
                    answ.request_Info.answer = "Error";

                    return Content(JsonConvert.SerializeObject(answ), "application/json");
                }
            }
            else
            {
                var answ = new response_api();
                answ.request_Info = new request_info();
                answ.request_Info.code = "400";
                answ.request_Info.answer = "Error";

                return Content(JsonConvert.SerializeObject(answ), "application/json");
            }
        }

        public async Task<IActionResult> smartcontractadd([FromQuery]string hash_сustomer, [FromQuery]string hash_еxecutor,
            [FromQuery]string order_sum, [FromQuery]string prepaid_expense, [FromQuery]string condition)
        {
            try
            {
                var contract_hash = await ShaEncoder.GenerateSHA256String($"{order_sum}{prepaid_expense}{DateTime.Now}");
                var contract = new Contract
                {
                    hash_сustomer = hash_сustomer,
                    hash_еxecutor = hash_еxecutor,
                    order_sum = Convert.ToInt32(order_sum) - Convert.ToInt32(prepaid_expense),
                    prepaid_expense = prepaid_expense,
                    is_freze = true,
                    is_Done = false,
                    contractID = contract_hash
                };

                var json = JsonConvert.SerializeObject(contract);
                await _blockchainService.AddBlockAsync(await _blockchainService.generateNextBlockAsync(json, hash_сustomer, hash_еxecutor));

                var transactions = new Transactions
                {
                    original_wallet = hash_сustomer,
                    destination_wallet = hash_еxecutor,
                    info = json,
                    timestamp = DateTime.Now.ToString()
                };

                var user = await _context.User.FirstOrDefaultAsync(u => u.Hash == hash_еxecutor);
                user.balance = Convert.ToInt16(prepaid_expense);
                _context.User.Update(user);

                await _context.Transactions.AddAsync(transactions);
                await _context.SaveChangesAsync();

                var data_response = new response_api();
                data_response.request_Info = new request_info();
                data_response.request_Info.answer = "OK";
                data_response.request_Info.code = "200";
                data_response.send_data = new send_data();

                List<Block> blocks = _context.Block.Where(u => u.wallet_hash == hash_сustomer && u.destination_wallet_hash == hash_еxecutor).ToList();

                List<Contract> contracts = new List<Contract>();
                foreach (var block in blocks)
                    contracts.Add(JsonConvert.DeserializeObject<Contract>(block.data));

                data_response.send_data.contracts = contracts;

                return Content(JsonConvert.SerializeObject(data_response), "application/json");
            }
            catch
            {
                var answ = new response_api();
                answ.request_Info = new request_info();
                answ.request_Info.code = "403";
                answ.request_Info.answer = "BadInfo";

                return Content(JsonConvert.SerializeObject(answ), "application/json");
            }
        }
    }
}
        