using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PersonalCard.Blockchain;
using PersonalCard.Context;
using PersonalCard.Encrypt;
using PersonalCard.Models;

namespace PersonalCard.Services
{
    public class BlockchainService
    {
        public static string REGION = "MSK";
        public static List<Nodes> NODES = new List<Nodes>();

        private readonly IMemoryCache _cache;
        private readonly MySQLContext _context;

        public BlockchainService(IMemoryCache memoryCache, MySQLContext context)
        {
            _context = context;
            _cache = memoryCache;
        }

        public async void Initialize()
        {
            if (!_context.Block.Any())
            {
                await _context.Block.AddAsync(getGenesisBlock());
                await _context.SaveChangesAsync();
            }

            if (!_context.Roles.Any())
            {
                await _context.Roles.AddRangeAsync(
                    new Role
                    {
                        RoleId = 1,
                        Name = "User"
                    },
                    new Role
                    {
                        RoleId = 2,
                        Name = "Doctor"
                    },
                    new Role
                    {
                        RoleId = 3,
                        Name = "Admin"
                    });

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Block>> GetBlocks() => await _context.Block.ToListAsync();

        public async Task AddBlockAsync(Block block)
        {
            await _context.Block.AddAsync(block);

            int n = await _context.SaveChangesAsync();
            if (n > 0)
                _cache.Set(block.index, block);
        }

        public async Task<Block> GetBlockAsync(int id)
        {
            Block block;

            if (!_cache.TryGetValue(id, out block))
            {
                block = await _context.Block.FirstOrDefaultAsync(p => p.blockID == id);
                if (block != null)
                {
                    _cache.Set(block.blockID, block);
                }
            }

            return block;
        }

        public async Task<Block> generateNextBlockAsync(string blockData)
        {
            var previousBlock = await getLatestBlockAsync();
            var nextIndex = previousBlock.index + 1;
            var nextTimestamp = DateTime.Now.ToString();
            var nextHash = await CalculateHashAsync(nextIndex, previousBlock.hash, nextTimestamp, blockData);

            return new Block(nextIndex, previousBlock.hash, nextTimestamp, blockData, nextHash, REGION);
        }

        public async Task<Block> generateNextBlockAsync(string blockData, string wallet_hash)
        {
            var previousBlock = await getLatestBlockAsync();
            var nextIndex = previousBlock.index + 1;
            var nextTimestamp = DateTime.Now.ToString();
            var nextHash = await CalculateHashAsync(nextIndex, previousBlock.hash, nextTimestamp, blockData);

            return new Block(nextIndex, previousBlock.hash, nextTimestamp, blockData, nextHash, REGION, wallet_hash);
        }

        public async Task<Block> generateNextBlockAsync(string blockData, string wallet_hash, string destination_wallet)
        {
            var previousBlock = await getLatestBlockAsync();
            var nextIndex = previousBlock.index + 1;
            var nextTimestamp = DateTime.Now.ToString();
            var nextHash = await CalculateHashAsync(nextIndex, previousBlock.hash, nextTimestamp, blockData);

            return new Block(nextIndex, previousBlock.hash, nextTimestamp, blockData, nextHash,
                REGION, wallet_hash, destination_wallet);
        }

        public async Task<Block> getLatestBlockAsync() => await _context.Block.LastOrDefaultAsync();

        /// <summary>
        /// Calculate block hash
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public async Task<string> calculateHashForBlockAsync(Block block) =>
            await CalculateHashAsync(block.index, block.previousHash, block.timestamp, block.data);

        /// <summary>
        /// Calculate hash
        /// </summary>
        /// <param name="index"></param>
        /// <param name="previousHash"></param>
        /// <param name="timestamp"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> CalculateHashAsync(int index, string previousHash, string timestamp, string data) =>
            await ShaEncoder.GenerateSHA256String(index.ToString() + previousHash + timestamp + data);

        public static Block getGenesisBlock() =>
            new Block(1, "0", DateTime.Now.ToString(),
            "Genesis block",
            "7d72de6d_context76a37cb1b3feab15c8e3_contextc9af9b4fdc68359ede298491e0dce941",
            REGION);
    }
}
