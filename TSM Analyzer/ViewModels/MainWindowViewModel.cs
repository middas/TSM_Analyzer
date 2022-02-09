using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TSM.Core.LocalStorage;
using TSM.Core.Models;
using TSM.Logic.Data_Parser;
using TSM_Analyzer.Mvvm;

namespace TSM_Analyzer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string backupPath = @"TradeSkillMaster\TSMApplication\Backups";

        private readonly IDataStore dataStore;

        private string backupStatus;
        private bool canScan = true;
        private Money totalGold = new(0);
        private Money totalProfit = new(0);
        private Money totalPurchases = new(0);
        private Money totalSales = new(0);
        private Character[] characterData;
        private AuctionBuyModel[] auctionBuyModels;
        private CharacterSaleModel[] characterSaleModels;

        public MainWindowViewModel(IDataStore dataStore)
        {
            this.dataStore = dataStore;

            PopulateCharacterData();
            PopulateAuctionData();
        }

        private async Task PopulateAuctionData()
        {
            auctionBuyModels = (await dataStore.GetAuctionBuyModels()).ToArray();
            if (auctionBuyModels != null && auctionBuyModels.Any(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)))
            {
                TotalPurchases = auctionBuyModels.Where(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)).Select(x => x.Money)
                    .Aggregate((left, right) => left + right);
            }

            characterSaleModels = (await dataStore.GetCharacterSaleModels()).ToArray();
            if (characterSaleModels != null && characterSaleModels.Any(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)))
            {
                TotalSales = characterSaleModels.Where(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)).Select(x => x.Money)
                    .Aggregate((left, right) => left + right);
            }

            TotalProfit = TotalSales - TotalPurchases;
        }

        private async Task PopulateCharacterData()
        {
            characterData = (await dataStore.GetCharactersData()).ToArray();

            if (characterData != null && characterData.Any())
            {
                TotalGold = characterData.Select(c => c.Money).Aggregate((left, right) => left + right);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string BackupStatus
        {
            get => backupStatus;
            set
            {
                backupStatus = value;
                FirePropertyChanged();
            }
        }

        public bool CanScan
        {
            get => canScan;
            set
            {
                canScan = value;
                FirePropertyChanged();
            }
        }

        public ICommand ScanBackupsCommand => new RelayCommand(() => ScanBackups());

        public Money TotalGold
        {
            get => totalGold;
            set
            {
                totalGold = value;
                FirePropertyChanged();
            }
        }

        public Money TotalProfit
        {
            get => totalProfit;
            set
            {
                totalProfit = value;
                FirePropertyChanged();
            }
        }

        public Money TotalPurchases
        {
            get => totalPurchases;
            set
            {
                totalPurchases = value;
                FirePropertyChanged();
            }
        }

        public Money TotalSales
        {
            get => totalSales;
            set
            {
                totalSales = value;
                FirePropertyChanged();
            }
        }

        private void FirePropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private async Task ScanBackups()
        {
            CanScan = false;

            DirectoryInfo backupDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), backupPath));
            var backupsScanned = await dataStore.GetBackupsScanned();
            await Task.Run(async () =>
            {
                foreach (var backupFile in backupDirectory.GetFiles().OrderBy(f => f.CreationTime))
                {
                    if (backupsScanned.Contains(backupFile.FullName))
                    {
                        continue;
                    }

                    BackupStatus = $"Parsing {backupFile.Name}";
                    var backup = await TsmBackupParser.ParseBackup(backupFile);

                    DateTimeOffset startTime = DateTimeOffset.Now;
                    BackupStatus = $"Storing character data for {backupFile.Name}";
                    await dataStore.StoreCharacters(backup.Characters);
                    BackupStatus = $"Storing buy data for {backupFile.Name}";
                    await dataStore.StoreAuctionBuys(backup.AuctionBuys);
                    BackupStatus = $"Storing sale data for {backupFile.Name}";
                    await dataStore.StoreCharacterSales(backup.CharacterSaleModels);
                    BackupStatus = $"Storing expired auction data for {backupFile.Name}";
                    await dataStore.StoreExpiredAuctions(backup.ExpiredAuctions);
                    BackupStatus = $"Storing cancelled auction data for {backupFile.Name}";
                    await dataStore.StoreCancelledAuctions(backup.CancelledAuctions);
                    BackupStatus = $"Storing backup file information for {backupFile.Name}";
                    await dataStore.StoreBackupScanned(backupFile, startTime);
                    BackupStatus = $"Storing known item names for {backupFile.Name}";
                    await dataStore.StoreItemNames(backup.Items);

                    await PopulateCharacterData();
                    await PopulateAuctionData();
                }
            });

            CanScan = true;
            BackupStatus = "";
        }
    }
}