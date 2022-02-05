using System.Windows;

namespace TSM_Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new TSM.Logic.Data_Parser.TsmBackupParser().ParseBackup(new System.IO.FileInfo(@"C:\Users\justi\AppData\Roaming\TradeSkillMaster\TSMApplication\Backups\SILEDORF_1643701576.zip"));
        }
    }
}