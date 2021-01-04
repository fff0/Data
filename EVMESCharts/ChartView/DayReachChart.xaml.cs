using EVMESCharts.Sqlite;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

namespace EVMESCharts.Charts
{
    /// <summary>
    /// DayReachChart.xaml 的交互逻辑
    /// </summary>
    public partial class DayReachChart : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 折线图数据格式
        /// </summary>
        public class LineStatistics:INotifyPropertyChanged
        {
            /// <summary>
            /// X轴
            /// </summary>
            public int Num
            {
                get;
                set;
            }

            public double linevalue;
            /// <summary>
            /// Y轴
            /// </summary>
            public double Value
            {
                get
                {
                    return linevalue;
                }
                set
                {
                    linevalue = value;
                    OnPropertyChanged(nameof(Value));
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

        public sealed class DayChartData
        {
            /// <summary>
            /// 折线图数据组
            /// </summary>
            public GearedValues<LineStatistics> LineChartData
            {
                get;
                set;
            } = new GearedValues<LineStatistics>();
        }


        public DayReachChart()
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
        /// 设备数据集合
        /// </summary>
        public List<DayChartData> ChartDataList
        {
            get;
            set;
        } = new List<DayChartData>();

        /// <summary>
        /// 添加折线图数据
        /// </summary>
        public void AddLineCharts()
        {
            string devlidtsql = "SELECT ID FROM Equipment";
            DataTable devdata = SQLiteHelp.ExecuteQuery(devlidtsql);

            //获取当前设备编号列表
            var DevNumberList = SQLiteHelp.NumberList(devdata, "ID");

            // 添加设备个数
            foreach (var id in DevNumberList)
            {
                BrushConverter brushconverter = new BrushConverter();
                Brush color = (Brush)brushconverter.ConvertFromString(MainWindow.ColorList[id % MainWindow.ColorList.Length]);
                color.Opacity = 0.1;
                UserLineCharts.Add(new GLineSeries
                {
                    Values = new GearedValues<double>(),
                    DataLabels = false,
                    Title = MainWindow.DeviceList[id],
                    LineSmoothness = 0.3,
                    StrokeThickness = 1,
                    Stroke = (Brush)brushconverter.ConvertFromString(MainWindow.ColorList[id % MainWindow.ColorList.Length]),
                    Fill = color,
                });
                AddChartValue(id);    // 执行添加主页面折线图数据函数
                OnPropertyChanged(nameof(UserLineCharts));
            }
        }

        /// <summary>
        /// 存储各个设备字段的长度
        /// </summary>
        public Dictionary<int, int> DevLength
        {
            get;
            set;
        } = new Dictionary<int, int>();
        /// <summary>
        /// System.Timers.Timer
        /// </summary>
        private Timer timerNotice;

        /// <summary>
        /// 添加设备图表折线图数据
        /// </summary>
        public void AddChartValue(int num)
        {
            //初始化长度值
            DevLength.Add(num , 0);
            
            //添加初始化设备数据
            ChartDataList.Add(new DayChartData());

            var Daytime = DateTime.Today.ToShortDateString();
            string Sql = $"SELECT Time,Produce FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = {num}";
            //获取数据库数据
            DataTable data = SQLiteHelp.ExecuteQuery(Sql);
            // 存储长度值
            DevLength[num] = data.Rows.Count;
            if (data.Rows.Count > 0)
            {
                // 遍历读取的数据
                foreach (DataRow dr in data.Rows)
                {
                    int Time = int.Parse(dr["Time"].ToString());
                    double Produce = double.Parse(dr["Produce"].ToString());
                    ChartDataList[num].LineChartData.Add(new LineStatistics
                    {
                        Num = Time,
                        Value = double.Parse((Produce / MainWindow.HourPlan).ToString("N3"))
                    });
                }
            }
            if (ChartDataList[0].LineChartData.Count > 20)
            {
                AxisXMax = ChartDataList[0].LineChartData.Count;
                AxisXMin = ChartDataList[0].LineChartData.Count - 20;
            }
            else
            {
                AxisXMax = 20;
                AxisXMin = 0;
            }
            // 添加设备折线图数据
            UserLineCharts[num].Values = ChartDataList[num].LineChartData;

            timerNotice = new System.Timers.Timer();
            // 每隔一段时间触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                var NowDaytime = DateTime.Today.ToShortDateString();
                string sql = $"SELECT Time,Produce FROM DayDeviceData WHERE Day = '{NowDaytime}' AND DevID = {num}";
                DataTable RunData = SQLiteHelp.ExecuteQuery(sql);

                // 如果运行时长超过一天，则清空图表
                if (NowDaytime != Daytime)
                {
                    ChartDataList[num].LineChartData.Clear();
                    DevLength[num] = 0;
                }
                else
                {
                    if (RunData.Rows.Count > DevLength[num])
                    {
                        ChartDataList[num].LineChartData.Add(new LineStatistics
                        {
                            Num = int.Parse(RunData.Rows[RunData.Rows.Count - 1][0].ToString()),
                            Value = double.Parse((double.Parse(RunData.Rows[RunData.Rows.Count - 1][1].ToString()) / MainWindow.HourPlan).ToString("N3"))
                        });
                        DevLength[num] = RunData.Rows.Count;
                    }
                    else
                    {
                        if (RunData.Rows.Count - 1 >= 0)
                        {
                            ChartDataList[num].LineChartData[RunData.Rows.Count - 1].Value = 
                            double.Parse((double.Parse(RunData.Rows[RunData.Rows.Count - 1][1].ToString()) / MainWindow.HourPlan).ToString("N3"));
                        }
                        
                    }
                }

                // 判断是否点击了显示所有按钮，改变X轴显示的大小
                if (IsShowAll)
                {
                    AxisXMax = 24;
                    AxisXMin = 0;
                }
                else
                {
                    if (ChartDataList[0].LineChartData.Count > 20)
                    {
                        AxisXMax = ChartDataList[0].LineChartData.Count;
                        AxisXMin = ChartDataList[0].LineChartData.Count;
                    }
                    else
                    {
                        AxisXMax = 20;
                        AxisXMin = 0;
                    }
                }
            });

            //触发间隔
            timerNotice.Interval = MainWindow.Time;
            //timerNotice.Start();
        }

        /// <summary>
        /// 判断是否显示所有数据
        /// </summary>
        public bool IsShowAll
        {
            get;
            set;
        } = false;

        /// <summary>
        /// 还原图表显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetClick(object sender, System.Windows.RoutedEventArgs e)
        {
            IsShowAll = false;
            if (ChartDataList[0].LineChartData.Count > 20)
            {
                AxisXMax = ChartDataList[0].LineChartData.Count;
                AxisXMin = ChartDataList[0].LineChartData.Count - 20;
            }
            else
            {
                AxisXMax = 20;
                AxisXMin = 0;
            }
        }

        /// <summary>
        /// 显示所有数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            IsShowAll = true;
            AxisXMax = 24;
            AxisXMin = 0;
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
        } = (value) => value.ToString($"{0}':00'");

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
