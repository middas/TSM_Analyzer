using TSM.Core.Models;

namespace TSM.Core.LocalStorage
{
    public interface IDataStore
    {
        void Configure();

        Task StoreAuctionBuys(IEnumerable<AuctionBuyModel> auctionBuyModels);

        Task StoreCancelledAuctions(IEnumerable<CancelledAuctionModel> cancelledAuctionModels);

        Task StoreCharacters(IEnumerable<Character> characters);

        Task StoreCharacterSales(IEnumerable<CharacterSaleModel> characterSaleModels);

        Task StoreExpiredAuctions(IEnumerable<ExpiredAuctionModel> expiredAuctionModels);
    }
}