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
    public class request_data
    {
        public string key;
        public string target;
        public string email;
        public string password;
    }

    public class request_info
    {
        public string code;
        public string answer;
    }

    public class send_data
    {
        public List<Medical> medicals;
    }

    public class response_api
    {
        public request_info request_Info;
        public send_data send_data;


    }



    public class ApiController : Controller
    {
        private mysqlContext _context;
        BlockchainService blockchainService;
        
        // key api : ee85d34b-8443-4c8d-9369-0cfb04c2d79d
        // GET: Api
        // example string http://www.example.com/Api?key=ee85d34b-8443-4c8d-9369-0cfb04c2d79d&target=authorization&email=1@gmail.com&password=1

        public ApiController(mysqlContext context, BlockchainService service)
        {
            blockchainService = service;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(string site, string organisation)
        {
            User user = null;
            API api = null;
            user = await _context.User.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);



            api = new API
            {
                is_active = true,
                site = site,
                organisation = organisation,
                timestamp = DateTime.Now.ToString(),
                token = await ShaEncoder.GenerateSHA256String(site + organisation + DateTime.Now.ToString())
            };
            await _context.Api.AddAsync(api);
            await _context.SaveChangesAsync();
            return Content("Token: " + api.token);

            
        }

        public async Task<IActionResult> login([FromQuery]string token, [FromQuery]string login, [FromQuery]string password)
        {
            User user = null;
            if (await _context.Api.FirstOrDefaultAsync(u => u.token == token && u.is_active == true) != null)
            {
                user = await _context.User.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
                if (user != null)
                {


                    // поиск пользователя в бд








                    if (user != null)
                    {
                        try
                        {
                            var data_response = new response_api();
                            data_response.request_Info = new request_info();
                            data_response.request_Info.answer = "OK";
                            data_response.request_Info.code = "200";
                            data_response.send_data = new send_data();
                            List<Block> blocks = _context.Block.Where(u=> u.wallet_hash == user.Hash).ToList();
                            
                            List<Medical> medicals = new List<Medical>();
                            foreach (var bloks in blocks)
                            {
                                medicals.Add(JsonConvert.DeserializeObject<Medical>(bloks.data));

                            }
                            data_response.send_data.medicals = medicals;
                            var dt_resp = JsonConvert.SerializeObject(data_response);
                            //var data_response2 = JsonConvert.DeserializeObject<response_api>(dt_resp);



                            return Content(dt_resp, "application/json");
                        }
                        catch
                        {
                            var answ = new response_api();
                            answ.request_Info = new request_info();
                            answ.request_Info.code = "403";
                            answ.request_Info.answer = "BadInfo";
                            var dt_resp = JsonConvert.SerializeObject(answ);
                            return Content(dt_resp, "application/json");

                        }


                    }
                    else
                    {
                        var answ = new response_api();
                        answ.request_Info = new request_info();
                        answ.request_Info.code = "403";
                        answ.request_Info.answer = "BadInfo";
                        var dt_resp = JsonConvert.SerializeObject(answ);
                        return Content(dt_resp, "application/json");

                    }

                }
                else
                {
                    var answ = new response_api();
                    answ.request_Info = new request_info();
                    answ.request_Info.code = "400";
                    answ.request_Info.answer = "Error";
                    var dt_resp = JsonConvert.SerializeObject(answ);
                    return Content(dt_resp, "application/json");
                }


            }
            else
            {
                var answ = new response_api();
                answ.request_Info = new request_info();
                answ.request_Info.code = "400";
                answ.request_Info.answer = "Error";
                var dt_resp = JsonConvert.SerializeObject(answ);
                return Content(dt_resp, "application/json");
            }

        }
    }
}
        