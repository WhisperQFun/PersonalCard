/*
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalCard.Models;
using Microsoft.AspNetCore.Mvc;
using PersonalCard.Blockchain;

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
        public string first_name;
        public string last_name;
        public string middle_name;
        public string UserGender;
        public string about_me;
        public string tags;
        public int rating;
        public string birthday_date;
        public string place_of_work;
    }

    public class response_api
    {
        public request_info request_Info;
        public send_data send_data;
       
        
    }

   

    public class ApiController : Controller
    {
        List<Medical> Ls = new List<Medical>();
        // key api : ee85d34b-8443-4c8d-9369-0cfb04c2d79d
        // GET: Api
        // example string http://www.example.com/Api?key=ee85d34b-8443-4c8d-9369-0cfb04c2d79d&target=authorization&email=1@gmail.com&password=1
        public ActionResult Index()
        {
            if (this.Request.QueryString != null && Request.QueryString["target"] != null && this.Request.QueryString["email"] != null && this.Request.QueryString["password"] != null)
            {
                if (this.Request.QueryString["key"] == "ee85d34b-8443-4c8d-9369-0cfb04c2d79d")
                {
                    var rg = new request_data();
                    rg.key = this.Request.QueryString["key"];
                    rg.target = this.Request.QueryString["target"];
                    rg.email = this.Request.QueryString["email"];
                    rg.password = this.Request.QueryString["password"];

                    // поиск пользователя в бд
                    User user = null;
                    Resume resume = null;
                    using (UserContext db = new UserContext())
                    {
                        user = db.Users.FirstOrDefault(u => u.email == rg.email && u.password == rg.password);
                        try
                        {
                            resume = db.Resume.FirstOrDefault(u => u.user_id == user.id);
                        }
                        catch
                        {

                        }
                        
                        
                        
                    }
                    if (user != null)
                    {
                        try
                        {
                            Ls.Add(new Places_of_work());
                            Ls.Add(new Places_of_work());
                            Ls.Add(new Places_of_work());
                            Ls[0].id = 1;
                            Ls[0].name = "First place";
                            Ls[0].position = "Programmer";
                            Ls[0].description = "вырпвфырфкпецепцкеп";
                            Ls[0].date_begin = "12.12.2017";
                            Ls[0].date_end = "12.12.2018";
                            Ls[1].id = 2;
                            Ls[1].name = "First place";
                            Ls[1].position = "Programmer";
                            Ls[1].description = "вырпвфырфкпецепцкеп";
                            Ls[1].date_begin = "12.12.2017";
                            Ls[1].date_end = "12.12.2018";
                            Ls[2].id = 3;
                            Ls[2].name = "First place";
                            Ls[2].position = "Programmer";
                            Ls[2].description = "вырпвфырфкпецепцкеп";
                            Ls[2].date_begin = "12.12.2017";
                            Ls[2].date_end = "12.12.2018";

                            var data_response = new response_api();
                            data_response.request_Info = new request_info();
                            data_response.request_Info.answer = "OK";
                            data_response.request_Info.code = "200";
                            data_response.send_data = new send_data();
                            data_response.send_data.first_name = user.first_name;
                            data_response.send_data.last_name = user.last_name;
                            data_response.send_data.middle_name = user.middle_name;
                            data_response.send_data.UserGender = user.UserGender.ToString();
                            data_response.send_data.about_me = user.about_me;
                            data_response.send_data.tags = user.tags;
                            data_response.send_data.rating = user.rating;
                            data_response.send_data.birthday_date = user.birthday_date;
                            data_response.Resume = resume;
                            data_response.Places_of_work = Ls;


                            var dt_resp = JsonConvert.SerializeObject(data_response);
                            var data_response2 = JsonConvert.DeserializeObject<response_api>(dt_resp);



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
        public ActionResult Game()
        {
            Game games_1 = new Game();
            Game games = new Game();
            GameModel model = new GameModel();
            if (this.Request.QueryString != null && this.Request.QueryString["category"] != null && this.Request.QueryString["answer"] != null && this.Request.QueryString["id_game"] != null && this.Request.QueryString["score"] != null)
            {
                if (this.Request.QueryString["key"] == "ee85d34b-8443-4c8d-9369-0cfb04c2d79d")
                {
                    using (UserContext db = new UserContext())
                    {
                        var str = this.Request.QueryString["id_game"];
                        games_1.id = Convert.ToInt32(str);
                        games_1.type = this.Request.QueryString["category"];
                        str = this.Request.QueryString["score"]; 
                        games_1.score = Convert.ToInt32(str);

                        try
                        {
                            games = db.Game.FirstOrDefault(u => u.type == games_1.type && u.id == games_1.id);
                        }
                        catch
                        {

                        }



                    }
                    if (games != null)
                    {
                       
                            

                            var data_response = new game_response();
                            data_response.game = new Game();
                            data_response.request_Info = new request_info();
                            data_response.request_Info.answer = "OK";
                            data_response.request_Info.code = "200";
                            data_response.game.id = games.id;
                            data_response.game.text = games.text;
                            data_response.game.chose_1 = games.chose_1;
                            data_response.game.chose_2 = games.chose_2;
                            data_response.game.item_1 = games.item_1;
                            data_response.game.item_2 = games.item_2;
                            data_response.game.right_answer = games.right_answer;
                            data_response.game.score = games.score;
                            data_response.game.type = games.type;


                            var dt_resp = JsonConvert.SerializeObject(data_response);
                            



                            return Content(dt_resp, "application/json");
                        
                            

                        


                    }
                    else
                    {
                        var answ = new game_response();
                        answ.request_Info = new request_info();
                        answ.request_Info.code = "403";
                        answ.request_Info.answer = "BadInfo";
                        var dt_resp = JsonConvert.SerializeObject(answ);
                        return Content(dt_resp, "application/json");

                    }

                }
                else
                {
                    var answ = new game_response();
                    answ.request_Info = new request_info();
                    answ.request_Info.code = "400";
                    answ.request_Info.answer = "Error";
                    var dt_resp = JsonConvert.SerializeObject(answ);
                    return Content(dt_resp, "application/json");
                }


            }
            else
            {
                var answ = new game_response();
                answ.request_Info = new request_info();
                answ.request_Info.code = "400";
                answ.request_Info.answer = "Error";
                var dt_resp = JsonConvert.SerializeObject(answ);
                return Content(dt_resp, "application/json");
            }

        }
    }
}
*/