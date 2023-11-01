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
                MessageBox.Show($"Тут мог быбыть ваш запрос \r {GlobalVars.ViewModel.CurrQueryId}");
                switch (GlobalVars.ViewModel.CurrQueryId)
                {
                    //case 8: flag = ((AMTS_1)QueryFields.Content).regimp.isDropDownOpen; break;
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
                case nameof(GetNextElementBt): GlobalVars.ViewModel.CurrDocumentColl++; break;
                case nameof(GetLastElementBt): GlobalVars.ViewModel.CurrDocumentColl = GlobalVars.ViewModel.DocumentsList.Count; break;
                case nameof(GetPrevElementBt): GlobalVars.ViewModel.CurrDocumentColl--; break;
                case nameof(GetFirstElementBt): GlobalVars.ViewModel.CurrDocumentColl = 1; break;
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
        {/*
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
                    }*/
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
            if (GlobalVars.ViewModel.ExecuteQueryFlag) return;

            if (QueryID == 0) return;

            if (QueryFields.Source != null && !recylce_Flag)
            {
                if (Regex.Match(QueryFields.CurrentSource.OriginalString, @"/\w+.xaml$").Value == Regex.Match(uri.OriginalString, @"/\w+.xaml$").Value) 
                    return;
            }

            QueryContainer.RowDefinitions[3].MinHeight = 0;
            QueryContainer.RowDefinitions[3].Height = new GridLength(0);
            Splitter.IsEnabled = false;
            GlobalVars.ViewModel.QueryPageLoadFlag = true;

            QueryFields.Source = null;
            DataContainer.ItemsSource = null;
            DataContainer.Items.Refresh();
            GlobalVars.ViewModel.DocumentsList = new List<FlowDocument>();
            TextContainer.Document = GlobalVars.ViewModel.CurrentDocument;
            GlobalVars.ViewModel.QueryResultFlag = false;

            await Task.Factory.StartNew(() => { Thread.Sleep(1200); });

            QueryFields.Source = uri;

            switch(QueryID) 
            {
                case 8:
                case 9:
                case 10:
                    QueryNameContainer.Text = "АМТС Республика Беларусь";
                    QueryNameContainer.ToolTip = GlobalVars.amts_by_TT; break;

                case 11:
                    QueryNameContainer.Text = "АМТС ДТ Республика Беларусь";
                    QueryNameContainer.ToolTip = GlobalVars.amts_dt_by_TT; break;

                case 41:
                    QueryNameContainer.Text = "Реестр физических лиц";
                    QueryNameContainer.ToolTip = GlobalVars.Reestr_TT; break;
            }

            CurrPage = uri;
            GlobalVars.ViewModel.CurrQueryId = QueryID;

            GlobalVars.ViewModel.QueryPageLoadFlag = false;
            QueryContainer.RowDefinitions[3].Height = new GridLength(ViewModel.DataHeaderHeight);
            GlobalVars.ViewModel.DataHeaderFlag = false;

            GC.Collect();
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
            MessageBox.Show("GetQueryResult");
            /*
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

                    
                }*/
            return null;
        }

        bool itemflag = false;
        private void ItemMouseDown(object sender, MouseEventArgs e)
        {
            itemflag = true;
        }

        private void ItemMouseLeave(object sender, MouseEventArgs e)
        {
            itemflag = false;
        }

        private void ItemMouseUp(object sender, MouseEventArgs e)
        {
            if (!itemflag) return;

            try
            {
                var item = ((Grid)sender).DataContext as GlobalVars.Menu;

                if (Menu_Dop.Width != 0) HideSubMenu();
                if (Menu_Dop.Width != 0 && item.NewID == 1) HideSubMenu2();

                LoadQueryPage(item.Uri, item.QueryID);
            }
            catch (NullReferenceException) 
            {
                var item = ((Grid)sender).DataContext as GlobalVars.Sub_Menu;

                if (Menu_Dop.Width != 0) HideSubMenu();
                if (Menu_Dop.Width != 0 && item.NewID == 1) HideSubMenu2();

                LoadQueryPage(item.Uri, item.QueryID);
            }
        }


        private async void HideSubMenu2()
        {
            if (Menu_Dop2.Width != 250) return;

            try
            {
                var animation = new DoubleAnimation
                {
                    From = 250,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.2),
                    FillBehavior = FillBehavior.HoldEnd
                };

                Menu_Dop2.BeginAnimation(WidthProperty, animation);

                if (Menu_Dop2.Width != 0)
                    await Task.Factory.StartNew(() => { Thread.Sleep(200); });

                GlobalVars.ViewModel.QueryMenu2.Find(x => x.MouseOver == true).MouseOver = false;
            }
            catch { }
        }

        private async void HideSubMenu()
        {
            if (Menu_Dop.Width != 250) return;

            try
            {
                var animation = new DoubleAnimation
                {
                    From = 250,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(2),
                    FillBehavior = FillBehavior.HoldEnd
                };

                Menu_Dop.BeginAnimation(WidthProperty, animation);

                if (Menu_Dop.Width != 0)
                    await Task.Factory.StartNew(() => { Thread.Sleep(200); });

                GlobalVars.ViewModel.QueryMenu.Find(x => x.MouseOver == true).MouseOver = false;
            }
            catch { }            
        }//2183

        private async void MenuItemMouseEnter(object sender, MouseEventArgs e)//2212
        {
            if (Menu.Width != 250) return;

            var currItem = GlobalVars.ViewModel.QueryMenu.FirstOrDefault(x => x.MouseOver == true);
            var item = ((Grid)sender).DataContext as GlobalVars.Menu;

            if (item == null && currItem == null) return;

            if (item.QueryID != 0) return;

            if (currItem == item) return;

            if (currItem != null) currItem.MouseOver = false;

            if (Menu.Width != 0)
            {
                var animation = new DoubleAnimation
                {
                    From = 250,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    FillBehavior = FillBehavior.HoldEnd
                };

                Menu.BeginAnimation(WidthProperty, animation);
            }

            item.MouseOver = true;

            if (item.QueryID == 0 && item.NewID == 0)
            {
                var animation = new DoubleAnimation
                {
                    To = 250,
                    Duration = TimeSpan.FromSeconds(0.3),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if(Menu_Dop.Width != 0)
                {
                    await Task.Factory.StartNew(() => { Thread.Sleep(300); });
                }

                if (item != GlobalVars.ViewModel.QueryMenu.FirstOrDefault(x => x.MouseOver == true)) return;

                Menu_Dop.ItemsSource = item.Sub;
                Menu_Dop.BeginAnimation(WidthProperty, animation);
            }

            if (item.QueryID == 0 && item.NewID == 1)
            {
                var animation = new DoubleAnimation
                {
                    To = 250,
                    Duration = TimeSpan.FromSeconds(0.3),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (Menu_Dop.Width != 0)
                {
                    await Task.Factory.StartNew(() => { Thread.Sleep(300); });
                }

                if (item != GlobalVars.ViewModel.QueryMenu.FirstOrDefault(x => x.MouseOver == true)) return;

                Menu_Dop.ItemsSource = GlobalVars.ViewModel.QueryMenu2;
                Menu_Dop.BeginAnimation(WidthProperty, animation);
            }

            if (item.QueryID == 0 && item.NewID == 2)
            {
                var animation = new DoubleAnimation
                {
                    To = 250,
                    Duration = TimeSpan.FromSeconds(0.3),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (Menu_Dop.Width != 0)
                {
                    await Task.Factory.StartNew(() => { Thread.Sleep(300); });
                }

                if (item != GlobalVars.ViewModel.QueryMenu.FirstOrDefault(x => x.MouseOver == true)) return;

                Menu_Dop.ItemsSource = GlobalVars.ViewModel.QueryMenu3;
                Menu_Dop.BeginAnimation(WidthProperty, animation);
            }
        }

        private void MenuItemMouseLeave(object sender, MouseEventArgs e)
        {
            var item = ((Grid)sender).DataContext as GlobalVars.Menu;

            if (item.NewID == 1 && Menu_Dop2 != null) 
            {
                HideSubMenu2();
            }

            if (item.QueryID == 0 && item.NewID == 0)
            {
                if (!Menu_Dop.IsMouseOver)
                {
                    var animation = new DoubleAnimation
                    {
                        From = 250,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    Menu_Dop.BeginAnimation(WidthProperty, animation);

                }
                else
                    return;
            }

            if (item.QueryID == 0 && item.NewID == 1)
            {
                if (!Menu_Dop.IsMouseOver)
                {
                    var animation = new DoubleAnimation
                    {
                        From = 250,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    Menu_Dop.BeginAnimation(WidthProperty, animation);
                }
                else
                    return;
            }

            if (item.QueryID == 0 && item.NewID == 2)
            {
                if (!Menu_Dop.IsMouseOver)
                {
                    var animation = new DoubleAnimation
                    {
                        From = 250,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    Menu_Dop.BeginAnimation(WidthProperty, animation);
                }
                else
                    return;
            }

            item.MouseOver = false;
        }

        private async void MenuItemMouseEnter2(object sender, MouseEventArgs e)//2212
        {
            if (Menu_Dop.Width != 250) return;

            var currItem = GlobalVars.ViewModel.QueryMenu2.FirstOrDefault(x => x.MouseOver == true);
            var item = ((Grid)sender).DataContext as GlobalVars.Menu;

            if (item == null && currItem == null)
            {
                currItem = GlobalVars.ViewModel.QueryMenu3.FirstOrDefault(x => x.MouseOver == true);
                item = ((Grid)sender).DataContext as GlobalVars.Menu;
            }
            if (item == null && currItem == null) return;

            if (item.QueryID != 0) return;

            if (currItem == item) return;

            if (currItem != null) currItem.MouseOver = false;

            if (Menu_Dop2.Width != 0)
            {
                var animation = new DoubleAnimation
                {
                    From = 250,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    FillBehavior = FillBehavior.HoldEnd
                };

                Menu_Dop2.BeginAnimation(WidthProperty, animation);
            }

            item.MouseOver = true;

            if (item.QueryID == 0 && item.NewID == 1)
            {
                var animation = new DoubleAnimation
                {
                    To = 250,
                    Duration = TimeSpan.FromSeconds(0.3),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (Menu_Dop.Width != 0)
                {
                    await Task.Factory.StartNew(() => { Thread.Sleep(300); });
                }

                if (item != GlobalVars.ViewModel.QueryMenu2.FirstOrDefault(x => x.MouseOver == true)) return;

                Menu_Dop2.ItemsSource = item.Sub;
                Menu_Dop2.BeginAnimation(WidthProperty, animation);
            }
        }

        private void MenuItemMouseLeave2(object sender, MouseEventArgs e) //2395  41
        {
            var item = ((Grid)sender).DataContext as GlobalVars.Menu;

            if (item == null) return;

            if (item.QueryID == 0) 
            {
                if (!Menu_Dop2.IsMouseOver)
                {
                    var animation = new DoubleAnimation
                    {
                        From = 250,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    Menu_Dop2.BeginAnimation(WidthProperty, animation);
                }
                else
                    return;
            }
            item.MouseOver = false;
        }
    }
}
