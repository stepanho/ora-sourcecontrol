using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Utils
{
    /// <summary>
    /// Static class to work with Oracle DB.
    /// Uses "Connection.config" config file to keep connection and additional info.
    /// </summary>
    public static class OracleDB
    {
        static Config<string, string> connConfig = new Config<string, string>("Connection.config");

        /// <summary>
        /// Method initialize connection string from config.
        /// </summary>
        public static void Init()
        {
            //Enter default values below
            connConfig.Update();
            connConfig["Host"] = connConfig["Host"] ?? ""; 
            connConfig["Port"] = connConfig["Port"] ?? "";
            connConfig["ServiceName"] = connConfig["ServiceName"] ?? "";
            connConfig["UserId"] = connConfig["UserId"] ?? "";
            connConfig["Password"] = connConfig["Password"] ?? "";
            connConfig["BindPrefix"] = connConfig["BindPrefix"] ?? "@:";
        }

        /// <summary>
        /// Get actual connection string from config file
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string ConnStr = "";
                ConnStr += "Data Source = ";
                ConnStr += "(DESCRIPTION = ";
                ConnStr += " (ADDRESS_LIST = ";
                ConnStr += " (ADDRESS = (PROTOCOL = TCP)(HOST = " + connConfig["Host"] + ")(PORT = " + connConfig["Port"] + ")) ";
                ConnStr += " ) ";
                ConnStr += " (CONNECT_DATA = ";
                ConnStr += " (SERVICE_NAME = " + connConfig["ServiceName"] + ") ";
                ConnStr += " ) ";
                ConnStr += ");User Id=" + connConfig["UserId"] + ";Password=" + connConfig["Password"] + "";
                return ConnStr;
            }
        }

        /// <summary>
        /// Get or set bind prefix for SQL statement
        /// </summary>
        public static string BindPrefix
        {
            get
            {
                return connConfig["BindPrefix"];
            }
            set
            {
                connConfig["BindPrefix"] = value;
            }
        }

        /// <summary>
        /// Change SQL by replacing bind prefix with parameters.
        /// </summary>
        /// <param name="sql">Input SQL. Bind places numeration starts from 1.</param>
        /// <param name="parameters">Array of parameters for replacement</param>
        /// <returns>Output SQL</returns>
        public static string PrepareSQL(string sql, params string[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                sql = sql.Replace(connConfig["BindPrefix"] + (i + 1), parameters[i]);
            }
            return sql;
        }

        #region Connectivity verification
        /// <summary>
        /// Tries open a connection to DB
        /// </summary>
        /// <returns>True if connectivity is successful, else False</returns>
        public static bool CheckOnlineUsingOracle()
        {
            OracleConnection myConn = new OracleConnection(ConnectionString + ";Connect Timeout=1");
            try
            {
                myConn.Open();
                myConn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tries open TCP port on WAS1
        /// </summary>
        /// <returns>True if connectivity is successful, else False</returns>
        public static bool CheckOnlineUsingWAS()
        {
            try
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient("10.122.15.17", 9001);
                client.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        } 
        #endregion

        #region Requests
        /// <summary>
        /// Return current time on DB
        /// </summary>
        /// <returns>Datetime string in YYYYMMDDHH24MISS format</returns>
        public static string GetServerNow()
        {
            string sSQL = "";
            sSQL += " SELECT TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') NOW ";
            sSQL += " FROM DUAL ";
            return RequestOneItem(sSQL);
        }

        /// <summary>
        /// Return result of SQL query in a string
        /// </summary>
        /// <param name="SQL">SQL query text</param>
        /// <returns>Resultset string with delimeter '|'</returns>
        public static string Request(string SQL)
        {
            try
            {
                string res = "";
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand Command = myConn.CreateCommand();
                    myConn.Open();
                    Command.CommandText = SQL;
                    using (OracleDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            for (int i = 0; i < Reader.FieldCount; i++)
                            {
                                res += Reader.GetValue(i).ToString() + "|";
                            }
                        }
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Error to execute: " + SQL);
                return null;
            }
        }

        /// <summary>
        /// Return result of SQL query, which returns only 1 value
        /// </summary>
        /// <param name="SQL">SQL query text</param>
        /// <returns>Result value</returns>
        public static string RequestOneItem(string SQL)
        {
            try
            {
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand Command = myConn.CreateCommand();
                    myConn.Open();
                    Command.CommandText = SQL;
                    return Convert.ToString(Command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Error to execute: " + SQL);
                return null;
            }
        }

        /// <summary>
        /// Return result of SQL query in a string queue
        /// </summary>
        /// <param name="SQL">SQL query text</param>
        /// <returns>Resultset as queue of strings</returns>
        public static Queue<string> RequestQueue(string SQL, params Parameter[] parameters)
        {
            try
            {
                Queue<string> res = new Queue<string>();
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand Command = myConn.CreateCommand();
                    myConn.Open();
                    Command.CommandText = SQL;
                    foreach (Parameter item in parameters)
                        Command.Parameters.Add(item.OracleParameter);
                    using (OracleDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            for (int i = 0; i < Reader.FieldCount; i++)
                            {
                                res.Enqueue(Reader.GetValue(i).ToString());
                            }
                        }
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Error to execute: " + SQL);
                return null;
            }
        }

        /// <summary>
        /// Execute command without retrieveing result (procedures, functions)
        /// </summary>
        /// <param name="command">Oracle command object with command text and parameters</param>
        /// <returns>Custom Response object with execution results (based on O_CD and O_MSG parameters)</returns>
        public static Response Request(OracleCommand command)
        {
            Response res = new Response();
            try
            {
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    command.Connection = myConn;
                    myConn.Open();
                    command.ExecuteNonQuery();
                    res.Code = command.Parameters.IndexOf("O_CD") >= 0 ? command.Parameters["O_CD"].Value.ToString() : "";
                    res.Message = command.Parameters.IndexOf("O_MSG") >= 0 ? command.Parameters["O_MSG"].Value.ToString() : "";
                    res.Parameters = command.Parameters;
                }
                return res;
            }
            catch (OracleException oex)
            {
                res.Exception = oex;
                Log.Write(LogType.ERROR, oex, "DB Execution error");
                return res;
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Error to execute: " + command.CommandText);
                return res;
            }
        }

        /// <summary>
        /// Execute command without retrieveing result (procedures, functions)
        /// </summary>
        /// <param name="commandText">Name of function/procedure</param>
        /// <param name="parameters">Array of Parameter values</param>
        /// <returns>Custom Response object with execution results (based on O_CD and O_MSG parameters)</returns>
        public static Response Request(string commandText, params Parameter[] parameters)
        {
            Response res = new Response();
            try
            {
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand comm = new OracleCommand(commandText, myConn) { CommandType = CommandType.StoredProcedure };
                    foreach (Parameter item in parameters)
                        comm.Parameters.Add(item.OracleParameter);
                    myConn.Open();
                    comm.ExecuteNonQuery();
                    res.Code = comm.Parameters.IndexOf("O_CD") >= 0 ? comm.Parameters["O_CD"].Value.ToString() : "";
                    res.Message = comm.Parameters.IndexOf("O_MSG") >= 0 ? comm.Parameters["O_MSG"].Value.ToString() : "";
                    res.Parameters = comm.Parameters;
                }
                return res;
            }
            catch (OracleException oex)
            {
                res.Exception = oex;
                Log.Write(LogType.ERROR, oex, "DB Execution error");
                return res;
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Other error");
                return res;
            }
        }

        /// <summary>
        /// Execute command without retrieveing result (procedures, functions)
        /// </summary>
        /// <param name="commandText">Name of function/procedure</param>
        /// <param name="parameters">Array of Parameter values</param>
        /// <returns>DataTable with data from Ref Cursor</returns>
        public static DataTable RequestRefCursor(string cmdText, params Parameter[] parameters)
        {
            try
            {
                DataTable dt = new DataTable();
                Response res = new Response();
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand comm = new OracleCommand(cmdText, myConn) { CommandType = CommandType.StoredProcedure };
                    foreach (Parameter item in parameters)
                        comm.Parameters.Add(item.OracleParameter);
                    myConn.Open();
                    comm.ExecuteNonQuery();
                    res.Code = comm.Parameters.IndexOf("O_CD") >= 0 ? comm.Parameters["O_CD"].Value.ToString() : "";
                    res.Message = comm.Parameters.IndexOf("O_MSG") >= 0 ? comm.Parameters["O_MSG"].Value.ToString() : "";
                    res.Parameters = comm.Parameters;

                    foreach (OracleParameter item in res.Parameters)
                    {
                        if (item.OracleDbType == OracleDbType.RefCursor)
                        {
                            using (OracleDataReader dr = ((OracleRefCursor)item.Value).GetDataReader())
                            {
                                dt.Load(dr);
                            }
                            return dt;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Other error");
                return null;
            }
        }

        /// <summary>
        /// Return result of SQL query in a string queue
        /// </summary>
        /// <param name="SQL">SQL query text</param>
        /// <returns>Resultset as data table</returns>
        public static DataTable RequestDataTable(string SQL, params Parameter[] parameters)
        {
            try
            {
                using (OracleConnection myConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand Command = myConn.CreateCommand();
                    myConn.Open();
                    Command.CommandText = SQL;
                    foreach (Parameter item in parameters)
                        Command.Parameters.Add(item.OracleParameter);
                    using (var da = new OracleDataAdapter(Command))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Error to execute: " + SQL);
                return null;
            }
        }
        #endregion

    }

    /// <summary>
    /// Class define response from DB after execution.
    /// Uses O_CD and O_MSG output parameters.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Collection of error codes, which represents as good.
        /// </summary>
        private List<string> goodResponses;

        /// <summary>
        /// Parameters of executed NonQuery command.
        /// </summary>
        public OracleParameterCollection Parameters
        {
            get; set;
        }

        /// <summary>
        /// Get or sets code of error.
        /// </summary>
        public string Code
        {
            get; set;
        }

        /// <summary>
        /// Get or sets description message of error.
        /// </summary>
        public string Message
        {
            get; set;
        }

        /// <summary>
        /// Get information as bool, is this response is 'good' or not.
        /// </summary>
        public bool IsGood
        {
            get
            {
                if (Exception != null)
                    return false;
                foreach (string it in goodResponses)
                {
                    if (it.Equals(Code))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Exception of execution, if exists
        /// </summary>
        public OracleException Exception
        {
            get; set;
        }
        
        /// <summary>
        /// Constructor. Adds default code 'Y' which represents as good.
        /// </summary>
        public Response()
        {
            goodResponses = new List<string> { "Y" };
        }

        /// <summary>
        /// Constructor with Code/Message init.
        /// </summary>
        /// <param name="code">Code of error</param>
        /// <param name="message">Description of error</param>
        public Response(string code, string message) : this()
        {        
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Adds custom error code which represents as good.
        /// </summary>
        /// <param name="code">New 'good' code</param>
        public void AddGoodCode(string code)
        {
            goodResponses.Add(code);
        }
    }

    /// <summary>
    /// Class represent functionality of OracleParameter.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; private set;}

        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Oracle type of value (based on object type)
        /// </summary>
        public OracleDbType Type { get; private set; }

        /// <summary>
        /// Parameter direction specificator string: IN, OUT, IN/OUT, RETURN
        /// </summary>
        public ParameterDirection Direction { get; private set; }

        /// <summary>
        /// Returns original OracleParameter object based on Parameter instance
        /// </summary>
        public OracleParameter OracleParameter
        {
            get
            {
                return new OracleParameter(Name, Type, (Value is string) ? 1000 : 20, Value, Direction);
            }
        }

        /// <summary>
        /// Constructor to create Parameter instance
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <param name="parameterDirection">Parameter direction specificator string: IN, OUT, IN/OUT, RETURN</param>
        public Parameter(string parameterName, object parameterValue, string parameterDirection = "IN")
        {
            Name = parameterName;
            Value = parameterValue;
            Type = GetOracleDbType(Value);
            
            string tmp = parameterDirection.ToUpper().Trim();
            if (tmp.Contains("IN"))
                if (tmp.Contains("OUT"))
                {
                    Direction = ParameterDirection.InputOutput;
                    return;
                }
                else
                {
                    Direction = ParameterDirection.Input;
                    return;
                }
            if (tmp.Contains("OUT"))
            {
                Direction = ParameterDirection.Output;
                return;
            }
            if (tmp.Contains("RETURN"))
            {
                Direction = ParameterDirection.ReturnValue;
                return;
            }
            throw new ArgumentException("Bad parameter direction specificator");
        }

        /// <summary>
        /// Get Oracle type of object.
        /// </summary>
        /// <param name="o">Parameter value</param>
        /// <returns>OracleDbType instance for specified object</returns>
        private static OracleDbType GetOracleDbType(object o) 
        {
            if (o is string && (o as string).ToUpper() == "REFCURSOR") return OracleDbType.RefCursor;
            if (o is string) return OracleDbType.Varchar2;
            if (o is DateTime) return OracleDbType.Date;
            if (o is Int64) return OracleDbType.Int64;
            if (o is Int32) return OracleDbType.Int32;
            if (o is Int16) return OracleDbType.Int16;
            if (o is sbyte) return OracleDbType.Byte;
            if (o is byte) return OracleDbType.Int16;
            if (o is decimal) return OracleDbType.Decimal;
            if (o is float) return OracleDbType.Single;
            if (o is double) return OracleDbType.Double;
            if (o is byte[]) return OracleDbType.Blob;

            return OracleDbType.Varchar2;
        }
    }
}
