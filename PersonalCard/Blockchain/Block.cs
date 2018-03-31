namespace PersonalCard.Blockchain
{
    public class Block
    {
        public int blockID { get; set; }
        public int index { get; set; }
        public string previousHash { get; set; }
        public string timestamp { get; set; }
        public string data { get; set; }
        public string hash { get; set; }
        public string wallet_hash { get; set;}

        public Block() { }

        public Block(int index, string previousHash, string timestamp, string data, string hash)
        {
            this.index = index;
            this.previousHash = previousHash;
            this.timestamp = timestamp;
            this.data = data;
            this.hash = hash;
        }
    }
}
