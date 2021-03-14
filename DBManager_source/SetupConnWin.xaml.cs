﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace DBManager
{
    
    /// <summary>
    /// Interaction logic for SetupConnWin.xaml
    /// </summary>
    public partial class SetupConnWin : Window
    {

        MainWindowViewModel mainWnd = null;
        MainWindow MainDispForm = null;
        public string connstr;
        bool selectMode;

        public SetupConnWin(MainWindowViewModel mainWin, MainWindow mainDispForm)
        {
            InitializeComponent();
            ServerNameStr.Text = Properties.Settings.Default.severName;
            InitDBName.Text = Properties.Settings.Default.currentDb;
            mainWnd = mainWin;
            MainDispForm = mainDispForm;
            LoginStr.IsEnabled = false;
            PassStr.IsEnabled = false;
            InitDBName.IsEnabled = false;
        }

        private void Cancell_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
               
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (AuthTypeSelector.SelectedIndex == 0) { selectMode = true; } else { selectMode = false; }
            if (WithDBName.IsChecked == false) { InitDBName.Text = null; }

            connstr = DBConnMode.GetConnectionString(ServerNameStr.Text, InitDBName.Text, LoginStr.Text, PassStr.Text, selectMode);

            MainDispForm.SaveToCsv.IsEnabled = true;
            MainDispForm.SaveToCsv.IsEnabled = true;
            MainDispForm.AddFromCsv.IsEnabled = true;
            MainDispForm.Execute_Querry.IsEnabled = true;
            MainDispForm.Disconnect.IsEnabled = true;

            Close();
        }

        private void AuthTypeSelector_DropDownClosed(object sender, EventArgs e)
        {
            if (AuthTypeSelector.SelectedIndex == 0)
            {
                LoginStr.IsEnabled = false;
                PassStr.IsEnabled = false;
            }
            else if (AuthTypeSelector.SelectedIndex == 1)
            {
                LoginStr.IsEnabled = true;
                PassStr.IsEnabled = true;
            }
        }

    }
}
