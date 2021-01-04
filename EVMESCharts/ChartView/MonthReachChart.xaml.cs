using EVMESCharts.Sqlite;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using System;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

namespace EVMESCharts.Charts
{
    /// <summary>
    /// MonthReachChart.xaml 的交互逻辑
    /// </summary>
    public partial class MonthReachChart : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 折线图数据格式
        /// </summary>
        public sealed class LineStatistics
        {
            /// <summary>
            /// X轴
            /// </summary>
            public int Num
            {
                get;
                set;
            }
            /// <summary>
            /// Y轴
            /// </summary>
            public double Value
            {
                get;
                set;
            }
        }
        public MonthReachChart()
        {
            InitializeComponent();
            //定义XY轴显示
            var mapper = Mappers.Xy<LineStatistics>()
             .X(model => model.Num)
             .Y(model => model.Value);
            Charting.For<LineStatistics>(mapper);

            // 默认X轴显示大小
            AxisXMax = 20;
            AxisXMin = 0;

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            //添加数据
            AddLineCharts();

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
        /// 折线图数据
        /// </summary>
        public SeriesCollection UserLineCharts
        {
            get;
            private set;
        } = new SeriesCollection();
        
        /// <summary>
        /// 添加折线图数据
        /// </summary>
        public void AddLineCharts()
        {
            string devlidtsql = "SELECT ID FROM Equipment";
            DataTable devdata = SQLiteHelp.ExecuteQuery(devlidtsql);

            //获取当前设备编号列表
            var DevNumberList = SQLiteHelp.NumberList(devdata, "ID");

            for (int i = 0; i < DevNumberList.Count; i++)
            {
                BrushConverter brushconverter = new BrushConverter();
                Brush color = (Brush)brushconverter.ConvertFromString(MainWindow.ColorList[i % MainWindow.ColorList.Length]);
                color.Opacity = 0.1;
                UserLineCharts.Add(new GLineSeries
                {
                    Values = new GearedValues<double>(),
                    DataLabels = false,
                    Title = MainWindow.DeviceList[i],
                    LineSmoothness = 0.3,
                    StrokeThickness = 1,
                    Stroke = (Brush)brushconverter.ConvertFromString(MainWindow.ColorList[i % MainWindow.ColorList.Length]),
                    Fill = color,
                });
                UserLineCharts[i].Values = AddChartValue(i);
                OnPropertyChanged(nameof(UserLineCharts));
            }
        }


        /// <summary>
        /// System.Timers.Timer
        /// </summary>
        private Timer timerNotice;

        /// <summary>
        /// 添加设备图表折线图数据
        /// </summary>
        public GearedValues<LineStatistics> AddChartValue(int num)
        {
            // 定义折线图数据结构
            GearedValues<LineStatistics> LineChartData = new GearedValues<LineStatistics>();
            // 获取当前天数
            var nowday = DateTime.Now.Day.ToString();
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();
            if (UserLineCharts[0].Values.Count > 20)
            {
                AxisXMax = UserLineCharts[0].Values.Count - 1;
                AxisXMin = UserLineCharts[0].Values.Count - 20;
            }
            else
            {
                AxisXMax = 20;
                AxisXMin = 0;
            }
            for (int i = 1; i < int.Parse(nowday) + 1; i++)
            {
                // 生成当前天数之前的日期 如果有日期没有数据，则添加图表数据为0
                string nowtime = nowyear + "/" + nowmonth + "/" + i;
                // 查询数据库
                string sql = $"SELECT DevID,Produce FROM MonthDeviceData WHERE Day = '{nowtime}' AND DevID = {num}";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    if (data.Rows[0][1].ToString() == "")
                    {
                        LineChartData.Add(new LineStatistics
                        {
                            Num = i - 1,
                            Value = 0
                        });
                    }
                    else
                    {
                        LineChartData.Add(new LineStatistics
                        {
                            Num = i - 1,
                            Value = double.Parse((double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan).ToString("N3"))
                        });
                    }
                }
                else
                {
                    LineChartData.Add(new LineStatistics
                    {
                        Num = i - 1,
                        Value = 0
                    });
                }
            }
            timerNotice = new System.Timers.Timer();
            //间隔触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {

            });
            //触发间隔
            timerNotice.Interval = MainWindow.Time;
            timerNotice.Start();
            return LineChartData;
        }

        /// <summary>
        /// 还原图表显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (UserLineCharts[0].Values.Count > 20)
            {
                AxisXMax = UserLineCharts[0].Values.Count - 1;
                AxisXMin = UserLineCharts[0].Values.Count - 20;
            }
            else
            {
                AxisXMax = 20;
                AxisXMin = 0;
            }
        }

        /// <summary>
        /// 重置按钮鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ResetIsOpen = true;
        }

        /// <summary>
        /// 重置按钮鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ResetIsOpen = false;
        }

        /// <summary>
        /// 显示所有数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            // 获取当前月的天数
            DateTime dtNow = DateTime.Now;
            int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            AxisXMax = days - 1;
            AxisXMin = 0;
        }

        /// <summary>
        /// 显示所有按钮鼠标滑过事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AllIsOpen = true;
        }

        /// <summary>
        /// 显示所有按钮鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AllIsOpen = false;
        }

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

        public int axismax;
        /// <summary>
        /// X轴显示的最大值
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
        /// <summary>
        /// X轴格式化串
        /// </summary>
        public Func<double, string> XFormatter
        {
            get;
            private set;
        } = (value) => (value+1).ToString($"{DateTime.Now.Month.ToString()}月{0}日");

        /// <summary>
        /// Y轴格式化串
        /// </summary>
        public Func<double, string> YFormatter
        {
            get;
            private set;
        } = (value) => value.ToString();

        public bool resetisopen;
        /// <summary>
        /// 是否显示重置提示框
        /// </summary>
        public bool ResetIsOpen
        {
            get
            {
                return resetisopen;
            }
            set
            {
                resetisopen = value;
                OnPropertyChanged(nameof(ResetIsOpen));
            }
        }

        public bool allisopen;
        /// <summary>
        /// 是否显示显示所有数据提示框
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
