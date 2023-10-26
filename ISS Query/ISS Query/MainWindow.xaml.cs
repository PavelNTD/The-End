using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Remoting.Channels;
using ISS_Client.QueryPages;
using System.Net;

namespace ISS_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Uri CurrPage;

        string CurrDbName {
            get
            {
                switch (GlobalVars.ViewModel.CurrQueryId)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4: return "MNS";

                    case 8:
                    case 9:
                    case 10: return "AMTS";

                    case 11: return "AMTS_DT";

                    case 12: return "Cennik";

                    default: return "ZaprosExport";
                }
            }
        }

        static string CommandID;

        Type dataType;

        static string Login = GlobalVars.CurrUserName;
        static string CustomCode = GlobalVars.CurrCustomsCode;

        public MainWindow()
        {
            this.DataContext = GlobalVars.ViewModel;
            InitializeComponent();

            GetNextElementBt.MouseLeftButtonUp += NavigationBt_MouseLeftButtonUp;
            GetLastElementBt.MouseLeftButtonUp -= NavigationBt_MouseLeftButtonUp;
            GetPrevElementBt.MouseLeftButtonUp+= NavigationBt_MouseLeftButtonUp;
            GetFirstElementBt.MouseLeftButtonUp-= NavigationBt_MouseLeftButtonUp;

            DataObject.AddCopyingHandler(TextContainer, RichTextBox_Copyinghandler);

            var menu = GlobalVars.ViewModel.QueryMenu;

            ShowHideMenuButton.MouseLeftButtonDown += (sender, e) =>
                FirstGrid.ColumnDefinitions[0].Width = FirstGrid.ColumnDefinitions[0].Width == new GridLength(0) ? new GridLength(300) : new GridLength(0);

            DataContainer.AutoGeneratingColumn += (sender, e) =>
            {
                e.Column.Header = e.Column.Header.ToString().Replace_();
                e.Column.CanUserSort = true;

                if (e.PropertyType == typeof(DateTime?))
                    ((DataGridTextColumn)e.Column).Binding.StringFormat = "dd.MM.yyyy";

                if (e.PropertyType == typeof(DateTime))
                    ((DataGridTextColumn)e.Column).Binding.StringFormat = "dd.MM.yyyy HH:mm";
            };

            this.PreviewMouseDown += (sender, e) => //206
            {
                var flag = false;
                switch (GlobalVars.ViewModel.CurrQueryId)
                {
                    case 8: flag = ((AMTS_1)QueryFields.Content).regimp.isDropDownOpen; break;
                        //case 45: flag = ((CBD_RKTS_Searching_5)QueryFields.Content).custom.isDropDownOpen; ; break;
                }

                if (!flag)
                {
                    QueryNameContainer.Focusable = true;
                    QueryNameContainer.Focus();
                    QueryNameContainer.Focusable = false;
                }
            };

            this.KeyDown += (sender, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
                    CancelCommand();

                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.T && GlobalVars.ViewModel.CurrQueryId != 0)
                    new TestQueryWindow { Owner = this }.ShowDialog();
            };

            UserGuideButton.Click += (sender, e) =>
            {
                try
                {
                    Process.Start(@"Instructions\Руководство пользователя.doc");
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            RecylceFormBt.Click += (sender, e) =>
            {
                var result = MessageBox.Show("Страница будет обновлена. Все данные ьудут утеряны.\r\nЖелаете продолжить?","Очистка формы", MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.OK);

                if (result == MessageBoxResult.Cancel) return;

                LoadQueryPage(CurrPage, GlobalVars.ViewModel.CurrQueryId, true);
            };

            RunQueryBt.Click += async (sender, e) =>
            {
                Splitter.IsEnabled = true;
                if (QueryContainer.RowDefinitions[3].Height == new GridLength(ViewModel.DataHeaderHeight))
                {
                    QueryContainer.RowDefinitions[1].Height = new GridLength(1.5, GridUnitType.Star);
                    QueryContainer.RowDefinitions[3].Height = new GridLength(5, GridUnitType.Star);
                    QueryContainer.RowDefinitions[3].MinHeight = ViewModel.DataHeaderHeight;
                }

                GlobalVars.ViewModel.ExecuteQueryFlag = true;

                await Task.Factory.StartNew(() =>
                {
                    var DBCode = string.Empty;
                    List<object> parameters = new List<object>();
                    string res = null;

                    switch (GlobalVars.ViewModel.CurrQueryId)
                    {
                        case 666: break;
                    }
                });

                GlobalVars.ViewModel.ExecuteQueryFlag = false;
                GlobalVars.ViewModel.DataHeaderFlag = true;
            };

            this.Closing += (sender, e) => 
            {
                if (GlobalVars.ViewModel.ExecuteQueryFlag && MessageBox.Show("Are you sure about that?", "ISS Query X", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.Cancel)
                    e.Cancel = true;
            };
        }

        void RichTextBox_CopingHandler(object sender, DataObjectEventArgs e)
        {
            try
            {
                var richTextbox = (RichTextBox)sender;
            }
            catch (Exception) { return; }

            e.CancelCommand();
            Clipboard.SetText(((RichTextBox)sender).Selection.Text);
        }


        private void NavigationBt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)sender;
            

            if (grid.Background == Brushes.White) return;

            switch (grid.Name)
            {
                case nameof(GetNextElementBt): GlobalVars.ViewModel.CurDocumentColl++; break;
                case nameof(GetLastElementBt): GlobalVars.ViewModel.CurDocumentColl = GlobalVars.ViewModel.DocumentList.Count; break;
                case nameof(GetPrevElementBt): GlobalVars.ViewModel.CurDocumentColl--; break;
                case nameof(GetFirstElementBt): GlobalVars.ViewModel.CurDocumentColl = 1; break;
            }
        }

        void RichTextBox_Copyinghandler(object sender, DataObjectEventArgs e)
        {
            try { var richTextBox = (RichTextBox)sender; } 
            catch (Exception) { return; }

            e.CancelCommand();
            Clipboard.SetText(((RichTextBox)sender).Selection.Text);
        }

        void CancelCommand()
        {
            if (GlobalVars.ViewModel.ExecuteQueryFlag)
                using (var client = new ZaprosServiceClient())
                    try
                    {
                        client.CancelCommand(CommandID);
                    }
                    catch (EndPointNotFoundException)
                    {
                        EndpointNotFoundException();
                    }
                    catch (Exception ex) 
                    {
                        Exception(ex.Message);
                    }
        }

        void EndpointNotFoundException() 
        {
            Dispatcher.Invoke(new Action(() =>
                MessageBox.Show("End host can't be reached.\r\n\rMaybe reason is: \r\n    - there isn't locla network connection\r\n    - Server error", "Connection error", MessageBoxButton.OKCancel)));
        }

        void Exception(string message)
        {
            this.Dispatcher.Invoke(new Action(() => MessageBox.Show(message,"Error", MessageBoxButton.OKCancel, MessageBoxImage.Error)));
        }

        private async void LoadQueryPage(Uri uri, int QueryID, bool recylce_Flag = false) //1967
        {
            
        }

        private void SetData<T>(IEnumerable<T> dataList)
        {
            DataContainer.ItemsSource = dataList;
            this.dataType = typeof(T);
            GC.Collect();
        }

        private DataTable GetDataTable() 
        {
            var dataList = DataContainer.ItemsSource;
            var type = this.dataType;
            var properties = type.GetProperties();

            var dataTable = new DataTable();
            foreach ( var info in properties) 
                dataTable.Columns.Add(new DataColumn(info.Name.Length > 64 ? $"{info.Name.Remove(64)}" : info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));

            foreach (var entity in dataList)
            {
                var value = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                    value[i] = properties[i].GetValue(entity, null);
            }
            return dataTable;
        }

        private string GetQueryResult(string DBCode, object[] Parameters) //1816
        {
            using (var client = new ZaprosServiceClient())
                try 
                {
                    CommandID = $"{Login}{CustomCode}{DateTime.Now.ToOADate()}";
                    return client.ExecuteQuery(CommandID, Login, CustomCode, DBCode, GlobalVars.ViewModel.CurrQueryId, Parameters);
                }
                catch (TimeoutException)
                {
                    Task.Factory.StartNew(() => client.CancelCommand(CommandID).Wait());
                    GlobalVars.ViewModel.ExecuteQueryFlag = false;
                    GC.Collect();
                    Dispatcher.Invoke(new Action(() => { 
                        MessageBox.Show("Query working too long.\r\nTry to add mode details for search.", "Waiting time has expired", MessageBoxButton.OK, MessageBoxImage.Error); }));

                    return null;
                }
                catch (FaultException ex) 
                {
                    GlobalVars.ViewModel.ExecuteQueryFlag = false;
                    GC.Collect();
                    Dispatcher.Invoke(new Action(() => {
                        MessageBox.Show(ex.message, "Service error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));

                    return null;
                }
        }
    }
}
