using EVMESCharts.Sqlite;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace EVMESCharts.ModBus
{
    class ModBusHelp
    {
        /// <summary>
        /// ip地址
        /// </summary>
        public static string IPaddress
        {
            get
            {
                string sql = "SELECT * FROM IPData";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                // 判断是否有IP地址数据
                if (data.Rows.Count > 0)
                {
                    byte ip1 = byte.Parse(data.Rows[data.Rows.Count - 1][0].ToString());
                    byte ip2 = byte.Parse(data.Rows[data.Rows.Count - 1][1].ToString());
                    byte ip3 = byte.Parse(data.Rows[data.Rows.Count - 1][2].ToString());
                    byte ip4 = byte.Parse(data.Rows[data.Rows.Count - 1][3].ToString());
                    IPAddress address = new IPAddress(new byte[] { ip1, ip2, ip3, ip4 });
                    return address.ToString();
                }
                else
                {
                    return "127.0.0.1";
                }
            }
        }

        /// <summary>
        /// 端口号
        /// </summary>
        public static int Port
        {
            get
            {
                string sql = "SELECT Port FROM IPData";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                // 判断是否有IP地址数据
                if (data.Rows.Count > 0)
                {
                    int port = int.Parse(data.Rows[data.Rows.Count - 1][0].ToString());
                    return port;
                }
                else
                {
                    return 502;
                }
            }
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        public static TcpClient Client
        {
            get
            {
                return new TcpClient(IPaddress, Port);
            }
        }

        /// <summary>
        /// 读取03 Holding Regyster (4x) AO的值
        /// </summary>
        /// <param name="address">IP</param>
        /// <param name="port">端口号</param>
        /// <param name="SlaveID">从站地址 (8位)</param>
        /// <param name="startAddress">起始地址 (16位)</param>
        /// <param name="numInputs">读取数量 (16位)</param>
        /// <returns>List</returns>
        public static List<int> HoldingRegister(ushort startAddress, ushort numInputs, byte SlaveID = 1)
        {
            List<int> HoldingList = new List<int>();
            Client.SendTimeout = 1;
            ModbusIpMaster master = ModbusIpMaster.CreateIp(Client);
            try
            {
                ushort[] inputs = master.ReadHoldingRegisters(SlaveID, startAddress, numInputs);
                for (int i = 0; i < numInputs; i++)
                {
                    // 将读取出的数据存储到List中
                    HoldingList.Add(inputs[i]);
                }
                return HoldingList;
            }
            catch (Exception)
            {
                Client.Close();
                return new List<int>() { -99 };
            }
        }


        #region 记录生产数据
        /// <summary>
        /// 当天当前时间之前的数据和
        /// </summary>
        public static int YieldSum
        {
            get;
            set;
        }

        /// <summary>
        /// 前一天8点到24点的数据和
        /// </summary>
        public static int YesterDaySum
        {
            get;
            set;
        }

        /// <summary>
        /// 当前天当前小时前一个小时到当天8点的数据
        /// </summary>
        public static int ToDayYieldData
        {
            get;
            set;
        }

        /// <summary>
        /// 当前小时的数据
        /// </summary>
        public static int NowYieldData
        {
            get;
            set;
        }

        /// <summary>
        /// 当日的生产总数据
        /// </summary>
        public static int ToDaySum
        {
            get;
            set;
        }

        /// <summary>
        /// 每台设备记录的信息条数
        /// </summary>
        public static List<int> DevValueLengthList
        {
            get;
            set;
        } = new List<int>();
        #endregion

        #region 记录故障数据

        #region 当前小时故障数据
        /// <summary>
        /// 小时A柜故障次数
        /// </summary>
        public static int HourAFault
        {
            get;
            set;
        }
        /// <summary>
        /// 小时B柜故障次数
        /// </summary>
        public static int HourBFault
        {
            get;
            set;
        }
        /// <summary>
        /// 小工位三故障次数
        /// </summary>
        public static int HourCFault
        {
            get;
            set;
        }
        /// <summary>
        /// 小时工位四故障次数
        /// </summary>
        public static int HourDFault
        {
            get;
            set;
        }
        /// <summary>
        /// 小时工位五故障次数
        /// </summary>
        public static int HourEFault
        {
            get;
            set;
        }
        #endregion

        #region 当前天当前小时前一个小时到当天8点的故障数据
        /// <summary>
        /// 当前天当前小时前一个小时到当天8点的A柜故障次数
        /// </summary>
        public static int ToDayAFaultData
        {
            get;
            set;
        }
        /// <summary>
        /// 当前天当前小时前一个小时到当天8点的B柜故障次数
        /// </summary>
        public static int ToDayBFaultData
        {
            get;
            set;
        }
        /// <summary>
        /// 当前天当前小时前一个小时到当天8点的工位三故障次数
        /// </summary>
        public static int ToDayCFaultData
        {
            get;
            set;
        }
        /// <summary>
        /// 当前天当前小时前一个小时到当天8点的工位四故障次数
        /// </summary>
        public static int ToDayDFaultData
        {
            get;
            set;
        }
        /// <summary>
        /// 当前天当前小时前一个小时到当天8点的工位五故障次数
        /// </summary>
        public static int ToDayEFaultData
        {
            get;
            set;
        }
        #endregion

        #region 当天当前时间之前的故障次数
        /// <summary>
        /// 当天当前时间之前的A柜故障次数
        /// </summary>
        public static int AFaultSum
        {
            get;
            set;
        }
        /// <summary>
        /// 当天当前时间之前的B柜故障次数
        /// </summary>
        public static int BFaultSum
        {
            get;
            set;
        }
        /// <summary>
        /// 当天当前时间之前的工位三故障次数
        /// </summary>
        public static int CFaultSum
        {
            get;
            set;
        }
        /// <summary>
        /// 当天当前时间之前的工位四故障次数
        /// </summary>
        public static int DFaultSum
        {
            get;
            set;
        }
        /// <summary>
        /// 当天当前时间之前的工位五故障次数
        /// </summary>
        public static int EFaultSum
        {
            get;
            set;
        }
        #endregion

        #region 前一天8点到24点的故障数据和
        /// <summary>
        /// 前一天8点到24点的A柜故障数据和
        /// </summary>
        public static int YesterDayAFault
        {
            get;
            set;
        }
        /// <summary>
        /// 前一天8点到24点的B柜故障数据和
        /// </summary>
        public static int YesterDayBFault
        {
            get;
            set;
        }
        /// <summary>
        /// 前一天8点到24点的工位三数据和
        /// </summary>
        public static int YesterDayCFault
        {
            get;
            set;
        }
        /// <summary>
        /// 前一天8点到24点的工位四数据和
        /// </summary>
        public static int YesterDayDFault
        {
            get;
            set;
        }
        /// <summary>
        /// 前一天8点到24点的工位五数据和
        /// </summary>
        public static int YesterDayEFault
        {
            get;
            set;
        }

        #endregion

        #region 当日的故障次数
        /// <summary>
        /// 当日的A柜故障次数
        /// </summary>
        public static int ToDayAFault
        {
            get;
            set;
        }
        /// <summary>
        /// 当日的B柜故障次数
        /// </summary>
        public static int ToDayBFault
        {
            get;
            set;
        }
        /// <summary>
        /// 当日的工位三故障次数
        /// </summary>
        public static int ToDayCFault
        {
            get;
            set;
        }
        /// <summary>
        /// 当日的工位四故障次数
        /// </summary>
        public static int ToDayDFault
        {
            get;
            set;
        }
        /// <summary>
        /// 当日的工位五故障次数
        /// </summary>
        public static int ToDayEFault
        {
            get;
            set;
        }
        #endregion

        #endregion

        /// <summary>
        /// 读取ModBus数据
        /// </summary>
        /// <param name="address">ModBus IP地址</param>
        /// <param name="runport">端口号</param>
        /// <param name="ID">读取的设备编号ID</param>
        public static void ReadModBus(int ID = 0)
        {
            //获取当前天
            var NowDay = DateTime.Today.Date.ToShortDateString();
            // 获取当前小时
            int Hour = int.Parse(DateTime.Now.Hour.ToString());

            // ModBus每天8点清空数据
            // 获取当前天数
            int nowday = int.Parse(DateTime.Now.Day.ToString());
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();
            // 获取到前一天
            string yesterday = $"{nowyear}/{nowmonth}/{nowday - 1}";

            //--------------读取故障数据------------------
            GetFault(NowDay,Hour,yesterday);
            // 获取前一天8点到24点的数据和
            string yesterdaySql = $"SELECT sum(Produce) FROM DayDeviceData WHERE Day = '{yesterday}' AND DevID = {ID} AND  Time > {8}";
            DataTable YesterDayData = SQLiteHelp.ExecuteQuery(yesterdaySql);
            // 有数据
            if (YesterDayData.Rows[0][0].ToString() != "")
            {
                YesterDaySum = int.Parse(YesterDayData.Rows[0][0].ToString());
            }
            else
            {
                YesterDaySum = 0;
            }

            // 获取当前小时之前的数据
            string BeforeSql = $"SELECT sum(Produce) FROM DayDeviceData WHERE Day = '{NowDay}' AND DevID = {ID} AND  Time < {Hour}";
            DataTable BeforeData = SQLiteHelp.ExecuteQuery(BeforeSql);
            // 判断是否存在数据
            if (BeforeData.Rows[0][0].ToString() != "")
            {
                YieldSum = int.Parse(BeforeData.Rows[0][0].ToString());
            }
            else
            {
                YieldSum = 0;
            }

            // 获取当前天当前小时前一个小时到当天8点的数据
            string TodaySql = $"SELECT sum(Produce) FROM DayDeviceData WHERE Day = '{NowDay}' AND DevID = {ID} AND  Time >= {8} AND Time <{Hour}";
            DataTable ToDayData = SQLiteHelp.ExecuteQuery(TodaySql);

            if (ToDayData.Rows[0][0].ToString() != "")
            {
                ToDayYieldData = int.Parse(ToDayData.Rows[0][0].ToString());
            }
            else
            {
                ToDayYieldData = 0;
            }

            // 获取当天的生产的总数据
            string Todaysumsql = $"SELECT sum(Produce) FROM DayDeviceData WHERE Day = '{NowDay}' AND DevID = {ID}";
            DataTable TodaySumData = SQLiteHelp.ExecuteQuery(Todaysumsql);
            if (TodaySumData.Rows[0][0].ToString() != "")
            {
                ToDaySum = int.Parse(TodaySumData.Rows[0][0].ToString());
            }
            else
            {
                ToDaySum = 0;
            }

            //------------------------------------------------------------------------------------------------------------------------
            //读取时间稼动率 D5975
            List<int> Produce = ModBusHelp.HoldingRegister(0, 1);
            // 获取到时间稼动率
            int TimeEfficiency = Produce[0];
            //------------------------------------------------------------------------------------------------------------------------
            //读取当前产量 D7000
            //List<int> YieldList = ModBusHelp.HoldingRegister(1, 1);
            // 获取当前产量
            //int Yield = YieldList[0];
            //------------------------------------------------------------------------------------------------------------------------
            // 读取故障总次数 D5071，D5171, D5271, D5371, D5471
            List<int> AFaultList = ModBusHelp.HoldingRegister(2, 1);     // 工位一故障次数
            List<int> BFaultList = ModBusHelp.HoldingRegister(3, 1);     // 工位二故障次数
            List<int> CFaultList = ModBusHelp.HoldingRegister(4, 1);     // 工位三故障次数
            List<int> DFaultList = ModBusHelp.HoldingRegister(5, 1);     // 工位四故障次数
            List<int> EFaultList = ModBusHelp.HoldingRegister(6, 1);     // 工位五故障次数
            int AFault = AFaultList[0];
            int BFault = BFaultList[0];
            int CFault = CFaultList[0];
            int DFault = DFaultList[0];
            int EFault = EFaultList[0];
            //------------------------------------------------------------------------------------------------------------------------
            // 读取详细故障数据 DetailedFault 表
            // 读取工位一详细故障数据 D5060 - D5070
            List<int> WorkStationOne = ModBusHelp.HoldingRegister(10, 11);
            // 读取工位二详细故障数据 D5160 - D5170
            List<int> WorkStationTwo = ModBusHelp.HoldingRegister(20, 11);
            // 读取工位三详细故障数据 D5260 - D5265
            List<int> WorkStationThree = ModBusHelp.HoldingRegister(40, 6);
            // 读取工位四详细故障数据 D5360 - D5366
            List<int> WorkStationFour = ModBusHelp.HoldingRegister(50, 7);
            // 读取工位五详细故障数据 D5460 - D5467
            List<int> WorkStationFive = ModBusHelp.HoldingRegister(60, 8);
            //------------------------------------------------------------------------------------------------------------------------
            // 读取上电时间  秒/分/小时 D5960 - D5962
            List<int> PowerTime = ModBusHelp.HoldingRegister(70, 3);
            // 读取运行时间 秒/分/小时 D5930 - D5932
            List<int> RunTime = ModBusHelp.HoldingRegister(80, 3);
            // 读取全线停机时间 秒/分/小时 D5940 - D5942
            List<int> StopTime = ModBusHelp.HoldingRegister(90, 3);
            // 读取全线故障时间 秒/分/小时 D5950 - D5952
            List<int> FaultTime = ModBusHelp.HoldingRegister(100, 3);
            //------------------------------------------------------------------------------------------------------------------------
            //读取小时产量列表 D7010-D7033
            List<int> HourProduceList = ModBusHelp.HoldingRegister(110, 24);
            // 读取小时达成率 D7210-D7233
            List<int> HourReachList = ModBusHelp.HoldingRegister(140, 24);
            //------------------------------------------------------------------------------------------------------------------------
            // 读取每个故障的持续时间
            List<int> FaultTimtList = ModBusHelp.HoldingRegister(150, 42);


            /*************************************************************存储生产数据************************************************************************/
            #region 存储每小时数据，达成率
            for (int i = 0; i <= Hour; i++)
            {
                string hoursql = $"SELECT 1 FROM DayDeviceData WHERE Day = '{NowDay}' AND Time = {i}";
                DataTable HourData = SQLiteHelp.ExecuteQuery(hoursql);
                //如果没有生产数据，将时间稼动率设为0
                if (HourProduceList[i] == 0)
                {
                    TimeEfficiency = 0;
                }
                else
                {
                    TimeEfficiency = Produce[0];
                }
                if (HourData.Rows.Count > 0)
                {
                    //更新数据库数据
                    string hourupdatasql = $"UPDATE DayDeviceData SET Reach = {HourReachList[i]},Produce = {HourProduceList[i]} WHERE Day = '{NowDay}' AND Time = {i} AND DevID = {ID}";
                    SQLiteHelp.SQLUpdate(hourupdatasql);
                }
                else
                {
                    // 插入数据
                    string hourinsertsql = $"INSERT INTO DayDeviceData VALUES ('{NowDay}',{i},{ID},{HourProduceList[i]},{HourReachList[i]},{TimeEfficiency},{0},{0},{0})";
                    SQLiteHelp.SQLInsert(hourinsertsql);
                    
                    if (HourReachList[i - 1] < 100)
                    {
                        // 获取上一个小时的数据，添加日志信息
                        string sql = $"INSERT INTO WarningData VALUES ('{NowDay}',{i - 1},{ID},{0},{(double)HourReachList[i - 1] / 100})";
                        SQLiteHelp.SQLInsert(sql);
                    }
                }

                // 修改当前小时的稼动率数据
                if (i == Hour)
                {
                    //更新数据库数据
                    string hourupdatasql = $"UPDATE DayDeviceData SET TimeEfficiency = {TimeEfficiency} WHERE Day = '{NowDay}' AND Time = {i} AND DevID = {ID}";
                    SQLiteHelp.SQLUpdate(hourupdatasql);

                }
            }

            #endregion

            #region 存储生产数据和工位故障数据
            // 当当前小时小于8时
            if (Hour < 8)
            {
                // 当当前小时数小于8时，ModBus数据还未清空
                // 当前小时的数据 = 读取出来的ModBus数据 - 前一天8点到24点的数据 - 当前时间之前的数据
                //NowYieldData = Yield - YesterDaySum - YieldSum;
                HourAFault = AFault - YesterDayAFault - AFaultSum;
                HourBFault = BFault - YesterDayBFault - BFaultSum;
                HourCFault = CFault - YesterDayCFault - CFaultSum;
                HourDFault = DFault - YesterDayDFault - DFaultSum;
                HourEFault = EFault - YesterDayEFault - EFaultSum;
            }
            else if (Hour == 8)
            {
                // 当前小时为8，ModBus数据刚清空，读取出的数据就是当前小时的产量数据
                //NowYieldData = Yield;
                HourAFault = AFault;
                HourBFault = BFault;
                HourCFault = CFault;
                HourDFault = DFault;
                HourEFault = EFault;
            }
            else
            {
                // 如果当前小时大于8小时，ModBus数据已经清空了
                // 当前小时数据 = 读取出来的数据 - 当前天当前小时前一个小时到当天8点的数据
                //NowYieldData = Yield - ToDayYieldData;
                HourAFault = AFault - ToDayAFaultData;
                HourBFault = BFault - ToDayBFaultData;
                HourCFault = CFault - ToDayCFaultData;
                HourDFault = DFault - ToDayDFaultData;
                HourEFault = EFault - ToDayEFaultData;
            }

            // 判断是否读取到数据
            //if (Yield >= 0 && TimeEfficiency >=0)
            //{
            //    // 查询当前时间下数据库中是否有数据
            //    string querydaysql = $"SELECT 1 FROM DayDeviceData WHERE Day = '{NowDay}' AND Time = {Hour} AND DevID = {ID}";
            //    DataTable HourData = SQLiteHelp.ExecuteQuery(querydaysql);

            //    if (HourData.Rows.Count > 0)
            //    {
            //        // 更新数据库
            //        string updatasql = $"UPDATE DayDeviceData SET TimeEfficiency = {TimeEfficiency},Produce = {NowYieldData} WHERE Day = '{NowDay}' AND Time = {Hour} AND DevID = {ID}";
            //        SQLiteHelp.SQLUpdate(updatasql);
            //    }
            //    else
            //    {
            //        // 插入数据（只在当前小时无数据时运行）
            //        string insertsql = $"INSERT INTO DayDeviceData VALUES ('{NowDay}',{Hour},{ID},{NowYieldData},{TimeEfficiency},{0},{0},{0})";
            //        SQLiteHelp.SQLInsert(insertsql);

            //        // 插入数据时，刚好是整点，读取上一个小时的数据，判断是否达到标准产能
            //        int NowHour = int.Parse(DateTime.Now.Hour.ToString());

            //        // 当当前小时不是 0 点时
            //        if (NowHour - 1 > 0)
            //        {
            //            string beforesql = $"SELECT Produce FROM DayDeviceData WHERE Day = '{NowDay}' AND DevID = {ID} AND Time = {NowHour - 1}";
            //            // 得到上一个小时的数据
            //            DataTable BeforeHourData = SQLiteHelp.ExecuteQuery(beforesql);
            //            // 判断是否存在数据
            //            if (BeforeHourData.Rows.Count > 0)
            //            {
            //                // 前一个小时的数据
            //                double HourValue = double.Parse(BeforeHourData.Rows[0][0].ToString());
            //                // 判断是否达到标准产能
            //                if (HourValue < MainWindow.HourPlan)
            //                {
            //                    string Data = (HourValue / MainWindow.HourPlan).ToString("N2");
            //                    // 添加信息    （未达成指定产量目标）
            //                    int content = 0;
            //                    string sql = $"INSERT INTO WarningData VALUES ('{NowDay}',{NowHour - 1},{ID},{content},{Data})";
            //                    SQLiteHelp.SQLInsert(sql);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            // 当前小时是零点 ，则读取前一天 23 点的数据
            //            string yesterdaysql = $"SELECT Produce FROM DayDeviceData WHERE Day = '{yesterday}' AND DevID = {ID} AND Time = {23}";
            //            // 获取到前一天23点的数据
            //            DataTable YesterDayHourData = SQLiteHelp.ExecuteQuery(yesterdaysql);
            //            // 判断是否存在数据
            //            if (YesterDayHourData.Rows.Count > 0)
            //            {
            //                // 前一天23小时的数据
            //                int YesterDayHourValue = int.Parse(YesterDayHourData.Rows[0][0].ToString());
            //                if (YesterDayHourValue < MainWindow.HourPlan)
            //                {
            //                    int content = 0;
            //                    string sql = $"INSERT INTO WarningData VALUES ('{yesterday}',{23},{ID},{content})";
            //                    SQLiteHelp.SQLInsert(sql);
            //                }
            //            }
            //        }
            //    }

            // 更新月数据库
            // 查询月数据下是否包含当日数据
            string querymonthsql = $"SELECT * FROM MonthDeviceData WHERE Day = '{NowDay}' AND DevID = {ID}";
            DataTable MonthData = SQLiteHelp.ExecuteQuery(querymonthsql);

            // 判断是否有当日数据  有==>更新   无==>插入数据
            if (MonthData.Rows.Count > 0)
            {
                // 更新数据库
                string updatasql = $"UPDATE MonthDeviceData SET TimeEfficiency = {TimeEfficiency},Produce = {ToDaySum} WHERE Day = '{NowDay}' AND DevID = {ID}";
                SQLiteHelp.SQLUpdate(updatasql);
            }
            else
            {
                // 插入数据
                string insertsql = $"INSERT INTO MonthDeviceData VALUES ('{NowDay}',{ID},{ToDaySum},{TimeEfficiency},{0},{0},{0})";
                SQLiteHelp.SQLInsert(insertsql);
            }
            //}

            // 添加A柜故障数数据
            if (AFault >= 0)
            {
                // 查询当前时间下数据库中是否有数据
                string Afaultsql = $"SELECT 1 FROM FaultData WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {0}";
                DataTable AData= SQLiteHelp.ExecuteQuery(Afaultsql);
                if (AData.Rows.Count > 0)
                {
                    // 更新数据库
                    string updatasql = $"UPDATE FaultData SET Content = {HourAFault} WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {0}";
                    SQLiteHelp.SQLUpdate(updatasql);
                }
                else
                {
                    // 插入数据（只在当前小时无数据时运行）
                    string insertsql = $"INSERT INTO FaultData VALUES ('{NowDay}',{Hour},{0},{HourAFault})";
                    SQLiteHelp.SQLInsert(insertsql);
                }
            }
            // 添加B柜故障数数据
            if (BFault >= 0)
            {
                // 查询当前时间下数据库中是否有数据
                string Bfaultsql = $"SELECT 1 FROM FaultData WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {1}";
                DataTable BData = SQLiteHelp.ExecuteQuery(Bfaultsql);
                if (BData.Rows.Count > 0)
                {
                    // 更新数据库
                    string updatasql = $"UPDATE FaultData SET Content = {HourBFault} WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {1}";
                    SQLiteHelp.SQLUpdate(updatasql);
                }
                else
                {
                    // 插入数据（只在当前小时无数据时运行）
                    string insertsql = $"INSERT INTO FaultData VALUES ('{NowDay}',{Hour},{1},{HourBFault})";
                    SQLiteHelp.SQLInsert(insertsql);
                }
            }
            // 添加工位三故障数数据
            if (CFault >= 0)
            {
                // 查询当前时间下数据库中是否有数据
                string Cfaultsql = $"SELECT 1 FROM FaultData WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {2}";
                DataTable CData = SQLiteHelp.ExecuteQuery(Cfaultsql);
                if (CData.Rows.Count > 0)
                {
                    // 更新数据库
                    string updatasql = $"UPDATE FaultData SET Content = {HourCFault} WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {2}";
                    SQLiteHelp.SQLUpdate(updatasql);
                }
                else
                {
                    // 插入数据（只在当前小时无数据时运行）
                    string insertsql = $"INSERT INTO FaultData VALUES ('{NowDay}',{Hour},{2},{HourCFault})";
                    SQLiteHelp.SQLInsert(insertsql);
                }
            }
            // 添加工位四故障数数据
            if (DFault >= 0)
            {
                // 查询当前时间下数据库中是否有数据
                string Dfaultsql = $"SELECT 1 FROM FaultData WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {3}";
                DataTable DData = SQLiteHelp.ExecuteQuery(Dfaultsql);
                if (DData.Rows.Count > 0)
                {
                    // 更新数据库
                    string updatasql = $"UPDATE FaultData SET Content = {HourDFault} WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {3}";
                    SQLiteHelp.SQLUpdate(updatasql);
                }
                else
                {
                    // 插入数据（只在当前小时无数据时运行）
                    string insertsql = $"INSERT INTO FaultData VALUES ('{NowDay}',{Hour},{3},{HourDFault})";
                    SQLiteHelp.SQLInsert(insertsql);
                }
            }
            // 添加工位五故障数数据
            if (EFault >= 0)
            {
                // 查询当前时间下数据库中是否有数据
                string Efaultsql = $"SELECT 1 FROM FaultData WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {4}";
                DataTable EData = SQLiteHelp.ExecuteQuery(Efaultsql);
                if (EData.Rows.Count > 0)
                {
                    // 更新数据库
                    string updatasql = $"UPDATE FaultData SET Content = {HourEFault} WHERE Day = '{NowDay}' AND Time = {Hour} AND FaultID = {4}";
                    SQLiteHelp.SQLUpdate(updatasql);
                }
                else
                {
                    // 插入数据（只在当前小时无数据时运行）
                    string insertsql = $"INSERT INTO FaultData VALUES ('{NowDay}',{Hour},{4},{HourEFault})";
                    SQLiteHelp.SQLInsert(insertsql);
                }
            }
            #endregion

            /***********************************************************存储详细故障信息**********************************************************************/
            #region 存储详细故障信息（所有故障数据）

            // 获取所有的故障类型列表
            List<int> TotalWorkStation = new List<int>();
            // 获取到所有故障数据的集合
            TotalWorkStation.AddRange(WorkStationOne);
            TotalWorkStation.AddRange(WorkStationTwo);
            TotalWorkStation.AddRange(WorkStationThree);
            TotalWorkStation.AddRange(WorkStationFour);
            TotalWorkStation.AddRange(WorkStationFive);

            // 当当前时间大于0点且小于8点时，当前ModBus数据还未清空
            if (Hour >= 0 && Hour < 8)
            {
                // 获取当前时间段数据，需要用读取到的ModBus数据减去 前一天8点到23点的数据（前一天时间段为 1 的数据）
                // 存入数据库，设为时间段 0 

                // 读取数据库数据，按ID排序
                string selsql = $"SELECT ID,Number,Duration FROM DetailedFault  WHERE Day = '{NowDay}' AND TimeSlot ={0} ORDER BY ID";
                DataTable SelData = SQLiteHelp.ExecuteQuery(selsql);

                // 读取前一天 8 点到 23 点的数据 （前一天时间段为1的数据）
                string yesql = $"SELECT ID,Number,Duration FROM DetailedFault  WHERE Day = '{yesterday}' AND TimeSlot ={1} ORDER BY ID";
                DataTable YesData = SQLiteHelp.ExecuteQuery(yesql);

                // 判断数据库是否存在数据，存在则更新，不存在则插入数据
                if (SelData.Rows.Count > 0)
                {
                    // 如果昨天存在数据，则减去昨天的数据
                    if (YesData.Rows.Count > 0)
                    {
                        // 更新数据库 UPDATE 表名称 SET 列名称 = 新值 WHERE 列名称 = 某值
                        for (int i = 0; i < TotalWorkStation.Count; i++)
                        {
                            // 获取对应ID的昨天的数据
                            int yesvalue = (int)YesData.Rows[i][1];
                            // 得到当前运行的数据，读取的ModBus数据 - 前一天8点到24点的数据
                            int nowvalue = TotalWorkStation[i] - yesvalue;
                            // 获取对应ID的昨天故障时间数据
                            int yesterdaytime = (int)YesData.Rows[i][2];
                            // 得到当前的故障时间数据，读取的ModBus数据 - 前一天8点到24点的数据
                            int nowtime = FaultTimtList[i] - yesterdaytime;

                            // 更新数据库
                            string upsql = $"UPDATE DetailedFault SET Number = {nowvalue},Duration = {nowtime} WHERE ID = {i} AND TimeSlot ={0} AND Day = '{NowDay}'";
                            SQLiteHelp.SQLUpdate(upsql);
                        }
                    }
                    else
                    {
                        // 不存在数据则直接更新读取到的ModBus数据
                        for (int i = 0; i < TotalWorkStation.Count; i++)
                        {
                            string upsql = $"UPDATE DetailedFault SET Number = {TotalWorkStation[i]},Duration = {FaultTimtList[i]} WHERE ID = {i} AND TimeSlot ={0} AND Day = '{NowDay}'";
                            SQLiteHelp.SQLUpdate(upsql);
                        }
                    }
                }
                else
                {
                    // 如果昨天存在数据，则需要减去昨天的数据
                    if (YesData.Rows.Count > 0)
                    {
                        // 插入数据 INSERT INTO table_name VALUES (值1, 值2,....)
                        for (int i = 0; i < TotalWorkStation.Count; i++)
                        {
                            // 获取对应ID的昨天的数据
                            int yesvalue = int.Parse(YesData.Rows[i][1].ToString());
                            // 得到当前运行的数据，读取的ModBus数据 - 前一天8点到24点的数据
                            int nowvalue = TotalWorkStation[i] - yesvalue;

                            // 获得对应ID的昨天的故障时间数据
                            int yesterdaytime = (int)YesData.Rows[i][2];
                            //得到当前时间的故障时间数据
                            int nowtime = FaultTimtList[i] - yesterdaytime;

                            // 插入数据库
                            string insesql = $"INSERT INTO DetailedFault VALUES ('{NowDay}',{0},{i},{nowvalue},{nowtime})";
                            SQLiteHelp.SQLInsert(insesql);
                        }
                    }
                    else
                    {
                        // 插入数据 INSERT INTO table_name (列1, 列2,...) VALUES (值1, 值2,....)
                        for (int i = 0; i < TotalWorkStation.Count; i++)
                        {
                            string insesql = $"INSERT INTO DetailedFault VALUES ('{NowDay}',{0},{i},{TotalWorkStation[i]},{FaultTimtList[i]})";
                            SQLiteHelp.SQLInsert(insesql);
                        }
                    } 
                }
            }
            else
            {
                // 当前时间段ModBus数据已清空，直接存入数据库，设为时间段 1
                string selsql = $"SELECT ID,Number FROM DetailedFault  WHERE Day = '{NowDay}' AND TimeSlot ={1} ORDER BY ID";
                DataTable SelData = SQLiteHelp.ExecuteQuery(selsql);
                if (SelData.Rows.Count > 0)
                {
                    // 更新数据库 UPDATE 表名称 SET 列名称 = 新值 WHERE 列名称 = 某值
                    for (int i = 0; i < TotalWorkStation.Count; i++)
                    {
                        string upsql = $"UPDATE DetailedFault SET Number = {TotalWorkStation[i]} WHERE ID = {i} AND TimeSlot ={1} AND Day = '{NowDay}'";
                        SQLiteHelp.SQLUpdate(upsql);
                    }
                }
                else
                {
                    // 插入数据 INSERT INTO table_name (列1, 列2,...) VALUES (值1, 值2,....)
                    for (int i = 0; i < TotalWorkStation.Count; i++)
                    {
                        string insesql = $"INSERT INTO DetailedFault VALUES ('{NowDay}',{1},{i},{TotalWorkStation[i]},{0})";
                        SQLiteHelp.SQLInsert(insesql);
                    }
                }
            }

            #endregion

            /***********************************************************存储运行时间信息**********************************************************************/
            #region 存储运行时间信息
            // 查询当日数据库，判断有无数据
            string selectsql = $"SELECT * FROM TimeData WHERE Day = '{NowDay}'";
            DataTable SelTable = SQLiteHelp.ExecuteQuery(selectsql);

            int PowerHour = PowerTime[2];             //上电时间-小时
            int PowerMin = PowerTime[1];              //上电时间-分钟
            int PowerS = PowerTime[0];                //上电时间-秒
            int RunHour = RunTime[2];                 //运行时间-小时
            int RunMin = RunTime[1];                  //运行时间-分钟
            int RunS = RunTime[0];                    //运行时间-秒
            int StopHour = StopTime[2];               //停机时间-小时
            int StopMin = StopTime[1];                //停机时间-分钟
            int StopS = StopTime[0];                  //停机时间-秒
            int FaultHour = FaultTime[2];             //故障时间-小时
            int FaultMin = FaultTime[1];              //故障时间-分钟
            int FaultS = FaultTime[0];                //故障时间-秒

            if (SelTable.Rows.Count > 0)
            {
                // 有数据，更新数据库
                string updatapowersql = $"UPDATE TimeData SET Hour = {PowerHour},Minute = {PowerMin},Second={PowerS} WHERE Day = '{NowDay}'AND TimeName = 'PowerTime'";
                string updatarunsql = $"UPDATE TimeData SET Hour = {RunHour},Minute = {RunMin},Second={RunS} WHERE Day = '{NowDay}'AND TimeName = 'RunTime'";
                string updatastopsql = $"UPDATE TimeData SET Hour = {StopHour},Minute = {StopMin},Second={StopS} WHERE Day = '{NowDay}'AND TimeName = 'StopTime'";
                string updatafaultsql = $"UPDATE TimeData SET Hour = {FaultHour},Minute = {FaultMin},Second={FaultS} WHERE Day = '{NowDay}'AND TimeName = 'FaultTime'";
                //将数据存储到数据库
                SQLiteHelp.SQLUpdate(updatapowersql);
                SQLiteHelp.SQLUpdate(updatarunsql);
                SQLiteHelp.SQLUpdate(updatastopsql);
                SQLiteHelp.SQLUpdate(updatafaultsql);
            }
            else
            {
                //无数据，插入数据
                string Insertpowersql = $"INSERT INTO TimeData Values ('{NowDay}','PowerTime',{PowerHour},{PowerMin},{PowerS})";
                string Insertrunsql = $"INSERT INTO TimeData Values ('{NowDay}','RunTime',{RunHour},{RunMin},{RunS})";
                string Insertstopsql = $"INSERT INTO TimeData Values ('{NowDay}','StopTime',{StopHour},{StopMin},{StopS})";
                string Insertfaultsql = $"INSERT INTO TimeData Values ('{NowDay}','FaultTime',{FaultHour},{FaultMin},{FaultS})";
                //将数据存储到数据库
                SQLiteHelp.SQLInsert(Insertpowersql);
                SQLiteHelp.SQLInsert(Insertrunsql);
                SQLiteHelp.SQLInsert(Insertstopsql);
                SQLiteHelp.SQLInsert(Insertfaultsql);
            }
            #endregion
        }

        /// <summary>
        /// 获取故障数据
        /// </summary>
        /// <param name="NowDay">今天</param>
        /// <param name="Hour">当前小时</param>
        /// <param name="yesterday">昨天</param>
        public static void GetFault(string NowDay, int Hour, string yesterday)
        {
            for (int i = 0; i < MainWindow.FaultList.Length; i++)
            {
                // 获取前一天8点到24点的数据和
                string yesterdaySql = $"SELECT sum(Content) FROM FaultData WHERE Day = '{yesterday}' AND FaultID = {i} AND  Time > {8}";
                DataTable YesterDay = SQLiteHelp.ExecuteQuery(yesterdaySql);
                // 有数据
                if (YesterDay.Rows[0][0].ToString() != "")
                {
                    switch (i)
                    {
                        case 0:
                            YesterDayAFault = int.Parse(YesterDay.Rows[0][0].ToString());
                            break;
                        case 1:
                            YesterDayBFault = int.Parse(YesterDay.Rows[0][0].ToString());
                            break;
                        case 2:
                            YesterDayCFault = int.Parse(YesterDay.Rows[0][0].ToString());
                            break;
                        case 3:
                            YesterDayDFault = int.Parse(YesterDay.Rows[0][0].ToString());
                            break;
                        case 4:
                            YesterDayEFault = int.Parse(YesterDay.Rows[0][0].ToString());
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            YesterDayAFault = 0;
                            break;            
                        case 1:
                            YesterDayBFault = 0;
                            break;            
                        case 2:
                            YesterDayCFault = 0;
                            break; 
                        case 3:
                            YesterDayDFault = 0;
                            break;            
                        case 4:
                            YesterDayEFault = 0;
                            break;            
                        default:              
                            break;            
                    }                         
                }
                
                // 获取当前小时之前的数据
                string BeforeSql = $"SELECT sum(Content) FROM FaultData WHERE Day = '{NowDay}' AND FaultID = {i} AND  Time < {Hour}";
                DataTable BeforeData = SQLiteHelp.ExecuteQuery(BeforeSql);
                // 判断是否存在数据
                if (BeforeData.Rows[0][0].ToString() != "")
                {
                    switch (i)
                    {
                        case 0:
                            AFaultSum = int.Parse(BeforeData.Rows[0][0].ToString());
                            break;
                        case 1:
                            BFaultSum = int.Parse(BeforeData.Rows[0][0].ToString());
                            break;
                        case 2:
                            CFaultSum = int.Parse(BeforeData.Rows[0][0].ToString());
                            break;
                        case 3:
                            DFaultSum = int.Parse(BeforeData.Rows[0][0].ToString());
                            break;
                        case 4:
                            EFaultSum = int.Parse(BeforeData.Rows[0][0].ToString());
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            AFaultSum = 0;
                            break;
                        case 1:
                            BFaultSum = 0;
                            break;
                        case 2:
                            CFaultSum = 0;
                            break;
                        case 3:
                            DFaultSum = 0;
                            break;
                        case 4:
                            EFaultSum = 0;
                            break;
                        default:
                            break;
                    }
                }

                // 获取当前天当前小时前一个小时到当天8点的数据
                string TodaySql = $"SELECT sum(Content) FROM FaultData WHERE Day = '{NowDay}' AND FaultID = {i} AND  Time >= {8} AND Time <{Hour}";
                DataTable ToDayData = SQLiteHelp.ExecuteQuery(TodaySql);

                if (ToDayData.Rows[0][0].ToString() != "")
                {
                    switch (i)
                    {
                        case 0:
                            ToDayAFaultData = int.Parse(ToDayData.Rows[0][0].ToString());
                            break;
                        case 1:
                            ToDayBFaultData = int.Parse(ToDayData.Rows[0][0].ToString());
                            break;
                        case 2:
                            ToDayCFaultData = int.Parse(ToDayData.Rows[0][0].ToString());
                            break;
                        case 3:
                            ToDayDFaultData = int.Parse(ToDayData.Rows[0][0].ToString());
                            break;
                        case 4:
                            ToDayEFaultData = int.Parse(ToDayData.Rows[0][0].ToString());
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            ToDayAFaultData = 0;
                            break;
                        case 1:
                            ToDayBFaultData = 0;
                            break;
                        case 2:
                            ToDayCFaultData = 0;
                            break;
                        case 3:
                            ToDayDFaultData = 0;
                            break;
                        case 4:
                            ToDayEFaultData = 0;
                            break;
                        default:
                            break;
                    }
                }

                // 获取当天的生产的总数据
                string Todaysumsql = $"SELECT sum(Content) FROM FaultData WHERE Day = '{NowDay}' AND FaultID = {i}";
                DataTable TodaySumData = SQLiteHelp.ExecuteQuery(Todaysumsql);
                if (TodaySumData.Rows[0][0].ToString() != "")
                {
                    switch (i)
                    {
                        case 0:
                            ToDayAFault = int.Parse(TodaySumData.Rows[0][0].ToString());
                            break;
                        case 1:
                            ToDayBFault = int.Parse(TodaySumData.Rows[0][0].ToString());
                            break;
                        case 2:
                            ToDayCFault = int.Parse(TodaySumData.Rows[0][0].ToString());
                            break;
                        case 3:
                            ToDayDFault = int.Parse(TodaySumData.Rows[0][0].ToString());
                            break;
                        case 4:
                            ToDayEFault = int.Parse(TodaySumData.Rows[0][0].ToString());
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            ToDayAFault = 0;
                            break;
                        case 1:
                            ToDayBFault = 0;
                            break;
                        case 2:
                            ToDayCFault = 0;
                            break;
                        case 3:
                            ToDayDFault = 0;
                            break;
                        case 4:
                            ToDayEFault = 0;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
