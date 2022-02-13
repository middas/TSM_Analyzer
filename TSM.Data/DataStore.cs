using Microsoft.EntityFrameworkCore;
using TSM.Core.LocalStorage;
using TSM.Data.Models;
using core = TSM.Core.Models;

namespace TSM.Data
{
    public class DataStore : IDataStore
    {
        private readonly SqlLiteDbContext dbContext;

        public DataStore(SqlLiteDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Configure()
        {
            dbContext.Database.Migrate();
        }

        public async Task<IEnumerable<core.AuctionBuyModel>> GetAuctionBuyModels()
        {
            var storeCharacterBuys = await dbContext.CharacterBuys.ToArrayAsync();

            return storeCharacterBuys.Select(x => new core.AuctionBuyModel
            {
                ItemId = x.ItemID,
                Price = x.Copper,
                Player = x.Character.Name,
                Quantity = x.Quantity,
                Source = x.Source,
                StackSize = x.StackSize,
                TimeEpoch = new DateTimeOffset(x.BoughtTime.Ticks, TimeSpan.Zero).ToUnixTimeSeconds()
            }).ToArray();
        }

        public async Task<string[]> GetBackupsScanned()
        {
            return await dbContext.ScannedBackups.Select(b => b.BackupPath).ToArrayAsync();
        }

        public async Task<IEnumerable<core.CancelledAuctionModel>> GetCancelledAuctionModels()
        {
            return await dbContext.CharacterCancelledAuctions.Select(x => new core.CancelledAuctionModel
            {
                ItemId = x.ItemID,
                PlayerName = x.Character.Name,
                Quantity = x.Quantity,
                StackSize = x.StackSize,
                TimeEpoch = new DateTimeOffset(x.CancelledTime, TimeSpan.Zero).ToUnixTimeSeconds()
            }).ToArrayAsync();
        }

        public async Task<IEnumerable<core.CharacterSaleModel>> GetCharacterSaleModels()
        {
            var storeSales = await dbContext.CharacterAuctionSales.ToArrayAsync();

            return storeSales.Select(x => new core.CharacterSaleModel
            {
                Character = x.Character.Name,
                Quantity = x.Quantity,
                ItemID = x.ItemID,
                Price = x.Copper,
                Time = new DateTimeOffset(x.TimeOfSale.Ticks, TimeSpan.Zero).ToUnixTimeSeconds(),
                Source = x.Source,
                StackSize = x.StackSize,
            });
        }

        public async Task<IEnumerable<core.Character>> GetCharactersData()
        {
            var storeCharacters = await dbContext.Characters.Include(c => c.CharacterMailItems).Include(c => c.CharacterBankItems)
                .Include(c => c.CharacterReagents).Include(c => c.CharacterInventoryItems).ToArrayAsync();

            return storeCharacters.Select(c => new core.Character(c.Name, Enum.Parse<core.Faction>(c.Faction), c.Realm)
            {
                BagItems = c.CharacterInventoryItems.ToDictionary(x => x.ItemID, x => x.Quantity),
                BankItems = c.CharacterBankItems.ToDictionary(x => x.ItemID, x => x.Quantity),
                Class = c.Class,
                MailItems = c.CharacterMailItems.ToDictionary(x => x.ItemID, x => x.Count),
                Money = new core.Money(c.Copper),
                ReagentItems = c.CharacterReagents.ToDictionary(x => x.ItemID, x => x.Quantity)
            }).ToArray();
        }

        public async Task<IEnumerable<core.ExpiredAuctionModel>> GetExpiredAuctionModels()
        {
            return await dbContext.CharacterExpiredAuctions.Select(x => new core.ExpiredAuctionModel
            {
                ItemId = x.ItemID,
                Player = x.Character.Name,
                Quantity = x.Quantity,
                StackSize = x.StackSize,
                TimeEpoch = new DateTimeOffset(x.ExpiredTime, TimeSpan.Zero).ToUnixTimeSeconds()
            }).ToArrayAsync();
        }

        public Task<Dictionary<string, string>> GetItems()
        {
            return dbContext.Items.ToDictionaryAsync(x => x.ItemID, x => x.Name);
        }

        public async Task StoreAuctionBuys(IEnumerable<core.AuctionBuyModel> auctionBuyModels)
        {
            Character[] characters = dbContext.Characters.ToArray();
            var characterBuys = dbContext.CharacterBuys.AsEnumerable().Where(x =>
                x.BoughtTime >= auctionBuyModels.Min(y => y.Time.UtcDateTime) && x.BoughtTime <= auctionBuyModels.Max(y => y.Time.UtcDateTime)).ToArray();

            foreach (var auctionBuyModel in auctionBuyModels)
            {
                CharacterBuy storeCharacterBuy = characterBuys.FirstOrDefault(x => x.ItemID == auctionBuyModel.ItemId
                    && x.BoughtTime == auctionBuyModel.Time.UtcDateTime && x.Source == auctionBuyModel.Source && x.Quantity == auctionBuyModel.Quantity);

                if (storeCharacterBuy == null)
                {
                    storeCharacterBuy = new()
                    {
                        BoughtTime = auctionBuyModel.Time.UtcDateTime,
                        Character = characters.Single(c => c.Name == auctionBuyModel.Player),
                        Copper = auctionBuyModel.Money.TotalCopper,
                        ItemID = auctionBuyModel.ItemId,
                        Quantity = auctionBuyModel.Quantity,
                        Source = auctionBuyModel.Source,
                        StackSize = auctionBuyModel.StackSize
                    };

                    await dbContext.AddAsync(storeCharacterBuy);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task StoreBackupScanned(FileInfo backupFile, DateTimeOffset startTime)
        {
            await dbContext.ScannedBackups.AddAsync(new ScannedBackup
            {
                BackupPath = backupFile.FullName,
                ScannedTime = startTime.UtcDateTime,
                Duration = DateTimeOffset.Now.Subtract(startTime).TotalSeconds
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task StoreCancelledAuctions(IEnumerable<core.CancelledAuctionModel> cancelledAuctionModels)
        {
            if (cancelledAuctionModels != null && cancelledAuctionModels.Any())
            {
                Character[] characters = dbContext.Characters.ToArray();
                var characterCancelledAuctions = dbContext.CharacterCancelledAuctions.AsEnumerable().Where(x =>
                    x.CancelledTime >= cancelledAuctionModels.Min(y => y.Time.UtcDateTime) && x.CancelledTime <= cancelledAuctionModels.Max(y => y.Time.UtcDateTime)).ToArray();

                foreach (var cancelledAuctionModel in cancelledAuctionModels)
                {
                    CharacterCancelledAuction storeCharacterCancelledAuction = characterCancelledAuctions.FirstOrDefault(x => x.ItemID == cancelledAuctionModel.ItemId
                        && x.CancelledTime == cancelledAuctionModel.Time.UtcDateTime);

                    if (storeCharacterCancelledAuction == null)
                    {
                        storeCharacterCancelledAuction = new()
                        {
                            CancelledTime = cancelledAuctionModel.Time.UtcDateTime,
                            Character = characters.Single(c => c.Name == cancelledAuctionModel.PlayerName),
                            ItemID = cancelledAuctionModel.ItemId,
                            Quantity = cancelledAuctionModel.Quantity,
                            StackSize = cancelledAuctionModel.StackSize
                        };

                        await dbContext.AddAsync(storeCharacterCancelledAuction);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task StoreCharacters(IEnumerable<core.Character> characters)
        {
            foreach (var character in characters)
            {
                Character storeCharacter = await dbContext.Characters.Include(c => c.CharacterReagents).Include(c => c.CharacterBankItems)
                    .Include(c => c.CharacterInventoryItems).Include(c => c.CharacterMailItems).SingleOrDefaultAsync(c => c.Name == character.Name
                        && c.Faction == character.Faction.ToString() && c.Realm == character.Realm);

                if (storeCharacter == null)
                {
                    storeCharacter = new()
                    {
                        Class = character.Class,
                        Name = character.Name,
                        Faction = character.Faction.ToString(),
                        Realm = character.Realm,
                        CharacterBankItems = new List<CharacterBank>(),
                        CharacterInventoryItems = new List<CharacterInventory>(),
                        CharacterReagents = new List<CharacterReagent>(),
                        CharacterMailItems = new List<CharacterMailItem>()
                    };

                    await dbContext.AddAsync(storeCharacter);
                }

                storeCharacter.Copper = character.Money.TotalCopper;
                SetCharacterBank(storeCharacter.CharacterBankItems, character.BankItems);
                SetCharacterReagents(storeCharacter.CharacterReagents, character.ReagentItems);
                SetCharacterInventory(storeCharacter.CharacterInventoryItems, character.BagItems);
                SetCharacterMail(storeCharacter.CharacterMailItems, character.MailItems);
                storeCharacter.LastUpdateTime = character.GoldLogLastUpdate.UtcDateTime;
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task StoreCharacterSales(IEnumerable<core.CharacterSaleModel> characterSaleModels)
        {
            Character[] characters = dbContext.Characters.ToArray();
            var characterAuctionSales = dbContext.CharacterAuctionSales.AsEnumerable().Where(x => x.TimeOfSale >= characterSaleModels.Min(y => y.TimeOfSale.UtcDateTime)
                && x.TimeOfSale <= characterSaleModels.Max(y => y.TimeOfSale.UtcDateTime)).ToArray();

            foreach (var characterSaleModel in characterSaleModels)
            {
                CharacterAuctionSale storeCharacterAuctionSale = characterAuctionSales.FirstOrDefault(x => x.ItemID == characterSaleModel.ItemID
                    && x.TimeOfSale == characterSaleModel.TimeOfSale.UtcDateTime && x.Copper == characterSaleModel.Money.TotalCopper && x.Quantity == characterSaleModel.Quantity
                    && x.StackSize == characterSaleModel.StackSize);

                if (storeCharacterAuctionSale == null)
                {
                    storeCharacterAuctionSale = new()
                    {
                        Character = characters.Single(c => c.Name == characterSaleModel.Character),
                        Copper = characterSaleModel.Money.TotalCopper,
                        ItemID = characterSaleModel.ItemID,
                        Quantity = characterSaleModel.Quantity,
                        TimeOfSale = characterSaleModel.TimeOfSale.UtcDateTime,
                        Source = characterSaleModel.Source,
                        StackSize = characterSaleModel.StackSize
                    };

                    await dbContext.AddAsync(storeCharacterAuctionSale);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task StoreExpiredAuctions(IEnumerable<core.ExpiredAuctionModel> expiredAuctionModels)
        {
            if (expiredAuctionModels != null && expiredAuctionModels.Any())
            {
                Character[] characters = dbContext.Characters.ToArray();
                var characterExpiredAuctions = dbContext.CharacterExpiredAuctions.AsEnumerable().Where(x =>
                    x.ExpiredTime >= expiredAuctionModels.Min(y => y.Time.UtcDateTime) && x.ExpiredTime <= expiredAuctionModels.Max(y => y.Time.UtcDateTime)).ToArray();

                foreach (var expiredAuctionModel in expiredAuctionModels)
                {
                    CharacterExpiredAuction storeCharacterExpiredAuction = characterExpiredAuctions.FirstOrDefault(x => x.ItemID == expiredAuctionModel.ItemId
                        && x.ExpiredTime == expiredAuctionModel.Time.UtcDateTime);

                    if (storeCharacterExpiredAuction == null)
                    {
                        storeCharacterExpiredAuction = new()
                        {
                            Character = characters.Single(c => c.Name == expiredAuctionModel.Player),
                            ExpiredTime = expiredAuctionModel.Time.UtcDateTime,
                            ItemID = expiredAuctionModel.ItemId,
                            Quantity = expiredAuctionModel.Quantity,
                            StackSize = expiredAuctionModel.StackSize
                        };

                        await dbContext.AddAsync(storeCharacterExpiredAuction);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task StoreItemNames(IDictionary<string, string> items)
        {
            var storeItems = dbContext.Items.Select(x => x.ItemID).ToArray();

            foreach (var item in items.Where(x => !storeItems.Contains(x.Key)))
            {
                await dbContext.Items.AddAsync(new Item
                {
                    ItemID = item.Key,
                    Name = item.Value
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private void SetCharacterBank(ICollection<CharacterBank> characterBankItems, Dictionary<string, int> bankItems)
        {
            characterBankItems.Clear();

            foreach (var kvp in bankItems)
            {
                characterBankItems.Add(new CharacterBank
                {
                    ItemID = kvp.Key,
                    Quantity = kvp.Value
                });
            }
        }

        private void SetCharacterInventory(ICollection<CharacterInventory> characterInventoryItems, Dictionary<string, int> bagItems)
        {
            characterInventoryItems.Clear();

            foreach (var kvp in bagItems)
            {
                characterInventoryItems.Add(new CharacterInventory
                {
                    ItemID = kvp.Key,
                    Quantity = kvp.Value
                });
            }
        }

        private void SetCharacterMail(ICollection<CharacterMailItem> characterMailItems, Dictionary<string, int> mailItems)
        {
            characterMailItems.Clear();

            foreach (var mailItem in mailItems)
            {
                characterMailItems.Add(new CharacterMailItem
                {
                    ItemID = mailItem.Key,
                    Count = mailItem.Value
                });
            }
        }

        private void SetCharacterReagents(ICollection<CharacterReagent> characterReagents, Dictionary<string, int> reagentItems)
        {
            characterReagents.Clear();

            foreach (var kvp in reagentItems)
            {
                characterReagents.Add(new CharacterReagent
                {
                    ItemID = kvp.Key,
                    Quantity = kvp.Value
                });
            }
        }
    }
}