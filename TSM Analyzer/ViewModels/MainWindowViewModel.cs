using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TSM.Core.Extensions;
using TSM.Core.LocalStorage;
using TSM.Core.Models;
using TSM.Logic.Data_Parser;
using TSM.Logic.ItemLookup;
using TSM_Analyzer.Mvvm;

namespace TSM_Analyzer.ViewModels
{
    public class DataGridModel
    {
        public enum ModelType
        {
            Purchase,
            Sale,
            Expired,
            Cancelled
        }

        public string Character { get; set; }

        public string ItemID { get; set; }

        public string ItemName { get; set; }

        public Money Money { get; set; }

        public int Quantity { get; set; }

        public string Source { get; set; }

        public int StackSize { get; set; }

        public DateTime? Time { get; set; }

        public Money Total { get; set; }

        public ModelType Type { get; set; }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string backupPath = @"TradeSkillMaster\TSMApplication\Backups";

        private readonly IDataStore dataStore;

        private AuctionBuyModel[] auctionBuyModels;
        private string backupStatus;
        private bool canFilter = true;
        private bool canLookupItems = true;
        private bool canScan = true;
        private Character[] characterData;
        private CharacterSaleModel[] characterSaleModels;
        private ICollectionView dataGridModels;
        private DateTime filterFrom;
        private DateTime filterTo;
        private Func<double, string> goldLabelFormatter;
        private ObservableCollection<string> labels;
        private DataGridColumn selectedColumn;
        private SeriesCollection series;
        private Money totalGold = 0;
        private Money totalProfit = 0;
        private Money totalPurchases = 0;
        private Money totalSales = 0;

        public MainWindowViewModel(IDataStore dataStore)
        {
            this.dataStore = dataStore;

            FilterFrom = DateTime.Now.AddDays(-14);
            FilterTo = DateTime.Now.AddDays(1);

            Task.Run(PopulateAllData);
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

        public bool CanFilter
        {
            get => canFilter;
            set
            {
                canFilter = value;
                FirePropertyChanged();
            }
        }

        public bool CanLookupItems
        {
            get => canLookupItems;
            set
            {
                canLookupItems = value;
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

        public ICollectionView DataGridModels
        {
            get => dataGridModels;
            set
            {
                dataGridModels = value;
                FirePropertyChanged();
            }
        }

        public ICommand FilterCommand => new RelayCommand(() => FilterData());

        public DateTime FilterFrom
        {
            get => filterFrom;
            set
            {
                filterFrom = value;
                FirePropertyChanged();
            }
        }

        public DateTime FilterTo
        {
            get => filterTo;
            set
            {
                filterTo = value;
                FirePropertyChanged();
            }
        }

        public Func<double, string> GoldLabelFormatter
        {
            get => goldLabelFormatter;
            set
            {
                goldLabelFormatter = value;
                FirePropertyChanged();
            }
        }

        public ObservableCollection<string> Labels
        {
            get => labels;
            set
            {
                labels = value;
                FirePropertyChanged();
            }
        }

        public ICommand LookupMissingItemsCommand => new RelayCommand(() => LookupMissingItems());

        public ICommand ScanBackupsCommand => new RelayCommand(() => ScanBackups());

        public DataGridColumn SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;
                FirePropertyChanged();
            }
        }

        public SeriesCollection Series
        {
            get => series;
            set
            {
                series = value;
                FirePropertyChanged();
            }
        }

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

        private IEnumerable<DateTimeOffset> EnumerateDays(DateTimeOffset start, DateTimeOffset end)
        {
            if (end < start) throw new ArgumentException($"{nameof(end)} cannot be less than {nameof(start)}");

            while (start <= end)
            {
                yield return start.Date;
                start = start.AddDays(1);
            }
        }

        private void FilterData()
        {
            CanFilter = false;

            PopulateAllData();

            CanFilter = true;
        }

        private void FirePropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private async Task LookupMissingItems()
        {
            CanLookupItems = false;
            await new ItemLookup().LookupItemsAsync(DataGridModels.SourceCollection.OfType<DataGridModel>().Where(x => x.ItemName is null).Select(x => x.ItemID).Distinct(), dataStore);
            await PopulateDataGrid();
            CanLookupItems = true;
        }

        private void PopulateAllData()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                PopulateCharacterData().Wait();
                PopulateAuctionData().Wait();
                PopulateDataGrid().Wait();

                PopulateChartData();
            });
        }

        private async Task PopulateAuctionData()
        {
            TotalProfit = TotalSales = TotalPurchases = 0;

            try
            {
                auctionBuyModels = (await dataStore.GetAuctionBuyModels()).Where(x => x.Time >= FilterFrom && x.Time < FilterTo).ToArray();
                if (auctionBuyModels != null && auctionBuyModels.Any(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)))
                {
                    TotalPurchases = auctionBuyModels.Where(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)).Select(x => x.Total).Sum();
                }

                characterSaleModels = (await dataStore.GetCharacterSaleModels()).Where(x => x.TimeOfSale >= FilterFrom && x.TimeOfSale < FilterTo).ToArray();
                if (characterSaleModels != null && characterSaleModels.Any(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)))
                {
                    TotalSales = characterSaleModels.Where(x => x.Source.Equals("auction", StringComparison.OrdinalIgnoreCase)).Select(x => x.Total).Sum();
                }

                var minDate = DateTimeOffset.FromUnixTimeSeconds(Math.Min(auctionBuyModels.Min(x => x.TimeEpoch), characterSaleModels.Min(x => x.Time)));
                if (FilterFrom < minDate.Date)
                {
                    FilterFrom = minDate.Date;
                }

                TotalProfit = TotalSales - TotalPurchases;
            }
            catch
            {
                // no data
            }
        }

        private async Task PopulateCharacterData()
        {
            characterData = (await dataStore.GetCharactersData()).ToArray();

            if (characterData != null && characterData.Any())
            {
                TotalGold = characterData.Select(c => c.Money).Aggregate((left, right) => left + right);
            }
        }

        private void PopulateChartData()
        {
            GoldLabelFormatter = c => ((Money)(long)c).ToString();
            var valuesByDate = new Dictionary<DateTimeOffset, BoughtSold>();
            var sold = characterSaleModels.GroupBy(x => x.TimeOfSale.LocalDateTime.Date).Select(x => new { x.Key, Money = x.Select(y => y.Total).Sum() });
            var bought = auctionBuyModels.GroupBy(x => x.Time.LocalDateTime.Date).Select(x => new { x.Key, Money = x.Select(y => y.Total).Sum() });

            Money prevDay = 0;
            foreach (var date in EnumerateDays(FilterFrom.Date, FilterTo))
            {
                valuesByDate[date] = new BoughtSold(prevDay)
                {
                    Bought = bought.SingleOrDefault(x => x.Key == date)?.Money ?? 0,
                    Sold = sold.SingleOrDefault(x => x.Key == date)?.Money ?? 0,
                };

                prevDay = valuesByDate[date].RunningTotal;
            }

            Labels = new ObservableCollection<string>(valuesByDate.Select(x => x.Key.ToLocalTime().ToString("d")));

            Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Auction Sold Value",
                    Values = new ChartValues<long>(valuesByDate.Select(x => x.Value.Sold.TotalCopper)),
                    PointGeometry = null,
                    LabelPoint = point => ((Money)(long)point.Y).ToString()
                },
                new LineSeries
                {
                    Title = "Auction Bought Value",
                    Values = new ChartValues<long>(valuesByDate.Select(x => -x.Value.Bought.TotalCopper)),
                    PointGeometry = null,
                    LabelPoint = point => ((Money)(long)point.Y).ToString()
                },
                new LineSeries
                {
                    Title = "Total Profit",
                    Values = new ChartValues<long>(valuesByDate.Select(x => (x.Value.RunningTotal).TotalCopper)),
                    PointGeometry = null,
                    LabelPoint = point => ((Money)(long)point.Y).ToString()
                }
            };
        }

        private async Task PopulateDataGrid()
        {
            var items = await dataStore.GetItems();
            List<DataGridModel> models = new();

            try
            {
                models.AddRange(characterSaleModels.Where(x => x.TimeOfSale >= FilterFrom && x.TimeOfSale < FilterTo).Select(x => new DataGridModel
                {
                    ItemID = x.ItemID,
                    Money = x.Money,
                    Quantity = x.Quantity,
                    Time = x.TimeOfSale.LocalDateTime,
                    Source = x.Source,
                    StackSize = x.StackSize,
                    ItemName = items.ContainsKey(x.ItemID) ? items[x.ItemID] : null,
                    Type = DataGridModel.ModelType.Sale,
                    Total = x.Total,
                    Character = x.Character
                }));
                models.AddRange(auctionBuyModels.Where(x => x.Time >= FilterFrom && x.Time < FilterTo).Select(x => new DataGridModel
                {
                    Time = x.Time.LocalDateTime,
                    ItemID = x.ItemId,
                    ItemName = items.ContainsKey(x.ItemId) ? items[x.ItemId] : null,
                    Money = -x.Money,
                    Quantity = x.Quantity,
                    Source = x.Source,
                    StackSize = x.StackSize,
                    Type = DataGridModel.ModelType.Purchase,
                    Total = -x.Total,
                    Character = x.Player
                }));
                models.AddRange((await dataStore.GetCancelledAuctionModels()).Where(x => x.Time >= FilterFrom && x.Time < FilterTo).Select(x => new DataGridModel
                {
                    ItemID = x.ItemId,
                    ItemName = items.ContainsKey(x.ItemId) ? items[x.ItemId] : null,
                    Money = null,
                    Quantity = x.Quantity,
                    Source = "Auction",
                    StackSize = x.StackSize,
                    Time = x.Time.LocalDateTime,
                    Type = DataGridModel.ModelType.Cancelled,
                    Character = x.PlayerName
                }));
                models.AddRange((await dataStore.GetExpiredAuctionModels()).Where(x => x.Time >= FilterFrom && x.Time < FilterTo).Select(x => new DataGridModel
                {
                    ItemID = x.ItemId,
                    ItemName = items.ContainsKey(x.ItemId) ? items[x.ItemId] : null,
                    Money = null,
                    Quantity = x.Quantity,
                    Source = "Auction",
                    StackSize = x.StackSize,
                    Time = x.Time.LocalDateTime,
                    Type = DataGridModel.ModelType.Expired,
                    Character = x.Player
                }));
            }
            catch (AggregateException)
            {
                // no data
            }

            DataGridModels = CollectionViewSource.GetDefaultView(models.OrderBy(x => x.Time));            
        }

        private async Task ScanBackups()
        {
            CanScan = false;

            DirectoryInfo backupDirectory = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), backupPath));
            var backupsScanned = await dataStore.GetBackupsScanned();
            await Task.Run(async () =>
            {
                TsmBackupParser tsmBackupParser = new();
                tsmBackupParser.OnStatusUpdated += s =>
                {
                    BackupStatus = s;
                };

                var result = await tsmBackupParser.ParseBackups(backupDirectory, backupsScanned);

                BackupStatus = "Storing character data.";
                await dataStore.StoreCharacters(result.Item1.Characters);

                BackupStatus = "Storing bought auction data.";
                await dataStore.StoreAuctionBuys(result.Item1.AuctionBuys);

                BackupStatus = "Storing cancelled auction data.";
                await dataStore.StoreCancelledAuctions(result.Item1.CancelledAuctions);

                BackupStatus = "Storing auction sale data.";
                await dataStore.StoreCharacterSales(result.Item1.CharacterSaleModels);

                BackupStatus = "Storing expired auction data.";
                await dataStore.StoreExpiredAuctions(result.Item1.ExpiredAuctions);

                BackupStatus = "Storing item names.";
                await dataStore.StoreItemNames(result.Item1.Items);

                BackupStatus = "Storing processed files.";
                foreach(var file in result.Item2)
                {
                    await dataStore.StoreBackupScanned(file.FileName, file.ScanTime);
                }

                await PopulateCharacterData();
                await PopulateAuctionData();
            });

            CanScan = true;
            BackupStatus = "";

            PopulateChartData();
            await PopulateDataGrid();
        }

        private class BoughtSold
        {
            private readonly Money prevDay;

            public BoughtSold(Money prevDay)
            {
                this.prevDay = prevDay;
            }

            public Money Bought { get; set; }

            public Money RunningTotal => prevDay + Sold - Bought;

            public Money Sold { get; set; }
        }
    }
}