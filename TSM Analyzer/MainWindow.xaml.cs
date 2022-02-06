using System.Windows;
using TSM.Core.LocalStorage;
using TSM_Analyzer.ViewModels;

namespace TSM_Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow(IDataStore dataStore)
        {
            InitializeComponent();

            DataContext = viewModel = new MainWindowViewModel(dataStore);
        }
    }
}