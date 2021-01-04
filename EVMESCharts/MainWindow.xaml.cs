using EVMESCharts.Charts;
using EVMESCharts.ChartView;
using EVMESCharts.ModBus;
using EVMESCharts.Popup;
using EVMESCharts.Sqlite;
using EVMESCharts.TableList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls; 

namespace EVMESCharts
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region 定义全局常量
        
        /// <summary>
        /// 刷新页面间隔时间 2秒
        /// </summary>
        readonly public static int Time = 1 * 1000 * 2;

        /// <summary>
        /// 定义小时计划产量  (用于计算小时达成率)
        /// </summary>
        public static int HourPlan
        {
            get
            {
                string sql = "SELECT * FROM StandardData";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    int dayplan = int.Parse(data.Rows[data.Rows.Count - 1][1].ToString());
                    int hour = int.Parse(data.Rows[data.Rows.Count - 1][2].ToString());
                    return (dayplan / hour);
                }
                else
                {
                    return 1;
                }

            }
        }

        /// <summary>
        /// 定义天计划产量    (用于计算天达成率)
        /// </summary>
        public static int DayPlan
        {
            get
            {
                string sql = "SELECT * FROM StandardData";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    int plan = int.Parse(data.Rows[data.Rows.Count - 1][1].ToString());
                    return plan;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// 定义月计划产量    (用于计算月达成率)
        /// </summary>
        public static int MonthPlan
        {
            get
            {
                string sql = "SELECT * FROM StandardData";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    int dayplan = int.Parse(data.Rows[data.Rows.Count - 1][1].ToString());
                    int day = int.Parse(data.Rows[data.Rows.Count - 1][3].ToString());
                    return (dayplan * day);
                }
                else
                {
                    return 1;
                }

            }
        }

        /// <summary>
        /// 定义天计划开机时间  (用于计算时间稼动率)
        /// </summary>
        readonly public static int DayRunTime = 20;

        /// <summary>
        /// 定义小时最高耗电量  (用于判断耗电量是否超出)
        /// </summary>
        readonly public static double HourMaxEC = 20;

        /// <summary>
        /// 定义小时最低耗电量  (用于判断耗电量是否低于)
        /// </summary>
        readonly public static double HourMinEC = 10;

        /// <summary>
        /// 定义天最高耗电量    (用于判断耗电量是否超出)
        /// </summary>
        readonly public static double DayMaxEC = 300;

        /// <summary>
        /// 定义天最低耗电量    (用于判断耗电量是否低于)
        /// </summary>
        readonly public static double DayMinEC = 100;

        /// <summary>
        /// 定义小时最高气消耗量  (用于判断耗气量是否超出)
        /// </summary>
        readonly public static double HourMaxGC = 20;

        /// <summary>
        /// 定义小时最低气消耗量  (用于判断耗气量是否低于)
        /// </summary>
        readonly public static double HourMinGC = 10;

        /// <summary>
        /// 定义天最高气消耗量    (用于判断耗气量是否超出)
        /// </summary>
        readonly public static double DayMaxGC = 300;

        /// <summary>
        /// 定义天最低气消耗量    (用于判断耗气量是否低于)
        /// </summary>
        readonly public static double DayMinGC = 100;

        /// <summary>
        /// 定义折线图线条颜色和柱状图 (饼状图填充色)
        /// </summary>
        readonly public static string[] ColorList = new string[]
        {
            "#60acfc",    // 蓝
            "#32d3eb",    // 青
            "#5bc49f",    // 绿
            "#feb64d",    // 橙
            "#ff7c7c",    // 红
            "#9287e7"     // 紫
        };

        /// <summary>
        /// 定义折线图线条颜色和柱状图 (饼状图填充色)
        /// </summary>
        public static string[] LineColorList = new string[]
        {
            "#388e3c",
            "#f44336",
            "#58ffff",
        };

        /// <summary>
        /// 定义故障编号列表 (显示故障类型)
        /// </summary>
        readonly public static string[] FaultList = new string[]
        {
            "工位一故障次数",
            "工位二故障次数",
            "工位三故障次数",
            "工位四故障次数",
            "工位五故障次数"
        };

        /// <summary>
        /// 定义主图表显示
        /// </summary>
        readonly public static string[] DayChartList = new string[]
        {
            "达成率",
            "良率",
            "时间稼动率"
        };

        /// <summary>
        /// 所有告警信息
        /// </summary>
        readonly public static string[] FaultMessageList = new string[]
        {
            "弹夹工位故障次数",
            "上料模组故障次数",
            "上料皮带故障次数",
            "1#ROBOT故障次数",
            "2#ROBOT故障次数",
            "3#ROBOT故障次数",
            "上预压块故障次数",
            "铝套预压故障次数",
            "下预压块故障次数",
            "保压故障次数",
            "工位一转盘故障次数",
            "下产品工位故障次数",
            "1#烘烤故障次数",
            "2#烘烤故障次数",
            "3#烘烤故障次数",
            "4#烘烤故障次数",
            "5#烘烤故障次数",
            "冷却流水线故障次数",
            "镭雕故障次数",
            "保压故障次数",
            "水箱故障次数",
            "工位二转盘故障次数",
            "弹夹工位报警累计次数",
            "上料模组报警累计次数",
            "上料皮带报警累计次数",
            "机器人报警累计次数",
            "高周波报警累计次数",
            "工位三转盘工位报警累计次数",
            "上产品工位报警累积次数",
            "脱铝套工位报警累积次数",
            "脱外环工位报警累积次数",
            "贴双面胶工位报警累积次数",
            "飞达工位报警累积次数",
            "保压工位报警累积数",
            "工位四转盘工位报警累积数",
            "上产品模组报警累计计数",
            "撕双面胶工位报警累计计数",
            "弹夹工位报警累计计数",
            "取网布模组报警累计计数",
            "冲切工位报警累计计数",
            "下产品工位报警累计计数",
            "下废料工位报警累计计数",
            "转盘工位报警累计计数",
        };

        public static Dictionary<int, string> Devlist;
        /// <summary>
        /// 定义机器编号列表 (显示设备名字)
        /// </summary>
        public static Dictionary<int, string> DeviceList
        {
            get
            {
                Devlist = new Dictionary<int, string>();
                string sql = "SELECT * FROM Equipment";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);

                if (data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        Devlist.Add(int.Parse(data.Rows[i][0].ToString()), data.Rows[i][1].ToString());
                    }
                }
                return Devlist;
            }
        }

        /// <summary>
        /// 定义告警信息列表  (显示告警内容)
        /// </summary>
        readonly public static string[] WarningTypeList = new string[]
        {
            "未达成指定产量",
            "用电量高于正常值",
            "用电量低于正常值",
            "用气量高于正常值",
            "用气量低于正常值"
        };

        /// <summary>
        /// 窗口主题颜色  #f9f9fc
        /// </summary>
        public static string BackGroundColor = "#1b1c3e";
        /// <summary>
        /// 窗口文字颜色  #000
        /// </summary>
        public static string WindowFontColor = "#FFF";
        /// <summary>
        /// 控件背景颜色 #fff
        /// </summary>
        public static string WindowBgColor = "#303153";

        /// <summary>
        /// 定义时间饼状图数据
        /// </summary>
        public static string[] TimeList = new string[]
        {
            "运行时间",
            "停机时间",
            "故障时间"
        };

        /// <summary>
        /// 定义时间饼状图颜色
        /// </summary>
        readonly public static string[] TimeColorList = new string[]
        {
            "#5bc49f",    // 绿
            "#ff7c7c",    // 红
            "#ccc"        // 灰
        };
        #endregion

        ///// <summary>
        ///// 主页面显示的图表
        ///// </summary>
        //public UserControl Homepage = new Home();
        ///// <summary>
        ///// 主页面显示的设备告警信息列表
        ///// </summary>
        //public UserControl HomeTableView = new HomeTableView();
        ///// <summary>
        ///// 设备信息页显示的设备信息图表
        ///// </summary>
        //public UserControl DataCharts = new DataCharts();
        ///// <summary>
        ///// 设备信息页显示的告警信息图表
        ///// </summary>
        //public UserControl DataTableView = new DataTableView();
        
        /// <summary>
        /// 主页面
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // 获取页面右上角显示的时间
            GetNowTime();

            // 判断是否有数据库
            SQLiteHelp.FindDBPath();

            BackGround = BackGroundColor;
            FontColor = WindowFontColor;
            BgColor = WindowBgColor;

            GetModBusData();

            this.DataContext = this;
        }
        /// <summary>
        /// 图表显示内容
        /// </summary>
        public  UserControl userchart;

        /// <summary>
        /// 定义追背景颜色
        /// </summary>
        public string BackGround
        {
            get;
            set;
        }

        /// <summary>
        /// 定义字体颜色
        /// </summary>
        public string FontColor
        {
            get;
            set;
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
        /// 图表显示控件模块
        /// </summary>
        public  UserControl UserChartsList
        {
            get
            {
                return userchart;
            }
            set
            {
                userchart = value;
                OnPropertyChanged(nameof(UserChartsList));
            }
        }

        /// <summary>
        /// 告警模块
        /// </summary>
        public UserControl waringlist;
        /// <summary>
        /// 页面告警模块显示内容
        /// </summary>
        public UserControl UserWarningList
        {
            get
            {
                return waringlist;
            }
            set
            {
                waringlist = value;
                OnPropertyChanged(nameof(UserWarningList));
            }
        }

        public string nowtime;
        /// <summary>
        /// 页面绑定的时间
        /// </summary>
        public string NowTime
        {
            get
            {
                return nowtime;
            }
            set
            {
                nowtime = value;
                OnPropertyChanged(nameof(NowTime));
            }
        }

        /// <summary>
        /// 获取ModBus地址
        /// </summary>
        public void GetModBusData()
        {
            string sql = "SELECT * FROM IPData";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);

            // 判断是否有IP地址数据
            if (data.Rows.Count > 0)
            {
                try
                {
                    RunGetModBus();
                }
                catch (Exception)
                {
                    Message mes = new Message(2, "读取ModBus失败");
                    mes.ShowDialog();
                }
            }
            else
            {
                Message mes = new Message(2, "获取ModBus地址失败");
                mes.ShowDialog();
            }
        }

        /// <summary>
        /// 运行读取ModBus数据函数
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Task RunGetModBus()
        {
            bool IsRunning = true;

            return Task.Factory.StartNew(() => 
            {
                // 设置线程名称
                Thread.CurrentThread.Name = "存储数据";
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Priority = ThreadPriority.Normal;

                while (IsRunning)
                {
                    Thread.Sleep(2000);
                    try
                    {
                        // 运行读取ModBus函数
                        ModBusHelp.ReadModBus();
                    }
                    catch (Exception)
                    {

                        bool IsConnect = true;
                        while (IsConnect)
                        {
                            Thread.Sleep(5000);
                            try
                            {
                                ModBusHelp.ReadModBus();
                                IsConnect = false;
                                Console.WriteLine("重连成功");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("连接失败");
                            }
                        }
                    }
                }
            },TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// System.Timers.Timer (时间显示计时)
        /// </summary>
        private System.Timers.Timer timerNotice;
        /// <summary>
        /// 获取当前时间
        /// </summary>
        private void GetNowTime()
        {
            timerNotice = new System.Timers.Timer();
            //间隔触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                var Year =  DateTime.Now.ToLongDateString().ToString();
                var Time = DateTime.Now.ToLongTimeString().ToString();
                string nowTime = Year +"   "+ Time;
                NowTime = nowTime;
            });
            //触发间隔  1秒
            timerNotice.Interval = 1000;
            timerNotice.Start();
        }

        /// <summary>
        /// 主页面点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeData(object sender, RoutedEventArgs e)
        {
            //添加主页面展示图表
            UserChartsList = new Home();
            //添加主页面展示告警信息列表
            UserWarningList = new HomeTableView();
        }
        /// <summary>
        /// 设备数据点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceData(object sender, RoutedEventArgs e)
        {
            //添加设备数据图表
            UserChartsList = new DataCharts();
            //添加设备信息页的告警信息
            UserWarningList = new DataTableView();
        }
        /// <summary>
        /// 运行状态点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunStatusChecked(object sender, RoutedEventArgs e)
        {
            UserChartsList = new RunStatus();
            UserWarningList = new RunStatusTableView();
        }

        public bool Isopen;
        /// <summary>
        /// 提示框显示与隐藏
        /// </summary>
        public bool IPIsOpen
        {
            get
            {
                return Isopen;
            }
            set
            {
                Isopen = value;
                OnPropertyChanged(nameof(IPIsOpen));
            }
        }

        public bool chIsopen;
        /// <summary>
        /// 提示框的显示与隐藏
        /// </summary>
        public bool ChangeIsOpen
        {
            get
            {
                return chIsopen;
            }
            set
            {
                chIsopen = value;
                OnPropertyChanged(nameof(ChangeIsOpen));
            }
        }

        /// <summary>
        /// 设置IP鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IPMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IPIsOpen = true;
        }
        /// <summary>
        /// 设置IP鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IPMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IPIsOpen = false;
        }
        /// <summary>
        /// 设置IP按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IPClick(object sender, RoutedEventArgs e)
        {
            IPConfigure ipchange = new IPConfigure();
            ipchange.ShowDialog();
        }

        /// <summary>
        /// 鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeIsOpen = true;
        }
        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeIsOpen = false;
        }

        public bool AddIsopen;
        /// <summary>
        /// 添加设备提示框显示与隐藏
        /// </summary>
        public bool AddIsOpen
        {
            get
            {
                return AddIsopen;
            }
            set
            {
                AddIsopen = value;
                OnPropertyChanged(nameof(AddIsOpen));
            }
        }
        /// <summary>
        /// 添加按钮鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AddIsOpen = true;
        }

        /// <summary>
        /// 添加按钮鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AddIsOpen = false;
        }

        /// <summary>
        /// 添加设备按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClick(object sender, RoutedEventArgs e)
        {
            AddEquipment add = new AddEquipment();
            add.ShowDialog();
        }
        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            PlanPopup popup = new PlanPopup();
            popup.ShowDialog();
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
}
