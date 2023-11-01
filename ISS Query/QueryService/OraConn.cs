using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace QueryService
{
    public class OraConn
    {
        public const string DB = "";

        public static List<OraCommand> OraCommands = new List<OraCommand>();

        static readonly string FieldName = nameof(FieldName), Condition = nameof(Condition), PreField = nameof(PreField), PostField = nameof(PostField), PreParam = nameof(PreParam), PostParam = nameof(PostParam);
        public class OraCommand 
        {
            public string CommandID { get; set; }

            public OracleCommand Command;

            public string ServerIP { get; set; }

        }

        static Regex ReplaceParamRowRegex(string paramName)
        {
            var pattern = $@"(?<{PreField}>((upper|trunc)\())?(?<{FieldName}>[A-Za-z]+\.\w+)(?<{PostField}>\))? (?<{Condition}>LIKE|=|>=|<=|BETWEEN) (?<{PreParam}>((upper|trunc)\())?{paramName}(?<{PostParam}>\))?";
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }

        public static Regex DeleteParamRowRegex(string paramName)
        {
            var pattern = $@"^.*{paramName}\D*$";
            return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public static TimeSpan ExecuteTestQuery(string connectionString, string fullTableName)
        {
            var serverIP = string.Empty;
            switch(connectionString) 
            {
                case DB: serverIP = "127.0.0.1"; break;

                default: throw new Exception("Can't identifie string with connection with BD");
            }

            var commandText = $@"SELECT '1' flag
                                FROM {fullTableName}
                                WHERE ROWNUM = 1";

            var dateStart = DateTime.Now;

            using(var connection = new OracleConnection(connectionString))
                using(var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                try
                {
                    connection.Open();
                }
                catch (Exception ex) 
                {
                    throw new Exception($"Can't connect to server {serverIP}\r\n{ex.Message}");
                }

                object res;
                try
                {
                    res = command.ExecuteScalar();
                }
                catch (Exception ex) 
                {
                    throw new Exception($"Can't take data from table {fullTableName.ToUpper()}\r\n{ex.Message}");
                }

                if (res != null)
                    return DateTime.Now - dateStart;
                else
                    throw new Exception($"There's no data in tha table {fullTableName.ToUpper()}");
            }
        }

        public static OracleCommand CreateCommand( string CommandID, OracleConnection connection, string commandText, List<object> parameters, bool OraCommandsFlag = true, int cmdTimeout = 60*20)
        {
            var command = new OracleCommand
            {
                Connection = connection,
                CommandTimeout = cmdTimeout
            };

            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];

                var paramName = $":{i + 1}";

                if (param != null)
                {
                    if (param.GetType() == typeof(string[]))
                    { 
                        foreach (Match match in ReplaceParamRowRegex(paramName).Matches(commandText))
                        {
                            commandText = commandText.Replace(match.Value,
                                $"({string.Join(" OR ", ((string[])param).Select(x => $"{match.Groups[PreField]}{match.Groups[FieldName]}{match.Groups[PostField]} {match.Groups[Condition]} {match.Groups[PreParam]}'{x}'{match.Groups[PostParam]}"))})");
                        }
                    }
                    else
                    {
                        command.Parameters.Add(new OracleParameter($":{i + 1}", param));
                    }
                }
                else
                {
                    commandText = DeleteParamRowRegex(paramName).Replace(commandText, "");
                }
            }

            commandText = Regex.Replace(commandText, @"WHERE\s*AND", "WHERE", RegexOptions.IgnoreCase);

            command.CommandText = commandText;

            if (OraCommandsFlag)
                OraCommands.Find(x => x.CommandID == CommandID).Command = command;

            return command;
        }
    }
}