using EVMESCharts.ChartView;
using EVMESCharts.DataSource;
using EVMESCharts.Popup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EVMESCharts.Charts
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 主页面图表
        /// </summary>
        public Home()
        {
            InitializeComponent();
            //QueryDate = DateTime.Today.ToShortDateString();
            //页面设备信息显示内容
            //this.ItemList.DataContext = new HomeItemList();

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            this.DataContext = this;
            //窗体卸载时，回收资源
            this.Unloaded += (sender, e) =>
            {
                GC.Collect();
            };
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
        /// 用于判断月点击还是日点击
        /// </summary>
        public bool IsDay
        {
            get;
            set;
        }

        /// <summary>
        /// 显示那种图表 true == 达成率图表   false == 产量柱状图图表
        /// </summary>
        public bool DisplayChart
        {
            get;
            set;
        } = true;
        
        #region 按钮点击事件

        /// <summary>
        /// 点击日统计按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DayChecked(object sender, RoutedEventArgs e)
        {
            DayBackcolor = "#cacaca";
            MonthBackcolor = "#f9f9fc";
            IsDay = true;
            
            var NowDate = QueryDate == null ? DateTime.Today.ToShortDateString() : QueryDate == DateTime.Today.ToShortDateString() ? DateTime.Today.ToShortDateString() : QueryDate;
            
            UserTable = new HomeDataTable(NowDate,"Day");
            if (DisplayChart)
            {
                //ReachCharts = new DayReachChart();
                //ChartTitle = "日达成率图表";
                ReachCharts = new DayChart(NowDate);
                ChartTitle = "日产线信息图表";
            }
            else
            {
                ReachCharts = new DayYieldChart(NowDate);
                ChartTitle = "日产量图表";
            }

        }

        /// <summary>
        /// 点击月统计按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonthChecked(object sender, RoutedEventArgs e)
        {
            DayBackcolor = "#f9f9fc";
            MonthBackcolor = "#cacaca";
            IsDay = false;
            var NowDate = DateTime.Today.ToShortDateString();
            UserTable = new HomeDataTable(NowDate,"Month");
            // 判断切换时改显示那种图表
            if (DisplayChart)
            {
                //ReachCharts = new MonthReachChart();
                //ChartTitle = "月达成率图表";
                ReachCharts = new MonthChart();
                ChartTitle = "月产线信息图表";
            }
            else
            {
                ReachCharts = new MonthYieldChart();
                ChartTitle = "月产量图表";
            }
        }

        /// <summary>
        /// 切换按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchClick(object sender, RoutedEventArgs e)
        {
            DisplayChart = !DisplayChart;
            var NowDate = QueryDate == null ? DateTime.Today.ToShortDateString() : QueryDate == DateTime.Today.ToShortDateString() ? DateTime.Today.ToShortDateString() : QueryDate;
            // 判断是否是日点击按钮
            if (IsDay)
            {
                UserTable = new HomeDataTable(NowDate, "Day");
                if (DisplayChart)
                {
                    //ReachCharts = new DayReachChart();
                    //ChartTitle = "日达成率图表";
                    ReachCharts = new DayChart(NowDate);
                    ChartTitle = "日产线信息图表";
                }
                else
                {
                    ReachCharts = new DayYieldChart(NowDate);
                    ChartTitle = "日产量图表";
                }
            }
            else
            {
                UserTable = new HomeDataTable(QueryDate,"Month");
                if (DisplayChart)
                {
                    //ReachCharts = new MonthReachChart();
                    //ChartTitle = "月达成率图表";
                    ReachCharts = new MonthChart();
                    ChartTitle = "月产线信息图表";
                }
                else
                {
                    ReachCharts = new MonthYieldChart();
                    ChartTitle = "月产量图表";
                }
            }
        }

        public string querydate;
        /// <summary>
        /// 记录查询的日期
        /// </summary>
        public string QueryDate
        {
            get
            {
                return querydate;
            }
            set
            {
                querydate = value;
            }
        }

        /// <summary>
        /// 查询历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query(object sender, RoutedEventArgs e)
        {
            string Date = this.UserDataPicker.Text;
            if (Date == "")
            {
                Message mes = new Message(1, "请选择查询的日期");
                mes.ShowDialog();
            }
            else
            {
                //在天点击按钮下才查询天数据
                if (IsDay)
                {
                    QueryDate = Date;
                    UserTable = new HomeDataTable(QueryDate, "Day");
                    // 判断是否点击了切换图表
                    if (DisplayChart)
                    {
                        ReachCharts = new DayChart(QueryDate);
                    }
                    else
                    {
                        ReachCharts = new DayYieldChart(QueryDate);
                    } 
                }
            }
        }

        /// <summary>
        /// 添加良品数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGoodClick(object sender, RoutedEventArgs e)
        {
            // 获取日期
            var NowDate = QueryDate == null ?
                DateTime.Today.ToShortDateString() : 
                QueryDate == DateTime.Today.ToShortDateString() ?
                DateTime.Today.ToShortDateString() : QueryDate;

            // 打开输入弹框
            AddGoodNumber addgoodnumber = new AddGoodNumber(NowDate);
            addgoodnumber.ShowDialog();
        }

        #endregion

        #region 鼠标事件

        /// <summary>
        /// 切换按纽鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchMouseEnter(object sender, MouseEventArgs e)
        {
            SwitchIsOpen = true;
        }

        /// <summary>
        /// 切换按钮鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchMouseLeave(object sender, MouseEventArgs e)
        {
            SwitchIsOpen = false;
        }
        #endregion

        #region 页面绑定的数据
       
        public bool switchisopen;
        /// <summary>
        /// 切换图表的显示与隐藏
        /// </summary>
        public bool SwitchIsOpen
        {
            get
            {
                return switchisopen;
            }
            set
            {
                switchisopen = value;
                OnPropertyChanged(nameof(SwitchIsOpen));
            }
        }
        
        /// <summary>
        /// 图表标题
        /// </summary>
        public string charttitle;
        /// <summary>
        /// 页面绑定图表标题
        /// </summary>
        public string ChartTitle
        {
            get
            {
                return charttitle;
            }
            set
            {
                charttitle = value;
                OnPropertyChanged(nameof(ChartTitle));
            }
        }

        /// <summary>
        /// 按钮背景色
        /// </summary>
        public string backcolor;
        /// <summary>
        /// 页面绑定背景色
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
        /// 页面绑定背景色
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
        /// 达成率图表
        /// </summary>
        public UserControl reachcharts;
        /// <summary>
        /// 页面绑定达成率图表
        /// </summary>
        public UserControl ReachCharts
        {
            get
            {
                return reachcharts;
            }
            set
            {
                reachcharts = value;
                OnPropertyChanged(nameof(ReachCharts));
            }
        }

        /// <summary>
        /// 表格
        /// </summary>
        public UserControl usertable;
        /// <summary>
        /// 页面绑定表格
        /// </summary>
        public UserControl UserTable
        {
            get
            {
                return usertable;
            }
            set
            {
                usertable = value;
                OnPropertyChanged(nameof(UserTable));
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
}
