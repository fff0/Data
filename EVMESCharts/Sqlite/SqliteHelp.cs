using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace EVMESCharts.Sqlite
{ /// <summary>
  /// 数据库操作类
  /// </summary>
    public static class SQLiteHelp
    {
        #region 数据库路径  创建数据库
        /// <summary>
        /// 文件路径  ~\Charts\bin\Debug\SQLiteDB
        /// </summary>
        static readonly string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQLiteDB");

        /// <summary>
        /// 数据库文件路径
        /// </summary>
        static readonly string DbPath = Path.Combine(SavePath, "Data.db");

        /// <summary>
        /// 连接数据库路径格式 
        /// </summary>
        public static string ConnectionString = $"DataSource = {DbPath}; Version = 3";

        /// <summary>
        /// 判断是否存在数据库文件
        /// </summary>
        public static void FindDBPath()
        {
            //查找文件路径
            if (!System.IO.File.Exists(DbPath))
            {
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);  //创建文件夹
                }
                try
                {
                    //创建数据库文件
                    SQLiteConnection.CreateFile(DbPath);
                    //创建数据表
                    NewTable(DbPath, "DayDeviceData");
                    NewTable(DbPath, "FaultData");
                    NewTable(DbPath, "MonthDeviceData");
                    NewTable(DbPath, "WarningData");
                    NewTable(DbPath, "StandardData");
                    NewTable(DbPath, "IPData");
                    NewTable(DbPath, "Equipment");
                }
                catch (Exception ex)
                {
                    throw new Exception("新建数据库文件" + DbPath + "失败：" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <param name="dbPath">指定数据库文件</param>
        /// <param name="tableName">表名称</param>
        static public void NewTable(string dbPath, string tableName)
        {
            //数据库连接
            SQLiteConnection sqliteConn = new SQLiteConnection("data source=" + dbPath);
            if (sqliteConn.State != ConnectionState.Open)
            {
                sqliteConn.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = sqliteConn
                };
                //创建数据库表
                switch (tableName)
                {
                    case "DayDeviceData":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(Day string, Time string, DevID int, Produce int, TimeEfficiency double, Electric double, Gas double, GoodProduct int)";
                        break;
                    case "FaultData":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(Day string, Time string, FaultID int, Content int)";
                        break;
                    case "MonthDeviceData":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(Day string, DevID int, Produce int, TimeEfficiency double, Electric double, Gas double, GoodProduct int)";
                        break;
                    case "WarningData":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(Day string, Time string, DevID int, Content int, ReachRate string)";
                        break;
                    case "StandardData":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(Day string, Capacity int, DayPlan int, MonthPlan int)";
                        break;
                    case "IPData":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(IP1 int, IP2 int, IP3 int, IP4 int, Port int)";
                        break;
                    case "Equipment":
                        cmd.CommandText = "CREATE TABLE " + tableName + "(ID int, Name string)";
                        break;
                    default:
                        break;
                }
                cmd.ExecuteNonQuery();
            }
            sqliteConn.Close();
        }
        #endregion

        #region 操作数据库
        
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        public static void DeleteSql(string sqlStr)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, sqlStr);
                    //事务处理
                    SQLiteTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        /// <summary>
        /// 查询 返回为DataTable
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <returns>输出DataTable表格</returns>
        public static DataTable ExecuteQuery(string sqlStr)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {

                    DataTable dt = new DataTable();
                    try
                    {
                        PrepareCommand(command, conn, sqlStr);
                        SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                        da.Fill(dt); // 生成表格数据
                        return dt;
                    }
                    catch (Exception)
                    {
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 插入数据到数据库
        /// </summary>
        /// <param name="sql">数据库语句</param>
        /// <returns>bool值</returns>
        public static bool SQLInsert(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();  // 连接数据库
                using (System.Data.SQLite.SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        //事务处理
                        cmd.Transaction = trans;
                        try
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="sql">数据库语句</param>
        public static void SQLUpdate(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();  // 连接数据库
                using (System.Data.SQLite.SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        //事务处理
                        cmd.Transaction = trans;
                        try
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 参数设置
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn">数据库</param>
        /// <param name="sqlStr">查询语句</param>
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string sqlStr)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.Parameters.Clear();
                cmd.Connection = conn;
                cmd.CommandText = sqlStr;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        #region 输出数据暂存表格数据到控制台 调试用

        /// <summary>
        /// 输出表格数据到控制台 调试
        /// </summary>
        /// <param name="table">表格名</param>
        public static void PrintTable(DataTable table)
        {
            PrintLine(12 * table.Columns.Count);
            foreach (DataColumn col in table.Columns)
            {
                Console.Write(string.Format("{0,12}", col.Caption));
            }
            Console.Write("\n");
            PrintLine(12 * table.Columns.Count);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    Console.Write(string.Format("{0,12}", table.Rows[i][j].ToString()));
                }
                Console.Write("\n");
            }
            PrintLine(12 * table.Columns.Count, "-");
        }

        /// <summary>
        /// 表格格式
        /// </summary>
        /// <param name="length">总长</param>
        /// <param name="lineChar">分割符号</param>
        private static void PrintLine(int length, string lineChar = "=")
        {
            string line = string.Empty;
            for (int i = 0; i < length; i++)
            {
                line += lineChar;
            }
            Console.WriteLine(line);
        }
        #endregion


        /// <summary>
        /// 获取DataTable中数据种类
        /// </summary>
        /// <param name="dt">数据库表</param>
        /// <param name="Name">统计Name的种类</param>
        /// <returns> List </returns>
        public static List<byte> NumberList (DataTable dt,string Name)
        {
            List<byte> NumList = new List<byte>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var id = dt.Rows[i][$"{Name}"].ToString();
                if (!NumList.Contains(byte.Parse(id)))
                {
                    NumList.Add(byte.Parse(id));
                }
            }
            return NumList;
        }
    }
}
