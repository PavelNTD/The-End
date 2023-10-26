using ISS_Client.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Deployment;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ISS_Client
{
    internal class GlobalVars
    {

        //public static List<Country> CountriesList { get; set; }

        //public static List<PTO> PTOList {  get; set; }

        static bool adminFlag = false;

        public static string CurrUserName
        {
            get { return adminFlag ? "ADMIN" : Settings.Default.LastUserName; }
            set { } 
        }

        public static string CurrCustomsCode
        {
            get { return adminFlag ? "02" : Settings.Default.LastCustomsCode; }
            set { }
        }

        public static ViewModel ViewModel = new ViewModel();

        public static string amts_dt_by_TT { get { return "Автотранспортные средства (юр. лица) Беларусь"; } }
        public static string amts_by_TT { get { return "Автотранспортные средства (физ. лица) Беларусь"; } }
        public static string Reestr_TT { get { return "Реестры"; } }

        static readonly string PageCatalog = "QueryPages/";

        static readonly Uri amts_1 = new Uri($"{PageCatalog + nameof(QueryPages.AMTS_1)}.xaml", UriKind.Relative);
        static readonly Uri amts_2 = new Uri($"{PageCatalog + nameof(QueryPages.AMTS_2)}.xaml", UriKind.Relative);
        static readonly Uri amts_3 = new Uri($"{PageCatalog + nameof(QueryPages.AMTS_3)}.xaml", UriKind.Relative);

        static readonly Uri amts_dt = new Uri($"{PageCatalog + nameof(QueryPages.AMTS_DT)}.xaml", UriKind.Relative);

        static readonly Uri reestrs = new Uri($"{PageCatalog + nameof(QueryPages.Reestrs)}.xaml", UriKind.Relative);


        public static List<Menu> DefaultMenu = new List<Menu>
        {
            new Menu
            {
                DbCode = "17",
                DbName = "Реестры",
                //LockMessage = Settings.Default.,
                ToolTip = Reestr_TT,
                QueryID = 41,
                Uri = reestrs
            } 
        };

        public static List<Menu> Sub2 = new List<Menu>
        {
            new Menu
            {
                DbCode = "04",
                Text = "Republic of Belarus",
                //LockMessage = Settings.Default.,
                ToolTip = amts_by_TT,
                NewID = 1,
                Sub = new List<Sub_Menu> 
                            { 
                                new Sub_Menu 
                                {
                                    Text = "Seach by information about movement TC",
                                    QueryID = 8,
                                    NewID = 1,
                                    Uri = amts_1
                                },
                                new Sub_Menu
                                {
                                    Text = "Seach by owner / mover",
                                    QueryID = 9,
                                    NewID = 1,
                                    Uri = amts_2
                                },
                                new Sub_Menu
                                {
                                    Text = "Seach by weacle",
                                    QueryID = 9,
                                    NewID = 1,
                                    Uri = amts_3
                                }
                            }
            }
        };

        public static List<Menu> Sub3 = new List<Menu>
        {
            new Menu
            {
                DbCode = "05",
                Text = "Republic of Belarus",
                //LockMessage= Settings.Default.,
                ToolTip = amts_dt_by_TT,
                QueryID = 11,
                Uri = amts_dt
            }
        };


        public static readonly string AssemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();

        public static string GetMainExceptionMessage(Exception exception)
        {
            var message = exception.Message;
            var ex = exception;
            var coll = 0;

            while (ex.InnerException != null) 
            { 
                ex = ex.InnerException;
                message += $"\r\n{ex.Message.PadLeft(ex.Message.Length + 2 * ++coll)}";
            }

            return message;
        }

        public class Menu:INotifyPropertyChanged
        {
            private string _DbCode;
            public string DbCode
            {
                get { return _DbCode; }
                set { _DbCode = value; }
            }

            private string _Text;
            public string Text
            {
                get { return _Text; }
                set { _Text = value; }
            }

            private bool _IsLock = false;
            public bool IsLock 
            {
                get { return _IsLock; }
                set { _IsLock = value; OnPropertyChanged(nameof(IsLock)); }
            }

            private string _LockMessage;
            public string LockMessage
            {
                get { return _LockMessage; }
                set { _LockMessage = value; OnPropertyChanged(nameof(LockMessage)); }
            }

            private int _ActiveCommands = 0;
            public int ActiveCommands
            {
                get { return _ActiveCommands; }
                set { _ActiveCommands = value; OnPropertyChanged(nameof(ActiveCommands));}
            }

            private int _QueryID = 0;
            public int QueryID
            {
                get { return QueryID; }
                set { _QueryID = value; }
            }

            private string _DbName;
            public string DbName
            {
                get { return DbName; }
                set { _DbName = value; }
            }

            private int _NewID = 0;
            public int NewID
            {
                get { return NewID; }
                set { _NewID = value; }
            }

            private string _ToolTip;
            public string ToolTip
            {
                get { return ToolTip; }
                set { _ToolTip = value;}
            }

            private Uri _Uri;
            public Uri Uri
            {
                get { return Uri; }
                set { _Uri = value;}
            }

            private List<Sub_Menu> _Sub;
            public List<Sub_Menu> Sub
            {
                get { return this._Sub; }
                set { _Sub = value;}
            }

            private bool _MouseOver = false;
            public bool MouseOver
            {
                get { return MouseOver; }
                set { _MouseOver = value; OnPropertyChanged(nameof(MouseOver));}
            }

            private bool _ExecuteQuery = false;
            public bool ExecuteQuery
            {
                get { return _ExecuteQuery; }
                set { _ExecuteQuery = value; OnPropertyChanged(nameof(ExecuteQuery)); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class Sub_Menu
        {
            public int QueryID { get; set; } = 1;

            public string Text { get; set; }

            public int NewID = 0;

            private string _ToolTip;
            public string ToolTip
            {
                get { return string.IsNullOrWhiteSpace(_ToolTip) ? Text : _ToolTip; }
                set { _ToolTip = value; }
            }

            public Uri Uri { get; set; }
        }
    }
}
