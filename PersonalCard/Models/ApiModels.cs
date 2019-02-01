using System.Collections.Generic;
using PersonalCard.Blockchain;

namespace PersonalCard.Models
{
    public class request_info
    {
        public string code;
        public string answer;
    }

    public class send_data
    {
        public List<Medical> medicals;
        public List<Contract> contracts;
        public User user;
    }

    public class response_api
    {
        public request_info request_Info;
        public send_data send_data;
    }
}
