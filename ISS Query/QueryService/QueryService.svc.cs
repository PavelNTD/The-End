using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using static QueryService.OraConn;

namespace QueryService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueryService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select QueryService.svc or QueryService.svc.cs at the Solution Explorer and start debugging.
    
    public class QueryService : IQueryService
    {
        static string UserIP
        {
            get 
            {
                var context = OperationContext.Current;
                var prop = context.IncomingMessageProperties;
                var endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                return endpoint.Address;
            }
        }

        static string GetMainExceptionMessage(Exception exception)
        {
            var message = exception.Message;

            var ex = exception;
            var count = 0;

            while (ex.InnerException != null) 
            {
                ex = ex.InnerException;

                message += $"\r\n{ex.Message.PadLeft(ex.Message.Length + 2 * ++count)}";
            }

            return message;
        }

        static DatabaseInfo AMTS_Info = new DatabaseInfo { DBCode = "01" };
        static DatabaseInfo Reestr_Info = new DatabaseInfo { DBCode = "13" };

        public string ExecuteQuery(string commandID, string Login, string CustomsCode, string DBCode, int QueryID, object[] parameers)
        {
            string DBServer = $"DBCode = {DBCode}";
            switch(DBCode)
            {
                case "01":
                    {
                        DBServer = "10.2.1.21";
                    }break;
                case "13":
                    {
                        DBServer = "10.2.1.13";
                    }
                    break;
            }

            var command = new OraCommand { CommandID = commandID, ServerIP = DBServer };

            string res;
            try
            {
                OraConn.OraCommands.Add(command);
                AddQueryInfo(QueryID);

                switch(QueryID)
                {
                    //case 8: res = new AMTS(CommandID).
                    default: break;
                }
            }
            catch (Exception ex) { }

            return string.Empty;
        }

        void AddQueryInfo(int QueryID) 
        {
            switch (QueryID) 
            {
                case 8:
                case 9:
                case 10: AMTS_Info.ActiveCommands++; break;
                case 41: Reestr_Info.ActiveCommands++; break;

                default: throw new NotImplementedException("Can't identify query");
            }
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
