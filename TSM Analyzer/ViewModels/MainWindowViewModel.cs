using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TSM.Core.LocalStorage;
using TSM.Logic.Data_Parser;
using TSM_Analyzer.Mvvm;

namespace TSM_Analyzer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IDataStore dataStore;

        public MainWindowViewModel(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand ScanBackupsCommand => new RelayCommand(() => ScanBackups());

        private void FirePropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private async Task ScanBackups()
        {
            var backup = await TsmBackupParser.ParseBackup(new System.IO.FileInfo(@"C:\Users\justi\AppData\Roaming\TradeSkillMaster\TSMApplication\Backups\SILEDORF_1644093144.zip"));

            DateTimeOffset startTime = DateTimeOffset.Now;
            await dataStore.StoreCharacters(backup.Characters);
            await dataStore.StoreAuctionBuys(backup.AuctionBuys);
            await dataStore.StoreCharacterSales(backup.CharacterSaleModels);
            await dataStore.StoreExpiredAuctions(backup.ExpiredAuctions);
            await dataStore.StoreCancelledAuctions(backup.CancelledAuctions);
            await dataStore.StoreBackupScanned(new System.IO.FileInfo(@"C:\Users\justi\AppData\Roaming\TradeSkillMaster\TSMApplication\Backups\SILEDORF_1644093144.zip"), startTime);
        }
    }
}