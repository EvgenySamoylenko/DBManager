using System.Windows;
using System.Windows.Documents;
using System.Data;
using System;
using System.Web.Script.Serialization;
using System.IO; 
using System.Windows.Controls;
using System.ComponentModel;

namespace DBManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SetupConnWin setupwin = null;
        MainWindowViewModel mainWndVm = new MainWindowViewModel();
        DataTable tbl;
        SplashInfo infoBox = null;

        bool resultOfDataReload = false;
        private void RunOnBackGround()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += delegate (object s, DoWorkEventArgs args) 
            {
                args.Result = mainWndVm.ReloadData();
                if (!CheckAccess())
                {
                    // On a different thread
                    Dispatcher.Invoke(() => infoBox.TextToShow.Text = "All is done...");
                    return;
                }
            };
            
            worker.RunWorkerCompleted += delegate (object s, System.ComponentModel.RunWorkerCompletedEventArgs args)
            {
                infoBox.Close();
                resultOfDataReload = (bool)args.Result;
            };

            worker.RunWorkerAsync();
        }
            //public string connoptions; //----------------

        public MainWindow()
        {

            InitializeComponent();
            this.DataContext = mainWndVm;

            SaveToCsv.IsEnabled = false;
            AddFromCsv.IsEnabled = false;
            Execute_Querry.IsEnabled = false;
            Disconnect.IsEnabled = false;
        }

        private void ConnectBtn(object sender, RoutedEventArgs e)
        { 
            /*setupwin = new SetupConnWin(mainWndVm, this);
            setupwin.ShowDialog();
            mainWndVm.setConnStr(setupwin.connstr);
            mainWndVm.ReloadData();*/

            setupwin = new SetupConnWin(mainWndVm, this);
            setupwin.ShowDialog();
            mainWndVm.setConnStr(setupwin.connstr);
            infoBox = new SplashInfo(this);
            infoBox.Show();
            RunOnBackGround();
        }

        private void DisconnectBtn(object sender, RoutedEventArgs e)
        {
            mainWndVm.Clear();
        }

        private void Execute_Querry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextRange queryText = new TextRange(querryBox.Document.ContentStart, querryBox.Document.ContentEnd);
                //answerGrid.ItemsSource = mainWndVm.Querying(queryText.Text, mainWndVm.connectionString).AsDataView();
                tbl = mainWndVm.Querying(queryText.Text, mainWndVm.connectionString);
                answerGrid.ItemsSource = tbl.AsDataView();
            }
            catch (Exception exept) { MessageBox.Show(exept.Message); }
        }

        private void SaveToCsv_Click(object sender, RoutedEventArgs e)
        {
            SavetToCsvFile.ExtractDataToCSV(tbl);
        }

        private void AddFromCsv_Click(object sender, RoutedEventArgs e)
        {
            LoadFromCsvFile.LoadToDb(mainWndVm.connectionString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ContentPresenter titlePanel = MenuPanel.Template.FindName("PART_TitleHost", MenuPanel) as ContentPresenter;
            if (titlePanel != null)
            {
                double titleHeight = titlePanel.ActualHeight;
                MenuPanel.Margin = new Thickness(MenuPanel.Margin.Left, -titleHeight, MenuPanel.Margin.Right, MenuPanel.Margin.Bottom);
            }
        }
        private void DeleteFromDB(object sender, RoutedEventArgs e)
        {
            var listOfItems= ServObserv.SelectedItem;
            string itemType = listOfItems.GetType().ToString();
            int idxOfDot = itemType.LastIndexOf(".");
            string tableorcolumName = "";

            if (idxOfDot > 0 && itemType.Substring(idxOfDot + 1) == "DbTable")
            {
                  tableorcolumName = ((DbTable)listOfItems).TableName;
                  mainWndVm.RemoveTable((DbTable)listOfItems, mainWndVm.connectionString);
            }
            else if (idxOfDot > 0 && itemType.Substring(idxOfDot + 1) == "TableColumn")
            {
                 tableorcolumName = ((TableColumn)listOfItems).ColumnName;
                 mainWndVm.RemoveTableColumn((TableColumn)listOfItems, mainWndVm.connectionString);
            }

            //mainWndVm.RemoveTable((DbTable)listOfItems, mainWndVm.connectionString);
            //mainWndVm.ReloadData();
        }

        private void ServObserv_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
             TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
             if (treeViewItem != null)
             {
                 treeViewItem.Focus();
                 e.Handled = true;
             }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            source = System.Windows.Media.VisualTreeHelper.GetParent(source);
            
            return source as TreeViewItem;
        }

    } 
}