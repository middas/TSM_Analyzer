using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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

            return storeCharacters.Select(c => (core.Character)c).ToArray();
        }

        public async Task<IEnumerable<core.ExpiredAuctionModel>> GetExpiredAuctionModels()
        {
            if (await dbContext.CharacterExpiredAuctions.AnyAsync(x => x.Hash == 0))
            {
                var dbModels = dbContext.CharacterExpiredAuctions.Where(x => x.Hash == 0);

                foreach (var m in dbModels)
                {
                    var tempModel = new core.ExpiredAuctionModel
                    {
                        ItemId = m.ItemID,
                        Player = m.Character.Name,
                        Quantity = m.Quantity,
                        StackSize = m.StackSize,
                        TimeEpoch = new DateTimeOffset(m.ExpiredTime, TimeSpan.Zero).ToUnixTimeSeconds()
                    };

                    m.Hash = tempModel.GetHashCode();
                }

                await dbContext.SaveChangesAsync();
            }

            var models = await dbContext.CharacterExpiredAuctions.Select(x => new core.ExpiredAuctionModel
            {
                ItemId = x.ItemID,
                Player = x.Character.Name,
                Quantity = x.Quantity,
                StackSize = x.StackSize,
                TimeEpoch = new DateTimeOffset(x.ExpiredTime, TimeSpan.Zero).ToUnixTimeSeconds(),
                Hash = x.Hash
            }).ToArrayAsync();

            return models;
        }

        public Task<Dictionary<string, string>> GetItems()
        {
            return dbContext.Items.ToDictionaryAsync(x => x.ItemID, x => x.Name);
        }

        public async Task StoreAuctionBuys(IEnumerable<core.AuctionBuyModel> auctionBuyModels)
        {
            if (auctionBuyModels == null || !auctionBuyModels.Any())
            {
                return;
            }

            Character[] characters = dbContext.Characters.ToArray();
            var minDate = await dbContext.CharacterBuys.Select(x => x.BoughtTime).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
            var characterBuys = dbContext.CharacterBuys.Where(x => x.BoughtTime >= minDate).ToArray();

            var characterGroupedAuctionBuyModels = auctionBuyModels.Where(x => x.Time >= minDate).GroupBy(x => x.Player);
            foreach (var characterGroupedAuctionBuyModel in characterGroupedAuctionBuyModels)
            {
                var character = characters.Single(c => c.Name == characterGroupedAuctionBuyModel.Key);

                foreach (var auctionBuyModel in characterGroupedAuctionBuyModel)
                {
                    if (!characterBuys.Any(x => x.ItemID == auctionBuyModel.ItemId
                     && x.BoughtTime == auctionBuyModel.Time.UtcDateTime && x.Source == auctionBuyModel.Source && x.Quantity == auctionBuyModel.Quantity))
                    {
                        CharacterBuy storeCharacterBuy = new()
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
            if (cancelledAuctionModels == null || !cancelledAuctionModels.Any())
            {
                return;
            }

            if (cancelledAuctionModels != null && cancelledAuctionModels.Any())
            {
                Character[] characters = dbContext.Characters.ToArray();
                var minDate = await dbContext.CharacterCancelledAuctions.Select(x => x.CancelledTime).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
                var characterCancelledAuctions = dbContext.CharacterCancelledAuctions.Where(x => x.CancelledTime >= minDate).ToArray();

                var characterGroupedCancelledAuctionModels = cancelledAuctionModels.Where(x => x.Time >= minDate).GroupBy(x => x.PlayerName);
                foreach (var characterGroupedCancelledAuctionModel in characterGroupedCancelledAuctionModels)
                {
                    var character = characters.Single(c => c.Name == characterGroupedCancelledAuctionModel.Key);

                    foreach (var cancelledAuctionModel in characterGroupedCancelledAuctionModel)
                    {
                        if (!characterCancelledAuctions.Any(x => x.ItemID == cancelledAuctionModel.ItemId
                         && x.CancelledTime == cancelledAuctionModel.Time.UtcDateTime))
                        {
                            CharacterCancelledAuction storeCharacterCancelledAuction = new()
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
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task StoreCharacters(IEnumerable<core.Character> characters)
        {
            if (characters == null || !characters.Any())
            {
                return;
            }

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
            if (characterSaleModels == null || !characterSaleModels.Any())
            {
                return;
            }

            Character[] characters = dbContext.Characters.ToArray();
            var minDate = await dbContext.CharacterAuctionSales.Select(x => x.TimeOfSale).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
            var characterAuctionSales = dbContext.CharacterAuctionSales.AsEnumerable().Where(x => x.TimeOfSale >= minDate).ToArray();

            var saleModelsByCharacter = characterSaleModels.Where(x => x.TimeOfSale >= minDate).GroupBy(x => x.Character);
            foreach (var groupedCharacterSaleModel in saleModelsByCharacter)
            {
                var character = characters.Single(c => c.Name == groupedCharacterSaleModel.Key);

                foreach (var characterSaleModel in groupedCharacterSaleModel)
                {
                    if (!characterAuctionSales.Any(x => x.ItemID == characterSaleModel.ItemID
                     && x.TimeOfSale == characterSaleModel.TimeOfSale.UtcDateTime && x.Copper == characterSaleModel.Money.TotalCopper && x.Quantity == characterSaleModel.Quantity
                     && x.StackSize == characterSaleModel.StackSize))
                    {
                        CharacterAuctionSale storeCharacterAuctionSale = new()
                        {
                            Character = character,
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
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task StoreExpiredAuctions(IEnumerable<core.ExpiredAuctionModel> expiredAuctionModels)
        {
            if (expiredAuctionModels == null || !expiredAuctionModels.Any())
            {
                return;
            }

            if (expiredAuctionModels != null && expiredAuctionModels.Any())
            {
                var minDate = await dbContext.CharacterExpiredAuctions.Select(x => x.ExpiredTime).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
                Character[] characters = dbContext.Characters.ToArray();
                var characterExpiredAuctions = dbContext.CharacterExpiredAuctions.Where(x => x.ExpiredTime >= minDate).Select(x => x.Hash).ToArray();

                foreach (var characterGroupedExpiredAuctions in expiredAuctionModels.GroupBy(x => x.Player))
                {
                    Character character = characters.Single(c => c.Name == characterGroupedExpiredAuctions.Key);

                    foreach (var expiredAuctionModel in characterGroupedExpiredAuctions.Where(x => x.Time >= minDate && !characterExpiredAuctions.Any(h => x.Hash == h)))
                    {
                        CharacterExpiredAuction storeCharacterExpiredAuction = new()
                        {
                            Character = character,
                            ExpiredTime = expiredAuctionModel.Time.UtcDateTime,
                            ItemID = expiredAuctionModel.ItemId,
                            Quantity = expiredAuctionModel.Quantity,
                            StackSize = expiredAuctionModel.StackSize,
                            Hash = expiredAuctionModel.Hash
                        };

                        await dbContext.AddAsync(storeCharacterExpiredAuction);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task StoreItemNames(IDictionary<string, string> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

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