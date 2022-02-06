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

        public async Task StoreCancelledAuctions(IEnumerable<core.CancelledAuctionModel> cancelledAuctionModels)
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

        public async Task StoreCharacters(IEnumerable<core.Character> characters)
        {
            foreach (var character in characters)
            {
                Character storeCharacter = await dbContext.Characters.SingleOrDefaultAsync(c => c.Name == character.Name && c.Faction == character.Faction.ToString() && c.Realm == character.Realm);

                if (storeCharacter == null)
                {
                    storeCharacter = new()
                    {
                        Class = character.Class,
                        Name = character.Name,
                        Faction = character.Faction.ToString(),
                        Realm = character.Realm
                    };

                    await dbContext.AddAsync(storeCharacter);
                }

                storeCharacter.Copper = character.Money.TotalCopper;
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
                    && x.TimeOfSale == characterSaleModel.TimeOfSale.UtcDateTime && x.Copper == characterSaleModel.SoldValue.TotalCopper && x.Quantity == characterSaleModel.Count);

                if (storeCharacterAuctionSale == null)
                {
                    storeCharacterAuctionSale = new()
                    {
                        Character = characters.Single(c => c.Name == characterSaleModel.Character.Name),
                        Copper = characterSaleModel.SoldValue.TotalCopper,
                        ItemID = characterSaleModel.ItemID,
                        Quantity = characterSaleModel.Count,
                        TimeOfSale = characterSaleModel.TimeOfSale.UtcDateTime
                    };

                    await dbContext.AddAsync(storeCharacterAuctionSale);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task StoreExpiredAuctions(IEnumerable<core.ExpiredAuctionModel> expiredAuctionModels)
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
}