using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand ScanBackupsCommand => new RelayCommand(() => ScanBackups());

        public MainWindowViewModel(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        private async Task ScanBackups()
        {
            var backup = await TsmBackupParser.ParseBackup(new System.IO.FileInfo(@"C:\Users\justi\AppData\Roaming\TradeSkillMaster\TSMApplication\Backups\SILEDORF_1644093144.zip"));

            await dataStore.StoreCharacters(backup.Characters);
            await dataStore.StoreAuctionBuys(backup.AuctionBuys);
            await dataStore.StoreCharacterSales(backup.CharacterSaleModels);
            await dataStore.StoreExpiredAuctions(backup.ExpiredAuctions);
            await dataStore.StoreCancelledAuctions(backup.CancelledAuctions);
        }

        private void FirePropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
