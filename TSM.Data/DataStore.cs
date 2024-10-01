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
            CharacterBuy[] storeCharacterBuys = await dbContext.CharacterBuys.ToArrayAsync();

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
            CharacterAuctionSale[] storeSales = await dbContext.CharacterAuctionSales.ToArrayAsync();

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
            Character[] storeCharacters = await dbContext.Characters.Include(c => c.CharacterMailItems).Include(c => c.CharacterBankItems)
                .Include(c => c.CharacterReagents).Include(c => c.CharacterInventoryItems).ToArrayAsync();

            return storeCharacters.Select(c => (core.Character)c).ToArray();
        }

        public async Task<IEnumerable<core.ExpiredAuctionModel>> GetExpiredAuctionModels()
        {
            if (await dbContext.CharacterExpiredAuctions.AnyAsync(x => x.Hash == 0))
            {
                IQueryable<CharacterExpiredAuction> dbModels = dbContext.CharacterExpiredAuctions.Where(x => x.Hash == 0);

                foreach (CharacterExpiredAuction? m in dbModels)
                {
                    core.ExpiredAuctionModel tempModel = new()
                    {
                        ItemId = m.ItemID,
                        Player = m.Character.Name,
                        Quantity = m.Quantity,
                        StackSize = m.StackSize,
                        TimeEpoch = new DateTimeOffset(m.ExpiredTime, TimeSpan.Zero).ToUnixTimeSeconds()
                    };

                    m.Hash = tempModel.GetHashCode();
                }

                _ = await dbContext.SaveChangesAsync();
            }

            core.ExpiredAuctionModel[] models = await dbContext.CharacterExpiredAuctions.Select(x => new core.ExpiredAuctionModel
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

        public async Task<core.WarBank> GetWarBankAsync()
        {
            WarBank? warBank = await dbContext.WarBanks.FirstOrDefaultAsync();

            return new core.WarBank
            {
                Money = new(warBank?.Copper ?? 0)
            };
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
            DateTime minDate = await dbContext.CharacterBuys.Select(x => x.BoughtTime).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
            CharacterBuy[] characterBuys = dbContext.CharacterBuys.Where(x => x.BoughtTime >= minDate).ToArray();

            IEnumerable<IGrouping<string, core.AuctionBuyModel>> characterGroupedAuctionBuyModels = auctionBuyModels.Where(x => x.Time >= minDate).GroupBy(x => x.Player);
            foreach (IGrouping<string, core.AuctionBuyModel> characterGroupedAuctionBuyModel in characterGroupedAuctionBuyModels)
            {
                Character character = characters.Single(c => c.Name == characterGroupedAuctionBuyModel.Key);

                foreach (core.AuctionBuyModel? auctionBuyModel in characterGroupedAuctionBuyModel)
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

                        _ = await dbContext.AddAsync(storeCharacterBuy);
                    }
                }
            }

            _ = await dbContext.SaveChangesAsync();
        }

        public async Task StoreBackupScanned(FileInfo backupFile, DateTimeOffset startTime)
        {
            _ = await dbContext.ScannedBackups.AddAsync(new ScannedBackup
            {
                BackupPath = backupFile.FullName,
                ScannedTime = startTime.UtcDateTime,
                Duration = DateTimeOffset.Now.Subtract(startTime).TotalSeconds
            });

            _ = await dbContext.SaveChangesAsync();
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
                DateTime minDate = await dbContext.CharacterCancelledAuctions.Select(x => x.CancelledTime).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
                CharacterCancelledAuction[] characterCancelledAuctions = dbContext.CharacterCancelledAuctions.Where(x => x.CancelledTime >= minDate).ToArray();

                IEnumerable<IGrouping<string, core.CancelledAuctionModel>> characterGroupedCancelledAuctionModels = cancelledAuctionModels.Where(x => x.Time >= minDate).GroupBy(x => x.PlayerName);
                foreach (IGrouping<string, core.CancelledAuctionModel> characterGroupedCancelledAuctionModel in characterGroupedCancelledAuctionModels)
                {
                    Character character = characters.Single(c => c.Name == characterGroupedCancelledAuctionModel.Key);

                    foreach (core.CancelledAuctionModel? cancelledAuctionModel in characterGroupedCancelledAuctionModel)
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

                            _ = await dbContext.AddAsync(storeCharacterCancelledAuction);
                        }
                    }
                }

                _ = await dbContext.SaveChangesAsync();
            }
        }

        public async Task StoreCharacters(IEnumerable<core.Character> characters)
        {
            if (characters == null || !characters.Any())
            {
                return;
            }

            foreach (core.Character character in characters)
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

                    _ = await dbContext.AddAsync(storeCharacter);
                }

                storeCharacter.Copper = character.Money.TotalCopper;
                SetCharacterBank(storeCharacter.CharacterBankItems, character.BankItems);
                SetCharacterReagents(storeCharacter.CharacterReagents, character.ReagentItems);
                SetCharacterInventory(storeCharacter.CharacterInventoryItems, character.BagItems);
                SetCharacterMail(storeCharacter.CharacterMailItems, character.MailItems);
                storeCharacter.LastUpdateTime = character.GoldLogLastUpdate.UtcDateTime;
            }
            _ = await dbContext.SaveChangesAsync();
        }

        public async Task StoreCharacterSales(IEnumerable<core.CharacterSaleModel> characterSaleModels)
        {
            if (characterSaleModels == null || !characterSaleModels.Any())
            {
                return;
            }

            Character[] characters = dbContext.Characters.ToArray();
            DateTime minDate = await dbContext.CharacterAuctionSales.Select(x => x.TimeOfSale).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
            CharacterAuctionSale[] characterAuctionSales = dbContext.CharacterAuctionSales.AsEnumerable().Where(x => x.TimeOfSale >= minDate).ToArray();

            IEnumerable<IGrouping<string, core.CharacterSaleModel>> saleModelsByCharacter = characterSaleModels.Where(x => x.TimeOfSale >= minDate).GroupBy(x => x.Character);
            foreach (IGrouping<string, core.CharacterSaleModel> groupedCharacterSaleModel in saleModelsByCharacter)
            {
                Character character = characters.Single(c => c.Name == groupedCharacterSaleModel.Key);

                foreach (core.CharacterSaleModel? characterSaleModel in groupedCharacterSaleModel)
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

                        _ = await dbContext.AddAsync(storeCharacterAuctionSale);
                    }
                }
            }

            _ = await dbContext.SaveChangesAsync();
        }

        public async Task StoreExpiredAuctions(IEnumerable<core.ExpiredAuctionModel> expiredAuctionModels)
        {
            if (expiredAuctionModels == null || !expiredAuctionModels.Any())
            {
                return;
            }

            if (expiredAuctionModels != null && expiredAuctionModels.Any())
            {
                DateTime minDate = await dbContext.CharacterExpiredAuctions.Select(x => x.ExpiredTime).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefaultAsync();
                Character[] characters = dbContext.Characters.ToArray();
                int[] characterExpiredAuctions = dbContext.CharacterExpiredAuctions.Where(x => x.ExpiredTime >= minDate).Select(x => x.Hash).ToArray();

                foreach (IGrouping<string, core.ExpiredAuctionModel> characterGroupedExpiredAuctions in expiredAuctionModels.GroupBy(x => x.Player))
                {
                    Character character = characters.Single(c => c.Name == characterGroupedExpiredAuctions.Key);

                    foreach (core.ExpiredAuctionModel? expiredAuctionModel in characterGroupedExpiredAuctions.Where(x => x.Time >= minDate && !characterExpiredAuctions.Any(h => x.Hash == h)))
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

                        _ = await dbContext.AddAsync(storeCharacterExpiredAuction);
                    }
                }

                _ = await dbContext.SaveChangesAsync();
            }
        }

        public async Task StoreItemNames(IDictionary<string, string> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            string[] storeItems = dbContext.Items.Select(x => x.ItemID).ToArray();

            foreach (KeyValuePair<string, string> item in items.Where(x => !storeItems.Contains(x.Key)))
            {
                _ = await dbContext.Items.AddAsync(new Item
                {
                    ItemID = item.Key,
                    Name = item.Value
                });
            }

            _ = await dbContext.SaveChangesAsync();
        }

        public async Task UpdateWarBank(core.WarBank warBank)
        {
            if (warBank is null)
            {
                return;
            }

            WarBank? storeWarBank = dbContext.WarBanks.FirstOrDefault();

            if (storeWarBank is null)
            {
                storeWarBank = new();
                _ = await dbContext.WarBanks.AddAsync(storeWarBank);
            }

            storeWarBank.Copper = warBank.Money?.TotalCopper ?? 0;

            _ = await dbContext.SaveChangesAsync();
        }

        private void SetCharacterBank(ICollection<CharacterBank> characterBankItems, Dictionary<string, int> bankItems)
        {
            characterBankItems.Clear();

            foreach (KeyValuePair<string, int> kvp in bankItems)
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

            foreach (KeyValuePair<string, int> kvp in bagItems)
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

            foreach (KeyValuePair<string, int> mailItem in mailItems)
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

            foreach (KeyValuePair<string, int> kvp in reagentItems)
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