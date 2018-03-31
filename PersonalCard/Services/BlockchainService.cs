using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalCard.Blockchain;
using PersonalCard.Context;
using PersonalCard.Encrypt;
using PersonalCard.Models;
namespace PersonalCard.Services
{



    public class BlockchainService
        {

            private IMemoryCache cache;
            private mysqlContext db;
            public BlockchainService(IMemoryCache memoryCache, mysqlContext context)
            {
                db = context;
                cache = memoryCache;
            }

            public async void Initialize()
            {
                if (!db.Block.Any())
                {
                    db.Block.Add(getGenesisBlock());
                    db.SaveChanges();
                }


            }

            public async Task<List<Block>> GetBlocks()
            {
                List<Block> lsblc;

                lsblc = await db.Block.ToListAsync();

                return lsblc;
            }

            public async void AddBlockAsync(Block block)
            {

                await db.Block.AddAsync(block);
                int n = await db.SaveChangesAsync();
                if (n > 0)
                {
                    cache.Set(block.index, block);
                }


            }

            public async Task<Block> GetBlockAsync(int id)
            {
                Block block;

                if (!cache.TryGetValue(id, out block))
                {
                    block = await db.Block.FirstOrDefaultAsync(p => p.blockID == id);
                    if (block != null)
                    {
                        cache.Set(block.blockID, block);
                    }
                }

                return block;
            }

            public async Task<Block> generateNextBlockAsync(string blockData)
            {
                var previousBlock = await getLatestBlockAsync();
                var nextIndex = previousBlock.index + 1;
                var nextTimestamp = DateTime.Now.ToString();
                var nextHash = await calculateHashAsync(nextIndex, previousBlock.hash, nextTimestamp, blockData);
                return new Block(nextIndex, previousBlock.hash, nextTimestamp, blockData, nextHash);
            }

            public async Task<Block> getLatestBlockAsync()
            {
                Block last_block;
                int max;

                //last_block = db.Block.Max();
                //last_block= await db.Block.FirstOrDefaultAsync(p => p.blockID == max);
                //last_block = await db.Block.FirstOrDefaultAsync(p => p.blockID == max);
                last_block = await db.Block.LastOrDefaultAsync();
                int er = 1;




                return last_block;
            }

            /// <summary>
            /// Просчет хеша для блока
            /// </summary>
            /// <param name="block"></param>
            /// <returns></returns>
            public async Task<string> calculateHashForBlockAsync(Block block)
            {
                return await calculateHashAsync(block.index, block.previousHash, block.timestamp, block.data);
            }
            /// <summary>
            /// Просчет хеша
            /// </summary>
            /// <param name="index"></param>
            /// <param name="previousHash"></param>
            /// <param name="timestamp"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public async Task<string> calculateHashAsync(int index, string previousHash, string timestamp, string data)
            {
                return await ShaEncoder.GenerateSHA256String(index.ToString() + previousHash + timestamp + data);
            }
            public static Block getGenesisBlock()
            {
                return new Block(1, "0", DateTime.Now.ToString(), "Genesis block", "7d72de6ddb76a37cb1b3feab15c8e3dbc9af9b4fdc68359ede298491e0dce941");
            }
        }
    }
    /*
        internal int index { get; set; }
            internal string previousHash { get; set; }
            internal DateTime timestamp { get; set; }
            internal string data { get; set;}
            internal string hash { get; set; }
    */

