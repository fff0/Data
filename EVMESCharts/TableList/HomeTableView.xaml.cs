using EVMESCharts.ChartView;
using EVMESCharts.DataSource;
using EVMESCharts.Popup;
using EVMESCharts.Sqlite;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace EVMESCharts.TableList
{
    /// <summary>
    /// HomeTableView.xaml 的交互逻辑
    /// </summary>
    public partial class HomeTableView : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 按钮不点击时的颜色
        /// </summary>
        public string Color = "#f9f9fc";
        /// <summary>
        /// 按钮点击时的颜色
        /// </summary>
        public string Click = "#cacaca";
        
        /// <summary>
        /// 饼状图图表
        /// </summary>
        public UserControl userpiecharts;
        /// <summary>
        /// 页面绑定饼状图图表
        /// </summary>
        public UserControl UserPieCharts
        {
            get
            {
                return userpiecharts;
            }
            set
            {
                userpiecharts = value;
                OnPropertyChanged(nameof(UserPieCharts));
            }
        }

        /// <summary>
        /// 主页面告警及饼状图模块
        /// </summary>
        public HomeTableView()
        {
            InitializeComponent();

            var DayTime = DateTime.Today.Date.ToShortDateString();
            var HourTime = DateTime.Now.Hour.ToString();
            string hoursql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}' AND Time = {HourTime}";
            DataTable HourData = SQLiteHelp.ExecuteQuery(hoursql);
            if (HourData.Rows.Count > 0)
            {
                // 默认显示小时数据
                UserPieCharts = new BreakDownPieChart("Hour", "Bottom");
            }
            else
            {
                UserPieCharts = new NoData();
            }

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            // 告警信息数据显示
            this.DataGrid.DataContext = new WarningList();

            this.DataContext = this;
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
        /// 小时点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hour_Checked(object sender, RoutedEventArgs e)
        {
            //按钮颜色
            HourBackcolor = Click;
            DayBackcolor = Color;
            MonthBackcolor = Color;

            var DayTime = DateTime.Today.Date.ToShortDateString();
            var HourTime = DateTime.Now.Hour.ToString();
            string hoursql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}' AND Time = {HourTime}";
            DataTable HourData = SQLiteHelp.ExecuteQuery(hoursql);
            if (HourData.Rows.Count > 0)
            {
                // 默认显示小时数据
                UserPieCharts = new BreakDownPieChart("Hour", "Bottom");
            }
            else
            {
                UserPieCharts = new NoData();
            }
        }

        /// <summary>
        /// 天点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Day_Checked(object sender, RoutedEventArgs e)
        {
            //按钮颜色
            HourBackcolor = Color;
            DayBackcolor = Click;
            MonthBackcolor = Color;

            var DayTime = DateTime.Today.Date.ToShortDateString();
            string daysql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}'";
            DataTable DayData = SQLiteHelp.ExecuteQuery(daysql);
            if (DayData.Rows.Count > 0)
            {
                // 默认显示小时数据
                UserPieCharts = new BreakDownPieChart("Day", "Bottom");
            }
            else
            {
                UserPieCharts = new NoData();
            }
        }

        /// <summary>
        /// 月点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Month_Checked(object sender, RoutedEventArgs e)
        {
            //按钮颜色
            HourBackcolor = Color;
            DayBackcolor = Color;
            MonthBackcolor = Click;

            string monthsql = $"SELECT 1 FROM FaultData";
            DataTable MonthData = SQLiteHelp.ExecuteQuery(monthsql);
            if (MonthData.Rows.Count > 0)
            {
                UserPieCharts = new BreakDownPieChart("Month", "Bottom");
            }
            else
            {
                UserPieCharts = new NoData();
            }
        }
        
        /// <summary>
        /// 显示全部故障信息按钮鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllMouseEnter(object sender, MouseEventArgs e)
        {
            AllIsOpen = true;
        }

        /// <summary>
        /// 显示全部故障信息鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllMouseLeave(object sender, MouseEventArgs e)
        {
            AllIsOpen = false;
        }

        /// <summary>
        /// 显示全部故障
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllClick(object sender, RoutedEventArgs e)
        {
            FaultsList faults = new FaultsList();
            faults.ShowDialog();
        }

        public bool allisopen;
        /// <summary>
        /// 是否显示提示框
        /// </summary>
        public bool AllIsOpen
        {
            get
            {
                return allisopen;
            }
            set
            {
                allisopen = value;
                OnPropertyChanged(nameof(AllIsOpen));
            }
        }

        /// <summary>
        /// 按钮背景色
        /// </summary>
        public string hourbackcolor;
        /// <summary>
        /// 页面绑定背景色
        /// </summary>
        public string HourBackcolor
        {
            get
            {
                return hourbackcolor;
            }
            set
            {
                hourbackcolor = value;
                OnPropertyChanged(nameof(HourBackcolor));
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
