using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace QueryService
{
    public class AMTS
    {
        string CommandID;

        static readonly string ArgumentExceptionMessage = "There isn't any parameters";

        static readonly string askdf = nameof(askdf);

        static readonly string askdf_old = nameof(askdf_old);

        static readonly string query = nameof(query);

        static readonly string vmain = nameof(vmain);

        static readonly string v = nameof(v);

        static readonly string
            sprav_n = nameof(sprav_n),
            pr_firms = nameof(pr_firms),
            p_pasp_dan = nameof(p_pasp_dan),
            p_fio = nameof(p_fio),
            p_pasp2 = nameof(p_pasp2),
            p_ln_pasp = nameof(p_ln_pasp),
            p_adres = nameof(p_adres),
            v_fio = nameof(v_fio),
            v_pasp2 = nameof(v_pasp2),
            v_ln_pasp = nameof(v_ln_pasp),
            v_adres = nameof(v_adres),
            marka_expl = nameof(marka_expl),
            model = nameof(model),
            tc = nameof(tc),
            vid = nameof(vid),
            godv = nameof(godv),
            n_mash = nameof(n_mash),
            n_vin = nameof(n_vin),
            n_shas = nameof(n_shas),
            n_kuz = nameof(n_kuz),
            n_dvig = nameof(n_dvig),
            obem = nameof(obem),
            nomer_tc = nameof(nomer_tc),
            regimp2 = nameof(regimp2),
            regimp = nameof(regimp),
            ts_25a = nameof(ts_25a),
            housesp = nameof(housesp),
            pointsp = nameof(pointsp),
            dataf = nameof(dataf),
            datasp = nameof(datasp),
            datak = nameof(datak),
            nraz_uvv = nameof(nraz_uvv),
            st_nomd = nameof(st_nomd),
            datat = nameof(datat),
            control = nameof(control),
            annul_flag = nameof(annul_flag),
            p_name = nameof(p_name),
            opersp = nameof(opersp),
            prev_nraz = nameof(prev_nraz),
            next_nraz = nameof(next_nraz),
            dattf = nameof(dattf),
            p_unp = nameof(p_unp);

        static readonly string _CommandText = $@"SELECT {v}.{sprav_n}, /*  varchar2 */
                                                        {v}.{nraz_uvv}, /*  varchar2 */
                                                        {v}.{dattf}, /* date */
                                                        {v}.{datak}, /* date */

                                                        decode({v}.{pr_firms}, '1', {v}.{p_pasp_dan}, {v}.{p_fio}) {p_name}, /* varchar2 */
                                                        decode({v}.{pr_firms}, '1', NULL, {v}.{p_pasp2}) {p_pasp2}, /* varchar2 */
                                                        decode({v}.{pr_firms}, '1', NULL, {v}.{p_ln_pasp}) {p_ln_pasp}, /* varchar2 */
                                                        decode({v}.{pr_firms}, '1', {v}.{p_ln_pasp}, NULL) {p_unp}, /* varchar2 */
                                                        {v}.{p_adres}, /* varchar2 */ 

                                                        {v}.{v_fio}, /* varchar2 */
                                                        {v}.{v_pasp2}, /* varchar2 */
                                                        {v}.{v_ln_pasp}, /* varchar2 */
                                                        {v}.{v_adres}, /* varchar2 */

                                                        {v}.{marka_expl} ||' ' ||{v}.{model} {tc}, /* varchar2 */
                                                        {v}.{godv}, /* number */
                                                        {v}.{obem}, /* number(6) */
                                                        {v}.{n_vin}, /* varchar2 */
                                                        {v}.{n_shas}, /* varchar2 */
                                                        {v}.{n_kuz}, /* varchar2 */
                                                        {v}.{n_mash}, /* varchar2 */

                                                        {v}.{regimp2}, /* varchar2 */
                                                        {v}.{regimp}, /* varchar2 */

                                                        {v}.{ts_25a}, /* varchar2 */

                                                        decode(instr({v}.{st_nomd}, 'Ф'), 0, decode(instr({v}.{st_nomd}, 'Д'), 0, decode({v}.{opersp}, 'Ф', NULL, {v}.{st_nomd}), NULL), {v}.{st_nomd}) {prev_nraz}, /* varchar2 */
                                                        decode(instr({v}.{st_nomd}, 'Д'), 0, decode(instr({v}.{st_nomd}, 'Ф'), 0, decode({v}.{opersp}, 'Д', NULL, {v}.{st_nomd}), NULL), {v}.{st_nomd}) {prev_nraz}, /* varchar2 */
                                                        decode({v}.{control}, '250', '1', '251', '1', '252', '1', '0') {annul_flag} /* varchar2 */
                                                                        
                                                     FROM {askdf}.{vmain} {v}";

        public AMTS(string CommandID)
        {
            this.CommandID = CommandID;
        }

        public static TimeSpan TestQueryDefault
        {
            get
            {
                TimeSpan time1 = new TimeSpan(),
                         time2 = new TimeSpan();

                var task1 = Task.Factory.StartNew( () => time1 = OraConn.ExecuteTestQuery(OraConn.DB, $"{askdf}.{vmain}"));
                var task2 = Task.Factory.StartNew( () => time1 = OraConn.ExecuteTestQuery(OraConn.DB, $"{askdf_old}.{vmain}"));

                Task.WaitAll(task1, task2);

                return new List<TimeSpan> { time1, time2 }.Max();
            }
        }

        public static TimeSpan TestQueryOwner
        {
            get
            {
                return TestQueryDefault;
            }
        }

        public static TimeSpan TestQueryTS
        {
            get
            {
                return TestQueryDefault;
            }
        }

        public string QueryDefault(DateTime? _datasp_from, DateTime? _datasp_to, string _sprav_n,
                                    string _ts_25a, string _regim, string _p_pasp_dan)
        {
            if (_sprav_n == null && _ts_25a == null && _regim == null && _p_pasp_dan == null) 
                throw new ArgumentException(ArgumentExceptionMessage);

            var commandText = $@"{_CommandText} WHERE {v}.{regimp} = :1
                                                        AND {v}.{sprav_n} = upper(:2)
                                                        AND trunc({v}.{datasp}) >= trunc(:3)
                                                        AND trunc({v}.{datasp}) <= trunc(:4)
                                                        AND {v}.{pr_firms} = '1' 
                                                        AND {v}.{p_pasp_dan} LIKE upper(:5)
                                                        AND {v}.{ts_25a} LIKE upper(:6)";

            OptimazeCommandTextByDates(_datasp_from, _datasp_to, ref commandText);

            if (_p_pasp_dan != null) _p_pasp_dan = $"%{_p_pasp_dan}%";

            var parameters = new List<object>
            {
                _regim, _sprav_n, _datasp_from, _datasp_to, _p_pasp_dan, _ts_25a
            };

            return GetQueryResults(commandText, parameters);
        }

        public string QueryOwner(DateTime? _datasp_from, DateTime? _datasp_to, string _fio, string _pasp2, string _ln_pasp) 
        {
            if (_fio == null && _pasp2 == null && _ln_pasp == null) throw new ArgumentException(ArgumentExceptionMessage);

            var commandText = $@"{_CommandText}
                                WHERE
                                    trunc({v}.{datasp}) >= trunc(:1)
                                AND trunc({v}.{datasp}) <= trumc(:2)
                                AND ({v}.{v_fio} LIKE upper(:3) OR {v}.{p_fio} LIKE upper(:3))
                                AND ({v}.{v_pasp2} LIKE upper(:4) OR {v}.{v_pasp2} LIKE upper(:4))
                                AND ({v}.{v_ln_pasp} LIKE upper(:5) OR {v}.{v_ln_pasp} LIKE upper(:5))";

            if (_fio != null) _fio = $"%{_fio}%";

            var parameters = new List<object>
            {
                _datasp_from, _datasp_to, _fio, _pasp2, _ln_pasp
            };

            return GetQueryResults(commandText, parameters);
        }

        
        private string GetQueryResults(string commandText, List<object> parameters)
        {
            using (var connection = new OracleConnection(OraConn.DB))
                using(var command = OraConn.CreateCommand(CommandID, connection, commandText, parameters))
            {
                var dt = new DataTable();

                connection.Open();
                dt.Load(command.ExecuteReader());

                try
                {
                    return string.Empty; //JsonConvert.SerializeObject(dt.AsEnumerable());
                }
                finally 
                {
                    dt.Dispose(); 
                }
            }
        }

        private void OptimazeCommandTextByDates(DateTime? dataStart, DateTime? dataEnd, ref string commandText)
        {
            if ((dataStart.HasValue && dataStart.Value.Year < 2004) || (dataEnd.HasValue && dataEnd.Value.Year < 2004))
                throw new ArgumentException("There isn't period for a search. ");

            if (dataStart != null && dataEnd != null) 
            {
                if (dataStart.Value.Year < DateTime.Now.Year - 9 && dataEnd.Value.Year >= DateTime.Now.Year - 9)
                    commandText = Regex.Replace(commandText, $"{askdf}.{vmain}", $"{query}.{query}", RegexOptions.IgnoreCase);

                if (dataStart.Value.Year == dataEnd.Value.Year)
                    commandText = Regex.Replace(commandText, $"{askdf}.{vmain}", $"{askdf}.{vmain} PARTITION(Y{dataStart.Value.Year})", RegexOptions.IgnoreCase);

                if (dataStart.Value.Year < DateTime.Now.Year - 9 && dataEnd.Value.Year < DateTime.Now.Year - 9)
                    commandText = Regex.Replace(commandText, $"{askdf}", $"{askdf_old}", RegexOptions.IgnoreCase);
            }
            else
            {
                if((dataStart != null && dataStart.Value.Year < DateTime.Now.Year - 9) || dataStart == null && dataEnd == null)
                    commandText = Regex.Replace(commandText, $"{askdf}.{vmain}", $"{query}.{query}", RegexOptions.IgnoreCase);

                if (dataEnd != null && dataEnd.Value.Year < DateTime.Now.Year - 9)
                    commandText = Regex.Replace(commandText,$"{askdf}", $"{askdf_old}", RegexOptions.IgnoreCase);
                
            }
        }
    }
}