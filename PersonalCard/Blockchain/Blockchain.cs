/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PersonalCard.Blockchain
{
    class Blockchain
    {
        List<Block> blockchain = new List<Block>();
        /// <summary>
        /// Конструктор и инициализатор Blockchain/Construct and initializate Blockchain
        /// </summary>
        internal Blockchain()
        {
            blockchain.Add(getGenesisBlock());
            Data message = new Data(blockchain[blockchain.Count - 1], "Genesis Block");
        }
        /// <summary>
        /// Изначальный блок/Genesis Block
        /// </summary>
        /// <returns></returns>
        public static Block getGenesisBlock()
        {
            return new Block(0, "0", DateTime.Now, "Genesis block", "7d72de6ddb76a37cb1b3feab15c8e3dbc9af9b4fdc68359ede298491e0dce941");
        }

        Block getLatestBlock() { return blockchain[blockchain.Count - 1]; }
        /// <summary>
        /// Веб интерфейс/Create web interface
        /// </summary>
        void HttpServer()
        {


        }
        /// <summary>
        /// Создание точки подключения/Create connect point
        /// </summary>
        void P2PServer()
        {

        }
        /// <summary>
        /// Подключение к другому узлу/Connect to the peer
        /// </summary>
        void Connection()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void MessageHandler(string msg)
        {


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        void ErrorHandler(int code, string msg)
        {

        }
        /// <summary>
        /// Проверка цепи блоков
        /// </summary>
        /// <param name="blockchainToValidate"></param>
        /// <returns></returns>
        bool isValidChain(List<Block> blockchainToValidate)
        {
            if (blockchainToValidate[0] != getGenesisBlock())
            {
                return false;
            }
            var tempBlocks = blockchainToValidate;
            for (var i = 1; i < blockchainToValidate.Count; i++)
            {
                if (isValidNewBlock(blockchainToValidate[i], tempBlocks[i - 1]))
                {
                    tempBlocks.Add(blockchainToValidate[i]);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Генерирует следующий блок
        /// </summary>
        /// <param name="blockData"></param>
        /// <returns></returns>
        public static Block generateNextBlock(string blockData)
        {
            var previousBlock = getLatestBlock();
            var nextIndex = previousBlock.index + 1;
            var nextTimestamp = DateTime.Now;
            var nextHash = calculateHash(nextIndex, previousBlock.hash, nextTimestamp, blockData);
            return new Block(nextIndex, previousBlock.hash, nextTimestamp, blockData, nextHash);
        }
        /// <summary>
        /// Добавление в цепь блока
        /// </summary>
        /// <param name="newBlock"></param>
        public static void addBlock(Block newBlock)
        {
            if (isValidNewBlock(newBlock, getLatestBlock()))
            {
                blockchain.Add(newBlock);
            }
        }
        /// <summary>
        /// Замена цепи
        /// </summary>
        /// <param name="newBlocks"></param>
        void repalceChain(List<Block> newBlocks)
        {
            if (isValidChain(newBlocks) && newBlocks.Count > blockchain.Count)
            {
                MessageHandler("Received blockchain is valid. Replacing current blockchain with received blockchain");
                blockchain = newBlocks;
            }
            else
            {
                MessageHandler("Received blockchain invalid");
            }
        }
        /// <summary>
        /// Загрузка цепочки из бекапа
        /// </summary>
        void loadChain()
        {
            using (StreamReader file = File.OpenText(@"blockchain.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<Block> Blocks = (List<Block>)serializer.Deserialize(file, typeof(List<Block>));
            }
        }
        /// <summary>
        /// Сохранение цепочки
        /// </summary>
        void saveChain()
        {
            List<Block> save_blocks = blockchain;
            //var tmp = JsonConvert.SerializeObject(save_blocks);

            using (StreamWriter file = File.CreateText(@"blockchain.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, save_blocks);
            }
        }
        /// <summary>
        /// Валидация блока
        /// </summary>
        /// <param name="newBlock"></param>
        /// <param name="previousBlock"></param>
        /// <returns></returns>
        bool isValidNewBlock(Block newBlock, Block previousBlock)
        {
            if (previousBlock.index + 1 != newBlock.index)
            {
                ErrorHandler(1, "invalid index");
                return false;
            }
            else
            {
                if (previousBlock.hash != newBlock.previousHash)
                {
                    ErrorHandler(2, "invalid previoushash");
                    return false;
                }
                else
                {
                    if (calculateHashForBlock(newBlock) != newBlock.hash)
                    {
                        ErrorHandler(3, "invalid hash: " + calculateHashForBlock(newBlock) + " " + newBlock.hash);
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Просчет хеша для блока
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        string calculateHashForBlock(Block block)
        {
            return calculateHash(block.index, block.previousHash, block.timestamp, block.data);
        }
        /// <summary>
        /// Просчет хеша
        /// </summary>
        /// <param name="index"></param>
        /// <param name="previousHash"></param>
        /// <param name="timestamp"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string calculateHash(int index, string previousHash, DateTime timestamp, string data)
        {
            return ShaEncoder.GenerateSHA256String(index.ToString() + previousHash + timestamp.ToString() + data);
        }
    }
}
*/