using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ISS_Client
{
    internal class ViewModel: INotifyPropertyChanged
    {
        public List<GlobalVars.Menu> QueryMenu
        {
            get
            {
                return GlobalVars.DefaultMenu;
            }
        }

        public List<GlobalVars.Menu> QueryMenu2
        {
            get
            {
                return GlobalVars.Sub2;
            }
        }

        public List<GlobalVars.Menu> QueryMenu3
        {
            get
            {
                return GlobalVars.Sub3;
            }
        }

        private List<FlowDocument> _DocumentsList;
        public List<FlowDocument> DocumentsList 
        { 
            get { return _DocumentsList; }
            set { _DocumentsList = value; _CurrDocumentColl = value == null || value.Count == 0 ? 0 : 1; OnPropertyChanged(nameof(DocumentsList)); }
        }

        private int _CurrDocumentColl;
        public int CurrDocumentColl
        {
            get { return _CurrDocumentColl; }
            set { _CurrDocumentColl = value; OnPropertyChanged(nameof(_CurrDocumentColl)); OnPropertyChanged(nameof(IsHaveNextDocumentFlag)); OnPropertyChanged(nameof(IsHavePrevDocumentFlag)); }
        }

        public FlowDocument CurrentDocument 
        {
            get { return CurrDocumentColl == 0 ? new FlowDocument() : DocumentsList[CurrDocumentColl - 1]; }
        }

        public bool IsHaveNextDocumentFlag
        {
            get { return CurrDocumentColl != 0 && CurrDocumentColl != DocumentsList.Count; }
        }

        public bool IsHavePrevDocumentFlag
        {
            get { return CurrDocumentColl != 0 && CurrDocumentColl != 1; }
        }


        private bool _QueryPageLoadFlag;
        public bool QueryPageLoadFlag
        {
            get { return _QueryPageLoadFlag; }
            set { _QueryPageLoadFlag = value; OnPropertyChanged(nameof(QueryPageLoadFlag));}
        }

        private bool _ExecuteQueryFlag;
        public bool ExecuteQueryFlag
        {
            get { return _ExecuteQueryFlag; }
            set { _ExecuteQueryFlag = value; OnPropertyChanged(nameof(ExecuteQueryFlag)); }
        }

        private bool _TestQueryFlag;
        public bool TestQueryFlag
        {
            get { return _TestQueryFlag; }
            set { _TestQueryFlag = value; OnPropertyChanged(nameof(TestQueryFlag)); }
        }

        private bool _QueryResultFlag;
        public bool QueryResultFlag
        {
            get { return _QueryResultFlag; }
            set { _QueryResultFlag = value; OnPropertyChanged(nameof(QueryResultFlag)); }
        }

        private bool _ExportFlag;
        public bool ExportFlag
        {
            get { return _ExportFlag; }
            set { _ExportFlag = value; OnPropertyChanged(nameof(ExportFlag)); }
        }

        private int _CurrQueryId;
        public int CurrQueryId
        {
            get { return _CurrQueryId; }
            set { _CurrQueryId = value; OnPropertyChanged(nameof(isExportEnabled)); }
        }


        private bool _ExportWordFlag;
        public bool ExportWordFlag
        {
            get { return _ExportWordFlag; }
            set { _ExportWordFlag = value; OnPropertyChanged(nameof(ExportWordFlag)); }
        }

        private bool _ExportAccessFlag;
        public bool ExportAccessFlag
        {
            get { return _ExportAccessFlag; }
            set { _ExportAccessFlag = value; OnPropertyChanged(nameof(ExportAccessFlag)); }
        }

        private bool _ExportTxtFlag;
        public bool ExportTxtFlag
        {
            get { return _ExportTxtFlag; }
            set { _ExportTxtFlag = value; OnPropertyChanged(nameof(ExportTxtFlag)); }
        }

        private bool _ExportExcelFlag;
        public bool ExportExcelFlag
        {
            get { return _ExportExcelFlag; }
            set { _ExportExcelFlag = value; OnPropertyChanged(nameof(ExportExcelFlag)); }
        }


        public bool isExportEnabled
        {
            get { return ExportWordFlag || ExportAccessFlag || ExportExcelFlag || ExportTxtFlag; }
        }

        private bool _TextContainerFlag;
        public bool TextContainerFlag
        {
            get { return _TextContainerFlag; }
            set { _TextContainerFlag = value; OnPropertyChanged(nameof(TextContainerFlag)); }
        }

        public string WindowTitle
        {
            get { return $"ISS Query {GlobalVars.AssemblyVersion}"; }
        }

        public static int DataHeaderHeight { get; } = 25;

        private bool _DataHeaderFlag;
        public bool DataHeaderFlag
        {
            get 
            {
                return _DataHeaderFlag;
            }
            set
            {
                _DataHeaderFlag = value;
                OnPropertyChanged(nameof(DataHeaderFlag));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
