using TSM.Core.Models;

namespace TSM.Core.LocalStorage
{
    public interface IDataStore
    {
        void Configure();

        Task<IEnumerable<AuctionBuyModel>> GetAuctionBuyModels();

        Task<string[]> GetBackupsScanned();

        Task<IEnumerable<CancelledAuctionModel>> GetCancelledAuctionModels();

        Task<IEnumerable<CharacterSaleModel>> GetCharacterSaleModels();

        Task<IEnumerable<Character>> GetCharactersData();

        Task<IEnumerable<ExpiredAuctionModel>> GetExpiredAuctionModels();

        Task<Dictionary<string, string>> GetItems();

        Task StoreAuctionBuys(IEnumerable<AuctionBuyModel> auctionBuyModels);

        Task StoreBackupScanned(FileInfo backupFile, DateTimeOffset startTime);

        Task StoreCancelledAuctions(IEnumerable<CancelledAuctionModel> cancelledAuctionModels);

        Task StoreCharacters(IEnumerable<Character> characters);

        Task StoreCharacterSales(IEnumerable<CharacterSaleModel> characterSaleModels);

        Task StoreExpiredAuctions(IEnumerable<ExpiredAuctionModel> expiredAuctionModels);

        Task StoreItemNames(IDictionary<string, string> items);
    }
}