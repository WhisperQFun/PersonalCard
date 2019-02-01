namespace PersonalCard.Blockchain
{
    public class Access
    {
        public int Id { get; set; }

        public string hash { get; set; }
        public string user { get; set; }
        public string timestamp { get; set; }

        public bool is_archive { get; set; }
    }
}
