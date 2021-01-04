
using EVMESCharts.Sqlite;
using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using EVMESCharts.Popup;
using System.Windows;
using EVMESCharts.ChartView;

namespace EVMESCharts.Charts
{
    /// <summary>
    /// DataCharts.xaml 的交互逻辑
    /// </summary>
    public partial class DataCharts : UserControl, INotifyPropertyChanged
    {
        #region 定义页面图标颜色和故障类型
        /// <summary>
        /// 上升图标
        /// </summary>
        string Icon01 = "\ue63c";
        /// <summary>
        /// 下降图标
        /// </summary>
        string Icon02 = "\ue668";
        /// <summary>
        /// 按钮点击颜色
        /// </summary>
        string Clickcolor = "#cacaca";
        /// <summary>
        /// 按钮颜色
        /// </summary>
        string Buttoncolor = "#f9f9fc";

        #endregion

        /// <summary>
        /// 设备详情页面
        /// </summary>
        public DataCharts()
        {
            InitializeComponent();

            //饼状图标签信息
            PointLabel = chartPoint =>
                string.Format("{0}", chartPoint.Y, chartPoint.Participation);

            //添加饼状图信息
            AddPieChartData();

            //添加默认设备数据信息
            AddDeviceList();

            BgColor = MainWindow.WindowBgColor;
            FColor = MainWindow.WindowFontColor;

            //触发改变数据函数（刷新数据）
            ChangeHourData();

            this.DataContext = this;
        }
        
        /// <summary>
        /// 定义控件背景颜色
        /// </summary>
        public string BgColor
        {
            get;
            set;
        }

        /// <summary>
        /// 定义字体颜色
        /// </summary>
        public string FColor
        {
            get;
            set;
        }

        /// <summary>
        /// 添加初始设备信息
        /// </summary>
        private void AddDeviceList()
        {
            DeviceChart = new ObservableCollection<DeviceList>();
            string devlidtsql = "SELECT ID FROM Equipment";
            DataTable devdata = SQLiteHelp.ExecuteQuery(devlidtsql);

            //获取当前设备编号列表
            var DevNumberList = SQLiteHelp.NumberList(devdata, "ID");

            for (int i = 0; i < DevNumberList.Count; i++)
            {
                //添加初始设备信息
                DeviceChart.Add(new DeviceList
                {
                    DeviceName = MainWindow.DeviceList[i],
                    ProductionSum = "0",
                    Efficiency = (0).ToString("p0"),
                    GoodProduce = (1).ToString("p0"),
                    ChartXAxisTitle = "",
                    DevReachTitle = "",
                    DevReachValue = new GearedValues<double>(),
                    DevQualityTitle = "",
                    DevQualityValue = new GearedValues<double>(),
                    FontColor = "#4caf50",
                    FontBgc = "#cbffcd",
                    FontText = Icon01,
                    AxisXMax = 10,
                    AxisXMin = 0,
                    PowerGasTitle = "",
                    ConsumptionGas = new ChartValues<double>(),
                    ConsumePower = new ChartValues<double>(),
                    BgColor = MainWindow.WindowBgColor,
                    FColor = MainWindow.WindowFontColor,
                });
            }
            //添加默认当天设备数据
            GetDayDeviceList();
        }


        #region 添加页面数据

        #region 添加饼状图数据信息
        /// <summary>
        /// 添加饼状图数据
        /// </summary>
        private void AddPieChartData()
        {
            // 添加小时数据
            var DayTime = DateTime.Today.Date.ToShortDateString();
            var HourTime = DateTime.Now.Hour.ToString();
            string hoursql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}' AND Time = {HourTime}";
            DataTable HourData = SQLiteHelp.ExecuteQuery(hoursql);

            // 判断是否有数据
            if (HourData.Rows.Count > 0)
            {
                HourPieCharts = new BreakDownPieChart("Hour", "Right");
            }
            else
            {
                HourPieCharts = new NoData();
            }

            // 添加天数据
            string daysql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}'";
            DataTable DayData = SQLiteHelp.ExecuteQuery(daysql);

            //判断是否有数据
            if (DayData.Rows.Count > 0)
            {
                DayPieCharts = new BreakDownPieChart("Day", "Right");
            }
            else
            {
                DayPieCharts = new NoData();
            }

            // 查询故障表（月）
            string monthsql = $"SELECT 1 FROM FaultData";
            DataTable MonthData = SQLiteHelp.ExecuteQuery(monthsql);

            // 判断是否有数据
            if (MonthData.Rows.Count > 0)
            {
                // 添加月数据
                MonthPieCharts = new BreakDownPieChart("Month", "Right");
            }
            else
            {
                MonthPieCharts = new NoData();
            }

        }

        /// <summary>
        /// 天饼状图图表
        /// </summary>
        public UserControl daypiecharts;
        /// <summary>
        /// 页面绑定天数据饼状图图表
        /// </summary>
        public UserControl DayPieCharts
        {
            get
            {
                return daypiecharts;
            }
            set
            {
                daypiecharts = value;
                OnPropertyChanged(nameof(DayPieCharts));
            }
        }

        /// <summary>
        /// 小时饼状图图表
        /// </summary>
        public UserControl hourpiecharts;
        /// <summary>
        /// 页面绑定小时数据饼状图图表
        /// </summary>
        public UserControl HourPieCharts
        {
            get
            {
                return hourpiecharts;
            }
            set
            {
                hourpiecharts = value;
                OnPropertyChanged(nameof(HourPieCharts));
            }
        }

        /// <summary>
        /// 小时饼状图图表
        /// </summary>
        public UserControl monthpiecharts;
        /// <summary>
        /// 页面绑定小时数据饼状图图表
        /// </summary>
        public UserControl MonthPieCharts
        {
            get
            {
                return monthpiecharts;
            }
            set
            {
                monthpiecharts = value;
                OnPropertyChanged(nameof(MonthPieCharts));
            }
        }

        #endregion

        /// <summary>
        /// 添加当天设备信息数据
        /// </summary>
        public void GetDayDeviceList()
        {
            // 获取屏幕工作区大小
            var height = SystemParameters.WorkArea.Size.Height;
            for (int i = 0; i < DeviceChart.Count; i++)
            {
                DeviceChart[i].ChartXAxisTitle = "时间/小时";                       // X轴显示的横坐标
                DeviceChart[i].DevReachTitle = "日达成率图表";                      // 达成率图表标题
                DeviceChart[i].DevQualityTitle = "日良品率图表";                    // 良品率图表标题
                DeviceChart[i].PowerGasTitle = "日电气消耗图表";                    // 电气消耗图表标题
                DeviceChart[i].DevReachValue = AddDayDevReachValue(i);              // 达成率图表数据
                DeviceChart[i].DevQualityValue = AddDayQuqlityValue(i);             // 良品率图表数据
                DeviceChart[i].ConsumptionGas = AddDayGasValue(i);                  // 图表显示气消耗数据
                DeviceChart[i].ConsumePower = AddPowerValue(i);                     // 图表显示电消耗数据
                DeviceChart[i].ProductionSum = AddData(i, "ProductionSum");         // 左侧显示生产总数
                DeviceChart[i].Efficiency = AddData(i, "Efficiency");               // 左侧显示生产效率
                DeviceChart[i].GoodProduce = AddData(i, "GoodProduce");             // 左侧显示良品率
                DeviceChart[i].FontText = AddData(i, "FontText");                   // 左侧ICON显示样式
                DeviceChart[i].FontColor = AddData(i, "FontColor");                 // ICON 文字 颜色
                DeviceChart[i].FontBgc = AddData(i, "FontBgc");                     // ICON 背景色
                DeviceChart[i].AxisXMax = ChangeDayXAxis(i, "Max");                 // 图表X轴显示的最大值
                DeviceChart[i].AxisXMin = ChangeDayXAxis(i, "Min");                 // 图表X轴显示的最小值
                DeviceChart[i].XFormatter = (value) => value.ToString($"{0}':00'"); // 格式化X轴坐标 
                DeviceChart[i].ChartHeight = height * 0.3;                          // 定义展示图表的高度
            }
        }

        /// <summary>
        /// 添加当月设备信息数据
        /// </summary>
        private void GetMonthDeviceList()
        {
            for (int i = 0; i < DeviceChart.Count; i++)
            {
                DeviceChart[i].ChartXAxisTitle = "时间/天";                         // X轴显示的横坐标
                DeviceChart[i].DevReachTitle = "月达成率图表";                      // 达成率图表标题
                DeviceChart[i].DevQualityTitle = "月良品率图表";                    // 良率图表标题
                DeviceChart[i].PowerGasTitle = "月电气消耗图表";                    // 电气消耗图表标题
                DeviceChart[i].DevReachValue = AddMonthDevReachValue(i);            // 达成率数据
                DeviceChart[i].DevQualityValue = AddMonthDevQualityValue(i);        // 良率数据
                DeviceChart[i].ConsumptionGas = AddMonthConsumeValue(i, "Gas");      // 气消耗数据
                DeviceChart[i].ConsumePower = AddMonthConsumeValue(i, "Electric");  // 电消耗数据
                DeviceChart[i].ProductionSum = AddMonthData(i, "ProductionSum");    // 左侧显示的设备总产量
                DeviceChart[i].Efficiency = AddMonthData(i, "Efficiency");          // 左侧显示的设备生产效率
                DeviceChart[i].GoodProduce = AddMonthData(i, "GoodProduce");        // 左侧显示的良率
                DeviceChart[i].FontText = AddMonthData(i, "FontText");              // ICON样式
                DeviceChart[i].FontColor = AddMonthData(i, "FontColor");            // ICON颜色
                DeviceChart[i].FontBgc = AddMonthData(i, "FontBgc");                // ICON背景
                DeviceChart[i].AxisXMax = ChangeMonthXAxis(i, "Max");               // X轴显示的最大值
                DeviceChart[i].AxisXMin = ChangeMonthXAxis(i, "Min");               // X轴显示的最小值
                DeviceChart[i].XFormatter = (value) => (value + 1).ToString($"{DateTime.Now.Month.ToString()}月{0}日");
            }
        }

        /********************************************************添加当日设备信息数据***********************************************************/

        #region 添加日设备信息数据

        /// <summary>
        /// 改变天数据的X轴显示的最大最小值
        /// </summary>
        /// <param name="num">设备ID</param>
        /// <param name="Type">Max  or  Min</param>
        /// <returns></returns>
        public int ChangeDayXAxis(int num, string Type)
        {
            //当前时间
            var Daytime = DateTime.Today.Date.ToShortDateString();
            string sql = $"SELECT Time FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = '{num}'";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);

            // 获取数据长度
            int xaxismax = data.Rows.Count;

            // 改变X轴显示的最大最小值
            switch (Type)
            {
                case "Max":
                    if (xaxismax < 10)
                    {
                        return 10;
                    }
                    else
                    {
                        return xaxismax;
                    }
                case "Min":
                    if (xaxismax < 10)
                    {
                        return 0;
                    }
                    else
                    {
                        return xaxismax - 10;
                    }
                default:
                    return 1;
            }
        }

        /// <summary>
        /// 添加当日其他展示数据 (生产总数、总生产效率、总良品率、ICON显示的颜色、样式、背景)
        /// </summary>
        /// <param name="i">设备编号</param>
        /// <param name="Type">展示字段名称</param>
        /// <returns></returns>
        private string AddData(int i, string Type)
        {
            //当前时间
            var Daytime = DateTime.Today.Date.ToShortDateString();

            // 查询当日数据
            string sql = "SELECT DevID,sum(Produce),sum(GoodProduct) " +
                         " FROM DayDeviceData " +
                         $" WHERE DevID = {i} AND Day = '{Daytime}'";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);

            // 添加数据
            if (data.Rows[0][1].ToString() != "")
            {
                switch (Type)
                {
                    // 生产总数
                    case "ProductionSum":
                        return data.Rows[0][1].ToString();

                    // 生产效率
                    case "Efficiency":
                        return (double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan).ToString("p2");

                    //良品率
                    case "GoodProduce":
                        //return (double.Parse(data.Rows[0][2].ToString()) / double.Parse(data.Rows[0][1].ToString())).ToString("p2");
                        return 1.ToString("p2");

                    // ICON样式
                    case "FontText":
                        if (double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan < 1)
                        {
                            return Icon02;
                        }
                        else
                        {
                            return Icon01;
                        }

                    // ICON 颜色
                    case "FontColor":
                        if (double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan < 1)
                        {
                            return "#ff4a4a";
                        }
                        else
                        {
                            return "#4caf50";
                        }

                    // ICON 背景颜色
                    case "FontBgc":
                        if (double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan < 1)
                        {
                            return "#ffcbcb";
                        }
                        else
                        {
                            return "#cbffcd";
                        }
                    default:
                        return "";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 添加当天达成率数据
        /// </summary>
        /// <param name="num">设备id</param>
        /// <returns>GearedValues</returns>
        public GearedValues<double> AddDayDevReachValue(int num)
        {
            // 查询当天数据
            var Daytime = DateTime.Today.Date.ToShortDateString();
            string sql = $"SELECT Time,Produce FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = '{num}'";

            //获取数据库表中当天的数据
            DataTable dt = SQLiteHelp.ExecuteQuery(sql);

            GearedValues<double> Reach = new GearedValues<double>();

            //打开程序时，添加初始化达成率信息
            foreach (DataRow dr in dt.Rows)
            {
                double HourReach = double.Parse(dr["Produce"].ToString());

                // 存储数据
                Reach.Add(HourReach / MainWindow.HourPlan);
            }
            return Reach;
        }

        /// <summary>
        /// 添加当天良率图表数据
        /// </summary>
        /// <param name="num">设备编号</param>
        /// <returns></returns>
        private GearedValues<double> AddDayQuqlityValue(int num)
        {
            var Daytime = DateTime.Today.Date.ToShortDateString();

            string sql = $"SELECT Time,Produce,GoodProduct FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = '{num}'";
            //获取数据库表中当天的数据
            DataTable dt = SQLiteHelp.ExecuteQuery(sql);

            GearedValues<double> Quqlity = new GearedValues<double>();

            //打开程序时，添加初始化良率信息  良品数/产出数
            foreach (DataRow dr in dt.Rows)
            {
                double HourReach = double.Parse(dr["Produce"].ToString());
                double HourGood = double.Parse(dr["GoodProduct"].ToString());
                //保留三位小数
                Quqlity.Add(double.Parse((HourGood / HourReach).ToString("N3")));
            }
            return Quqlity;
        }

        /// <summary>
        /// 添加气消耗数据（当天）
        /// </summary>
        /// <param name="num">设备编号</param>
        /// <returns></returns>
        private ChartValues<double> AddDayGasValue(int num)
        {
            var Daytime = DateTime.Today.Date.ToShortDateString();

            string sql = $"SELECT Time,Gas FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = '{num}'";

            //获取数据库表中当天的数据
            DataTable dt = SQLiteHelp.ExecuteQuery(sql);

            ChartValues<double> GasList = new ChartValues<double>();

            //打开程序时，添加初始化气消耗信息
            foreach (DataRow dr in dt.Rows)
            {
                double HourGas = double.Parse(dr["Gas"].ToString());
                GasList.Add(HourGas);
            }
            return GasList;
        }

        /// <summary>
        /// 添加电消耗数据 (当天)
        /// </summary>
        /// <param name="num">设备编号</param>
        /// <returns></returns>
        private ChartValues<double> AddPowerValue(int num)
        {
            var Daytime = DateTime.Today.Date.ToShortDateString();

            string sql = $"SELECT Time,Electric FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = '{num}'";

            //获取数据库表中当天的数据
            DataTable dt = SQLiteHelp.ExecuteQuery(sql);

            ChartValues<double> Electric = new ChartValues<double>();

            //打开程序时，添加初始化电消耗信息
            foreach (DataRow dr in dt.Rows)
            {
                double HourGas = double.Parse(dr["Electric"].ToString());
                Electric.Add(HourGas);
            }
            return Electric;
        }
        
        /// <summary>
        /// System.Timers.Timer (计时)
        /// </summary>
        private Timer DaytimerNotice;

        /// <summary>
        /// 每台设备记录的信息条数
        /// </summary>
        public List<int> DevValueLengthList
        {
            get;
            set;
        } = new List<int>();
        /// <summary>
        /// 每小时改变数据 间隔触发函数
        /// </summary>
        public void ChangeHourData()
        {
            var Daytime = DateTime.Today.Date.ToShortDateString();

            for (int i = 0; i < DeviceChart.Count; i++)
            {
                string sql = $"SELECT Time From DayDeviceData WHERE DevID = {i} AND Day = '{Daytime}'";
                DataTable timetable = SQLiteHelp.ExecuteQuery(sql);

                // 记录每台设备当天的数据条数
                DevValueLengthList.Add(timetable.Rows.Count);
            }

            DaytimerNotice = new System.Timers.Timer();
            //间隔触发函数
            DaytimerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                var nowday = DateTime.Today.Date.ToShortDateString();
                //获取数据库中设备个数
                for (int i = 0; i < DeviceChart.Count; i++)
                {
                    // 当程序运行超过一天时，清空当前图表上一天的数据重新添加新数据
                    if (nowday != Daytime)
                    {
                        DevValueLengthList[i] = 0;
                        DeviceChart[i].DevReachValue.Clear();
                        DeviceChart[i].DevQualityValue.Clear();
                        DeviceChart[i].ConsumptionGas.Clear();
                        DeviceChart[i].ConsumePower.Clear();
                    }

                    // 查询图表数据
                    string sql = $"SELECT Time,DevID,Produce,Electric,Gas,GoodProduct From DayDeviceData WHERE DevID = {i} AND Day = '{nowday}'";
                    DataTable devvalue = SQLiteHelp.ExecuteQuery(sql);

                    // 更新图表数据
                    if (devvalue.Rows.Count > DevValueLengthList[i])
                    {
                        // 获取最后一条数据
                        int time = int.Parse(devvalue.Rows[devvalue.Rows.Count - 1][0].ToString());
                        int devnumber = int.Parse(devvalue.Rows[devvalue.Rows.Count - 1][1].ToString());         // 设备编号
                        int produce = int.Parse(devvalue.Rows[devvalue.Rows.Count - 1][2].ToString());           // 小时产出
                        double electric = double.Parse(devvalue.Rows[devvalue.Rows.Count - 1][3].ToString());    // 小时用电量
                        double gas = double.Parse(devvalue.Rows[devvalue.Rows.Count - 1][4].ToString());         // 小时耗气量
                        int goodproduct = int.Parse(devvalue.Rows[devvalue.Rows.Count - 1][5].ToString());       // 良品数

                        double reachvalue = double.Parse((double.Parse(produce.ToString()) / MainWindow.HourPlan).ToString("N3"));    // 达成率
                        double qualityvalue = double.Parse((double.Parse(goodproduct.ToString()) / produce).ToString("N3"));          // 良率

                        // 添加对应图表信息数据
                        DeviceChart[i].DevReachValue.Add(reachvalue);
                        DeviceChart[i].DevQualityValue.Add(qualityvalue);
                        DeviceChart[i].ConsumptionGas.Add(gas);
                        DeviceChart[i].ConsumePower.Add(electric);

                        // 改变图表显示的X轴大小
                        if (devvalue.Rows.Count > 10)
                        {
                            DeviceChart[i].AxisXMax = devvalue.Rows.Count;
                            DeviceChart[i].AxisXMin = devvalue.Rows.Count - 10;
                        }
                        else
                        {
                            DeviceChart[i].AxisXMax = 10;
                            DeviceChart[i].AxisXMin = 0;
                        }

                        //改变后更新对应的设备数据条数
                        DevValueLengthList[i] = devvalue.Rows.Count;
                    }

                    // 添加左侧显示的当日总数据
                    string daysql = "SELECT DevID,sum(Produce),sum(GoodProduct) " +
                         " FROM DayDeviceData " +
                         $" WHERE DevID = {i} AND Day = '{nowday}'";
                    DataTable daytable = SQLiteHelp.ExecuteQuery(daysql);

                    if (daytable.Rows[0][1].ToString() != "")
                    {
                        double prosum = double.Parse(daytable.Rows[0][1].ToString());
                        double goodsum = double.Parse(daytable.Rows[0][2].ToString());
                        // 添加左侧显示数据
                        DeviceChart[i].ProductionSum = daytable.Rows[0][1].ToString();
                        DeviceChart[i].Efficiency = (prosum / MainWindow.DayPlan).ToString("p2");
                        //DeviceChart[i].GoodProduce = (goodsum / prosum).ToString("p2");
                        DeviceChart[i].GoodProduce = (1).ToString("p2");
                        if (prosum / MainWindow.DayPlan < 1)
                        {
                            DeviceChart[i].FontText = Icon02;
                            DeviceChart[i].FontBgc = "#ffcbcb";
                            DeviceChart[i].FontColor = "#ff4a4a";
                        }
                        else
                        {
                            DeviceChart[i].FontText = Icon01;
                            DeviceChart[i].FontBgc = "#cbffcd";
                            DeviceChart[i].FontColor = "#4caf50";
                        }
                    }
                }
            });
            //触发间隔 
            DaytimerNotice.Interval = MainWindow.Time;
            DaytimerNotice.Start();
        }


        #endregion

        /********************************************************添加当月设备信息数据***********************************************************/

        #region 添加月设备信息数据

        /// <summary>
        /// 改变月数据显示的X轴最大最小值
        /// </summary>
        /// <param name="num"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public int ChangeMonthXAxis(int num, string Type)
        {
            int nowday =int.Parse(DateTime.Now.Day.ToString());
            switch (Type)
            {
                case "Max":
                    if (nowday > 10)
                    {
                        return nowday;
                    }
                    else
                    {
                        return 10;
                    }
                case "Min":
                    if (nowday > 10)
                    {
                        return nowday - 10;
                    }
                    else
                    {
                        return 0;
                    }
                default:
                    break;
            }
            return 10;
        }

        /// <summary>
        /// 储存除当天数据外的当月产量数据
        /// </summary>
        public List<int> DevSumList
        {
            get;
            set;
        } = new List<int>();

        /// <summary>
        /// 储存除当天数据外的当月良品数数据
        /// </summary>
        public List<int> DevGoodSumList
        {
            get;
            set;
        } = new List<int>();

        /// <summary>
        /// 添加月其他数据（生产总数、总生产效率、总良品率、ICON显示的颜色、样式、背景）
        /// </summary>
        /// <param name="num">设备ID</param>
        /// <param name="Type">要添加的数据名(生产总数、总生产效率、总良品率、ICON显示的颜色、样式、背景)</param>
        /// <returns></returns>
        public string AddMonthData(int num, string Type)
        {
            // 获取当前天数
            var nowday = DateTime.Now.Day.ToString();
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();
            var nowdaytime = DateTime.Today.Date.ToShortDateString();

            double sum = 0;
            double goodsum = 0;
            
            for (int i = 1; i < int.Parse(nowday) + 1; i++)
            {
                // 生成当前天数之前的日期
                string nowtime = nowyear + "-" + nowmonth + "-" + i;
                // 查询数据库
                string sql = $"SELECT DevID,Produce,GoodProduct FROM MonthDeviceData WHERE Day = '{nowtime}' AND DevID = {num}";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    if (data.Rows[0][1].ToString() == "")
                    {
                        sum += 0;
                    }
                    else
                    {
                        // 计算当月的总产量和良品数
                        switch (Type)
                        {
                            case "ProductionSum":
                                sum += double.Parse(data.Rows[0][1].ToString());
                                break;
                            case "Efficiency":
                                sum += double.Parse(data.Rows[0][1].ToString());
                                break;
                            case "GoodProduce":
                                sum += double.Parse(data.Rows[0][1].ToString());
                                //goodsum += double.Parse(data.Rows[0][2].ToString());
                                goodsum += double.Parse(data.Rows[0][1].ToString());
                                break;
                            case "FontText":
                                sum += double.Parse(data.Rows[0][1].ToString());
                                break;
                            case "FontColor":
                                sum += double.Parse(data.Rows[0][1].ToString());
                                break;
                            case "FontBgc":
                                sum += double.Parse(data.Rows[0][1].ToString());
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
                else
                {
                    sum += 0;
                }
            }
            //获取当日的产量和良品数
            string sumsql = $"SELECT DevID,Produce,GoodProduct FROM MonthDeviceData WHERE Day = '{nowdaytime}' AND DevID = {num}";
            DataTable sumdata = SQLiteHelp.ExecuteQuery(sumsql);
            if (sumdata.Rows.Count > 0)
            {
                int nowproduce = int.Parse(sumdata.Rows[0][1].ToString());
                int nowgood = int.Parse(sumdata.Rows[0][2].ToString());
                if (sum > 0)
                {
                    // 存储除当前日的当月所有良品数和产量
                    DevSumList.Add((int)sum - nowproduce);
                    DevGoodSumList.Add((int)goodsum - nowgood);
                }
                else
                {
                    DevSumList.Add(0);
                    DevGoodSumList.Add(0);
                }
            }
            switch (Type)
            {
                //生产总数、总生产效率、总良品率、ICON显示的颜色、样式、背景
                // 生产总数
                case "ProductionSum":
                    return sum.ToString();

                // 总生产效率
                case "Efficiency":
                    return (sum / MainWindow.MonthPlan).ToString("p2");

                // 总良品率
                case "GoodProduce":
                    //return (goodsum / sum).ToString("p2");
                    return (1).ToString("p2");

                // ICON显示的样式 上升/下降
                case "FontText":
                    if (sum/ MainWindow.MonthPlan < 1)
                    {
                        return Icon02;
                    }
                    else
                    {
                        return Icon01;
                    }

                // 颜色
                case "FontColor":
                    if (sum / MainWindow.MonthPlan < 1)
                    {
                        return "#ff4a4a";
                    }
                    else
                    {
                        return "#4caf50";
                    }

                // 背景
                case "FontBgc":
                    if (sum / MainWindow.MonthPlan < 1)
                    {
                        return "#ffcbcb";
                    }
                    else
                    {
                        return "#cbffcd";
                    }
                default:
                    return "";
            }
        }

        /// <summary>
        /// 添加月达成率数据
        /// </summary>
        /// <param name="num">设备编号</param>
        /// <returns></returns>
        public GearedValues<double> AddMonthDevReachValue(int num)
        {
            // 获取当前天数
            var nowday = DateTime.Now.Day.ToString();
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();

            GearedValues<double> reach = new GearedValues<double>();

            for (int i = 1; i < int.Parse(s: nowday)+1; i++)
            {
                // 生成当前天数之前的日期
                string nowtime = nowyear + "-" + nowmonth + "-" + i;
                // 查询数据库
                string sql = $"SELECT DevID,Produce FROM MonthDeviceData WHERE Day = '{nowtime}' AND DevID = {num}";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);

                if (data.Rows.Count > 0)
                {
                    // 如果查询的天数据没有生产记录时，添加数据为 0
                    if (data.Rows[0][1].ToString() == "")
                    {
                        reach.Add(0);
                    }
                    else
                    {
                        // 添加数据
                        reach.Add(double.Parse((double.Parse(data.Rows[0][1].ToString())/ MainWindow.DayPlan).ToString("N3")));
                    }
                }
                else
                {
                    reach.Add(0);
                }
            }
            return reach;
        }

        /// <summary>
        /// 添加月良品率数据
        /// </summary>
        /// <param name="num">设备编号</param>
        /// <returns></returns>
        public GearedValues<double> AddMonthDevQualityValue(int num)
        {
            // 获取当前天数
            var nowday = DateTime.Now.Day.ToString();
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();

            GearedValues<double> quality = new GearedValues<double>();

            for (int i = 1; i < int.Parse(s: nowday) + 1; i++)
            {
                // 生成当前天数之前的日期
                string nowtime = nowyear + "-" + nowmonth + "-" + i;
                // 查询数据库
                string sql = $"SELECT DevID,Produce,GoodProduct FROM MonthDeviceData WHERE Day = '{nowtime}' AND DevID = {num}";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    if (data.Rows[0][1].ToString() == "")
                    {
                        // 如果查询的天数据没有良品记录时，添加数据为 0
                        quality.Add(0);
                    }
                    else
                    {
                        // 添加数据 良品数/生产总数
                        quality.Add(double.Parse((double.Parse(data.Rows[0][2].ToString()) / double.Parse(data.Rows[0][1].ToString())).ToString("N3")));
                    }
                }
                else
                {
                    quality.Add(0);
                }
            }
            return quality;
        }

        /// <summary>
        /// 添加月电气消耗数据
        /// </summary>
        /// <param name="num">设备编号</param>
        /// <param name="Type">电/气消耗</param>
        /// <returns></returns>
        public ChartValues<double> AddMonthConsumeValue(int num, string Type)
        {
            // 获取当前天数
            var nowday = DateTime.Now.Day.ToString();
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();

            ChartValues<double> consume = new ChartValues<double>();

            for (int i = 1; i < int.Parse(s: nowday) + 1; i++)
            {
                // 生成当前天数之前的日期
                string nowtime = nowyear + "-" + nowmonth + "-" + i;
                // 查询数据库
                string sql = $"SELECT DevID,Electric,Gas FROM MonthDeviceData WHERE Day = '{nowtime}' AND DevID = {num}";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    if (data.Rows[0][1].ToString() == "")
                    {
                        // 如果无消耗数据，添加 0
                        consume.Add(0);
                    }
                    else
                    {
                        switch (Type)
                        {
                            // 气消耗
                            case "Gas":
                                consume.Add(double.Parse(data.Rows[0][2].ToString()));
                                break;
                            // 电消耗
                            case "Electric":
                                consume.Add(double.Parse(data.Rows[0][1].ToString()));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    consume.Add(0);
                }
            }
            return consume;
        }

        /// <summary>
        /// 记录天数，判断是否需要添加数据
        /// </summary>
        public int Day
        {
            get;
            set;
        }
        /// <summary>
        /// System.Timers.Timer (月数据计时)
        /// </summary>
        private Timer MonthtimerNotice;
        /// <summary>
        /// 改变月数据函数 （间隔触发）
        /// 每次更新替换掉当天的数据（不改变之前数据）
        /// </summary>
        public void ChangeMonthData()
        {
            // 获取当前天数
            Day = int.Parse(DateTime.Now.Day.ToString());
            // 获取当前月份
            int Month = int.Parse(DateTime.Now.Month.ToString());
            // 获取当前年份
            int Year = int.Parse(DateTime.Now.Year.ToString());

            MonthtimerNotice = new System.Timers.Timer();
            //间隔触发函数
            MonthtimerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                //复制数据
                List<int> FormerProduce = new List<int>(DevSumList.ToArray());
                List<int> FormerGood = new List<int>(DevGoodSumList.ToArray());

                int NowDay = int.Parse(DateTime.Now.Day.ToString());
                int NowMonth = int.Parse(DateTime.Now.Month.ToString());
                // 生成当前天数之前的日期
                string Nowtime = Year + "-" + NowMonth + "-" + NowDay;

                for (int i = 0; i < DeviceChart.Count; i++)
                {
                    // 查询数据库
                    string sql = $"SELECT DevID,Produce,Electric,Gas,GoodProduct FROM MonthDeviceData WHERE Day = '{Nowtime}' AND DevID = {i}";
                    DataTable monthdata = SQLiteHelp.ExecuteQuery(sql);
                    
                    // 判断是否有数据
                    if (monthdata.Rows.Count > 0)
                    {
                        double produce = double.Parse(monthdata.Rows[0][1].ToString());
                        double electric = double.Parse(monthdata.Rows[0][2].ToString());
                        double gas = double.Parse(monthdata.Rows[0][3].ToString());
                        double goodproduct = double.Parse(monthdata.Rows[0][4].ToString());
                        
                        // 如果程序运行时长较长，判断是否跨天，比较当前天和之前的天数,添加新一天的数据
                        //如果是月末，则比较月份的大小，清空上月数据，重新添加数据
                        if (NowMonth > Month)
                        {
                            // 初始化数据
                            DeviceChart[i].DevReachValue.Clear();
                            DeviceChart[i].DevQualityValue.Clear();
                            DeviceChart[i].ConsumePower.Clear();
                            DeviceChart[i].ConsumptionGas.Clear();
                            DeviceChart[i].ProductionSum = 0.ToString();
                            //DeviceChart[i].GoodProduce = 0.ToString("p2");
                            DeviceChart[i].GoodProduce = 1.ToString("p2");
                            DeviceChart[i].Efficiency = 0.ToString("p2");
                            DeviceChart[i].FontColor = "#4caf50";
                            DeviceChart[i].FontBgc = "#cbffcd";
                            DeviceChart[i].FontText = Icon01;
                            DeviceChart[i].AxisXMax = 10;
                            DeviceChart[i].AxisXMin = 0;
                            Day = 0;
                        }
                        // 判断天数大小
                        if (NowDay > Day)
                        {
                            if (monthdata.Rows[0][1].ToString() == "")
                            {
                                DeviceChart[i].DevReachValue.Add(0);
                                DeviceChart[i].DevQualityValue.Add(0);
                                DeviceChart[i].ConsumePower.Add(0);
                                DeviceChart[i].ConsumptionGas.Add(0);
                                DeviceChart[i].AxisXMax = NowDay > 10 ? NowDay : 10;
                                DeviceChart[i].AxisXMin = NowDay - 10 > 0 ? NowDay - 10 : 0;
                            }
                            else
                            {
                                //添加达成率数据
                                DeviceChart[i].DevReachValue.Add(double.Parse((produce / MainWindow.DayPlan).ToString("N3")));
                                //添加良率数据
                                DeviceChart[i].DevQualityValue.Add(double.Parse((goodproduct / produce).ToString("N3")));
                                //添加电消耗数据
                                DeviceChart[i].ConsumePower.Add(electric);
                                //添加气消耗数据
                                DeviceChart[i].ConsumptionGas.Add(gas);
                                //更新X轴显示大小
                                DeviceChart[i].AxisXMax = NowDay > 10 ? NowDay : 10;
                                DeviceChart[i].AxisXMin = NowDay - 10 > 0 ? NowDay - 10 : 0;
                            }
                            Day = NowDay;
                        }
                        else
                        {
                            //更新达成率数据
                            DeviceChart[i].DevReachValue[NowDay - 1] = double.Parse((produce / MainWindow.DayPlan).ToString("N3"));
                            //更新良率数据
                            DeviceChart[i].DevQualityValue[NowDay - 1] = double.Parse((goodproduct / produce).ToString("N3"));
                            //更新电消耗数据
                            DeviceChart[i].ConsumePower[NowDay - 1] = electric;
                            //更新气消耗数据
                            DeviceChart[i].ConsumptionGas[NowDay - 1] = gas;
                            //更新X轴显示大小
                            DeviceChart[i].AxisXMax = NowDay > 10 ? NowDay : 10;
                            DeviceChart[i].AxisXMin = NowDay - 10 > 0 ? NowDay - 10 : 0;
                        }
                    }
                }
            });
            //触发间隔
            MonthtimerNotice.Interval = MainWindow.Time;
            // 开始
            MonthtimerNotice.Start();
        }

        #endregion

        /// <summary>
        /// 日点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Day_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MonthtimerNotice != null)
            {
                //结束月份的计时
                MonthtimerNotice.Stop();
            }
            DayBackcolor = Clickcolor;
            MonthBackcolor = Buttoncolor;
            DevTitle = "当日设备信息图表";

            //运行添加设备当天数据函数
            if (DeviceChart != null)
            {
                GetDayDeviceList();
            }
        }

        /// <summary>
        /// 月点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Month_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //结束天数据的计时
            DaytimerNotice.Stop();
            DayBackcolor = Buttoncolor;
            MonthBackcolor = Clickcolor;
            DevTitle = "当月设备信息图表";
            //运行添加设备当月数据函数
            GetMonthDeviceList();
            //触发间隔刷新数据函数
            ChangeMonthData();
        }

        #endregion

        #region 定义页面绑定信息字段
        /// <summary>
        /// 饼状图标签
        /// </summary>
        public Func<ChartPoint, string> PointLabel { get; set; }

        /// <summary>
        /// 天统计的饼状图数据
        /// </summary>
        public SeriesCollection DayPieChart
        {
            get;
            set;
        } = new SeriesCollection();

        /// <summary>
        /// 按月统计的饼状图数据
        /// </summary>
        public SeriesCollection MonthPieChart
        {
            get;
            set;
        } = new SeriesCollection();

        /// <summary>
        /// 按小时统计的饼状图数据
        /// </summary>
        public SeriesCollection HourPieChart
        {
            get;
            set;
        } = new SeriesCollection();

        public ObservableCollection<DeviceList> deviceLists;
        /// <summary>
        /// 页面绑定设备信息字段
        /// </summary>
        public ObservableCollection<DeviceList> DeviceChart
        {
            get
            {
                return deviceLists;
            }
            set
            {
                deviceLists = value;
                OnPropertyChanged(nameof(DeviceChart));
            }
        }
        /// <summary>
        /// 按钮背景色
        /// </summary>
        public string backcolor;
        /// <summary>
        /// 按钮绑定背景色
        /// </summary>
        public string DayBackcolor
        {
            get
            {
                return backcolor;
            }
            set
            {
                backcolor = value;
                OnPropertyChanged(nameof(DayBackcolor));
            }
        }
        /// <summary>
        /// 按钮背景色
        /// </summary>
        public string monthbackcolor;
        /// <summary>
        /// 按钮绑定背景色
        /// </summary>
        public string MonthBackcolor
        {
            get
            {
                return monthbackcolor;
            }
            set
            {
                monthbackcolor = value;
                OnPropertyChanged(nameof(MonthBackcolor));
            }
        }

        /// <summary>
        /// 图表标题
        /// </summary>
        public string devtitle;
        /// <summary>
        /// 页面设备信息图表标题 （当 天/月 设备信息图表）
        /// 位于 天/月 按钮切换同一行
        /// </summary>
        public string DevTitle
        {
            get
            {
                return devtitle;
            }
            set
            {
                devtitle = value;
                OnPropertyChanged(nameof(DevTitle));
            }
        }
        #endregion
        
        /// <summary>
        /// 属性变更通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变更函数
        /// </summary>
        /// <param name="propertyName">属性名</param>
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #region 定义页面图表设备信息字段
    /// <summary>
    /// 设备信息字段列表
    /// </summary>
    public class DeviceList : INotifyPropertyChanged
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName
        {
            get;
            set;
        }

        /// <summary>
        /// 设备产量
        /// </summary>
        public string deviceyield;
        /// <summary>
        /// 设备产量
        /// </summary>
        public string ProductionSum
        {
            get
            {
                return deviceyield;
            }
            set
            {
                deviceyield = value;
                OnPropertyChanged(nameof(ProductionSum));
            }
        }

        /// <summary>
        /// 生产效率
        /// </summary>
        public string efficiency;
        /// <summary>
        /// 生产效率
        /// </summary>
        public string Efficiency
        {
            get
            {
                return efficiency;
            }
            set
            {
                efficiency = value;
                OnPropertyChanged(nameof(Efficiency));
            }
        }

        /// <summary>
        /// 良率
        /// </summary>
        public string goodproduce;
        /// <summary>
        /// 良品率
        /// </summary>
        public string GoodProduce
        {
            get
            {
                return goodproduce;
            }
            set
            {
                goodproduce = value;
                OnPropertyChanged(nameof(GoodProduce));
            }
        }

        public string devqualitytitle;
        /// <summary>
        /// 页面绑定良品率图表标题 （当 天/月 良品率图表）
        /// </summary>
        public string DevQualityTitle
        {
            get
            {
                return devqualitytitle;
            }
            set
            {
                devqualitytitle = value;
                OnPropertyChanged(nameof(DevQualityTitle));
            }
        }

        /// <summary>
        /// 良品率数据
        /// </summary>
        public GearedValues<double> devqualityvalue;
        /// <summary>
        /// 页面绑定良品率图表数据
        /// </summary>
        public GearedValues<double> DevQualityValue
        {
            get
            {
                return devqualityvalue;
            }
            set
            {
                devqualityvalue = value;
                OnPropertyChanged(nameof(DevQualityValue));
            }
        }

        public string powergastitle;
        /// <summary>
        /// 页面绑定电气消耗图表标题 （当 天/月 电气消耗量图表）
        /// </summary>
        public string PowerGasTitle
        {
            get
            {
                return powergastitle;
            }
            set
            {
                powergastitle = value;
                OnPropertyChanged(nameof(PowerGasTitle));
            }
        }

        /// <summary>
        /// 用电量数据
        /// </summary>
        public ChartValues<double> power;
        /// <summary>
        /// 页面绑定图表耗电量数据
        /// </summary>
        public ChartValues<double> ConsumePower
        {
            get
            {
                return power;
            }
            set
            {
                power = value;
                OnPropertyChanged(nameof(ConsumePower));
            }
        }

        /// <summary>
        /// 用气量数据
        /// </summary>
        public ChartValues<double> gas;
        /// <summary>
        /// 页面绑定图表用气量数据
        /// </summary>
        public ChartValues<double> ConsumptionGas
        {
            get
            {
                return gas;
            }
            set
            {
                gas = value;
                OnPropertyChanged(nameof(ConsumptionGas));
            }
        }

        public string devreachtitle;
        /// <summary>
        /// 页面绑定达成率图表标题 （当 天/月 达成率图表）
        /// </summary>
        public string DevReachTitle
        {
            get
            {
                return devreachtitle;
            }
            set
            {
                devreachtitle = value;
                OnPropertyChanged(nameof(DevReachTitle));
            }
        }
        
        /// <summary>
        /// 达成率
        /// </summary>
        public GearedValues<double> devreach;
        /// <summary>
        /// 页面绑定达成率图表数据
        /// </summary>
        public GearedValues<double> DevReachValue
        {
            get
            {
                return devreach;
            }
            set
            {
                devreach = value;
                OnPropertyChanged(nameof(DevReachValue));
            }

        }

        /// <summary>
        /// 文字显示颜色
        /// </summary>
        public string fontcolor;
        /// <summary>
        /// 文字显示颜色
        /// </summary>
        public string FontColor
        {
            get
            {
                return fontcolor;
            }
            set
            {
                fontcolor = value;
                OnPropertyChanged(nameof(FontColor));
            }
        }
        
        /// <summary>
        /// ICON显示样式
        /// </summary>
        public string fonttext;
        /// <summary>
        /// ICON显示样式
        /// </summary>
        public string FontText
        {
            get
            {
                return fonttext;
            }
            set
            {
                fonttext = value;
                OnPropertyChanged(nameof(FontText));
            }
        }

        /// <summary>
        /// ICON背景色
        /// </summary>
        public string fontbgc;
        /// <summary>
        /// ICON背景色
        /// </summary>
        public string FontBgc
        {
            get
            {
                return fontbgc;
            }
            set
            {
                fontbgc = value;
                OnPropertyChanged(nameof(FontBgc));
            }
        }

        /// <summary>
        /// X轴显示的最小值
        /// </summary>
        public int axismin;
        /// <summary>
        /// X轴显示的最小值
        /// </summary>
        public int AxisXMin
        {
            get
            {
                return axismin;
            }
            set
            {
                axismin = value;
                OnPropertyChanged(nameof(AxisXMin));
            }
        }

        /// <summary>
        /// 图表X轴显示的最大值
        /// </summary>
        public int axismax;
        /// <summary>
        /// 图表X轴显示的最大值
        /// </summary>
        public int AxisXMax
        {
            get
            {
                return axismax;
            }
            set
            {
                axismax = value;
                OnPropertyChanged(nameof(AxisXMax));
            }
        }

        public Func<double, string> formate;
        /// <summary>
        /// X轴格式化串
        /// </summary>
        public Func<double, string> XFormatter
        {
            get
            {
                return formate;
            }
            set
            {
                formate = value;
                OnPropertyChanged(nameof(XFormatter));
            }
        }

        public string charttitle;
        /// <summary>
        /// 页面图表X轴显示标题（时间/天   时间/小时）
        /// </summary>
        public string ChartXAxisTitle
        {
            get
            {
                return charttitle;
            }
            set
            {
                charttitle = value;
                OnPropertyChanged(nameof(ChartXAxisTitle));
            }
        }

        /// <summary>
        /// 每个设备数据显示的高度
        /// </summary>
        public double itemheight;
        /// <summary>
        /// 每个设备数据显示的高度
        /// </summary>
        public double ChartHeight
        {
            get
            {
                return itemheight;
            }
            set
            {
                itemheight = value;
                OnPropertyChanged(nameof(ChartHeight));
            }
        }

        /// <summary>
        /// 定义控件背景颜色
        /// </summary>
        public string BgColor
        {
            get;
            set;
        }

        /// <summary>
        /// 定义字体颜色
        /// </summary>
        public string FColor
        {
            get;
            set;
        }

        /// <summary>
        /// 属性变更通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变更函数
        /// </summary>
        /// <param name="propertyName">属性名</param>
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}
