using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QueryService
{
    public class DBServersInfo
    {
        public int AllActiveCommands { get; set; }
        public Dictionary<string, int> ServersDetails { get; set; }
    }

    public class DatabaseInfo
    {
        public string IPAddress { get; set; }
        public string DBCode { get; set; }

        public int ActiveCommands { get; set; }

    }


}