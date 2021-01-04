using EVMESCharts.Sqlite;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EVMESCharts.ChartView
{
    /// <summary>
    /// DayChart.xaml 的交互逻辑
    /// </summary>
    public partial class DayChart : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 折线图数据格式
        /// </summary>
        public class LineStatistics : INotifyPropertyChanged
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

        /// <summary>
        /// 折线图数据组格式
        /// </summary>
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

        /// <summary>
        /// 按天统计的主图表（达成率、效率、时间稼动率）
        /// </summary>
        /// <param name="Date">时间信息</param>
        public DayChart(string Date)
        {
            InitializeComponent();

            //定义XY轴显示
            var mapper = Mappers.Xy<LineStatistics>()
             .X(model => model.Num)
             .Y(model => model.Value);
            Charting.For<LineStatistics>(mapper);

            Daytime = Date;
            // 默认X轴显示大小
            AxisXMax = 20;
            AxisXMin = 0;

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            //添加数据
            AddDayChart();

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
        /// 设备数据集合
        /// </summary>
        public List<DayChartData> ChartDataList
        {
            get;
            set;
        } = new List<DayChartData>();

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
        public void AddDayChart()
        {
            //MainWindow.DayChartList
            for (int i = 0; i < MainWindow.DayChartList.Length; i++)
            {
                BrushConverter brushconverter = new BrushConverter();
                Brush color = (Brush)brushconverter.ConvertFromString(MainWindow.LineColorList[(i) % MainWindow.LineColorList.Length]);
                color.Opacity = 0.1;
                UserLineCharts.Add(new GLineSeries
                {
                    Values = new GearedValues<double>(),
                    DataLabels = false,
                    Title = MainWindow.DayChartList[i],
                    LineSmoothness = 0.3,
                    StrokeThickness = 2,
                    Stroke = (Brush)brushconverter.ConvertFromString(MainWindow.LineColorList[(i) % MainWindow.LineColorList.Length]),
                    Fill = color,
                });
                AddChartValue(i);    // 执行添加主页面折线图数据函数
                OnPropertyChanged(nameof(UserLineCharts));
            }
        }

        public string daytime;

        /// <summary>
        /// 页面展示的时间
        /// </summary>
        public string Daytime
        {
            get
            {
                return daytime;
            }
            set
            {
                daytime = value;
            }
        }

        /// <summary>
        /// 添加折线图数据
        /// </summary>
        /// <param name="i">需要展示的折线图类型</param>
        public void AddChartValue(int i)
        {
            // 添加初始化数据格式
            ChartDataList.Add(new DayChartData());
            switch (i)
            {
                case 0:
                    // 添加达成率图表信息
                    string Sql = $"SELECT Time,Reach FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = {0}";
                    //获取数据库数据
                    DataTable data = SQLiteHelp.ExecuteQuery(Sql);
                    if (data.Rows.Count > 0)
                    {
                        // 遍历读取的数据
                        foreach (DataRow dr in data.Rows)
                        {
                            int Time = int.Parse(dr["Time"].ToString());                       // 小时
                            double Reach = double.Parse(dr["Reach"].ToString());           // 生产总数
                            // 添加折线图数据
                            ChartDataList[0].LineChartData.Add(new LineStatistics
                            {
                                Num = Time,
                                Value = double.Parse((Reach / 100).ToString("N2"))
                            });
                        }
                    }
                    UserLineCharts[0].Values = ChartDataList[0].LineChartData;
                    break;
                case 1:
                    // 添加良率图表信息
                    string goodSql = $"SELECT Time,Produce,GoodProduct FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = {0}";
                    //获取数据库数据
                    DataTable Gooddata = SQLiteHelp.ExecuteQuery(goodSql);
                    if (Gooddata.Rows.Count > 0)
                    {
                        // 遍历读取的数据
                        foreach (DataRow dr in Gooddata.Rows)
                        {
                            int Time = int.Parse(dr["Time"].ToString());                         // 小时
                            double Produce = double.Parse(dr["Produce"].ToString());             // 生产总数
                            double GoodProduct = double.Parse(dr["GoodProduct"].ToString());     // 良品数
                            double Good;                                                         // 良率
                            if (Produce == 0 && GoodProduct == 0)
                            {
                                // 如果当天产量和良品数都为0，则将良率设为0
                                Good = 0;
                            }
                            else
                            {
                                Good = GoodProduct / Produce;
                            }
                            // 添加折线图数据
                            ChartDataList[1].LineChartData.Add(new LineStatistics
                            {
                                Num = Time,
                                Value = double.Parse(Good.ToString("N2"))
                            });
                        }
                    }
                    UserLineCharts[1].Values = ChartDataList[1].LineChartData;
                    break;
                case 2:
                    // 添加时间稼动率信息
                    string timeSql = $"SELECT Time,TimeEfficiency FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = {0}";
                    //获取数据库数据
                    DataTable Timedata = SQLiteHelp.ExecuteQuery(timeSql);
                    if (Timedata.Rows.Count > 0)
                    {
                        // 遍历读取的数据
                        foreach (DataRow dr in Timedata.Rows)
                        {
                            int Time = int.Parse(dr["Time"].ToString());                                 // 小时
                            double TimeEfficiency = double.Parse(dr["TimeEfficiency"].ToString());       // 时间稼动率
                            // 添加折线图数据
                            ChartDataList[i].LineChartData.Add(new LineStatistics
                            {
                                Num = Time,
                                Value = double.Parse((TimeEfficiency/100).ToString())
                            });
                        }
                    }
                    UserLineCharts[i].Values = ChartDataList[i].LineChartData;
                    break;
                default:
                    break;
            }

            ChangeChartData(i);
        }

        /// <summary>
        /// 存储各个图表数据的长度
        /// </summary>
        public Dictionary<int, int> ChartLength
        {
            get;
            set;
        } = new Dictionary<int, int>();

        /// <summary>
        /// System.Timers.Timer 计时
        /// </summary>
        private Timer timerNotice;

        /// <summary>
        /// 刷新图表数据
        /// </summary>
        /// <param name="i">需要展示的图表数据类型</param>
        public void ChangeChartData(int i)
        {
            switch (i)
            {
                case 0:
                    // 获取图表显示的折线图数据长度
                    int ProduceLength = ChartDataList[i].LineChartData.Count;
                    ChartLength.Add(i, ProduceLength);
                    break;
                case 1:
                    // 获取图表显示的折线图数据长度
                    int GoodLength = ChartDataList[i].LineChartData.Count;
                    ChartLength.Add(i, GoodLength);
                    break;
                case 2:
                    // 获取图表显示的折线图数据长度
                    int TimeLength = ChartDataList[i].LineChartData.Count;
                    ChartLength.Add(i, TimeLength);
                    break;
                default:
                    break;
            }

            timerNotice = new System.Timers.Timer();
            // 每隔一段时间触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                string sql = $"SELECT Time,Produce,Reach,TimeEfficiency,GoodProduct FROM DayDeviceData WHERE Day = '{Daytime}' AND DevID = {0}";
                DataTable RunData = SQLiteHelp.ExecuteQuery(sql);

                int Hour = int.Parse(DateTime.Now.Hour.ToString());
                // 如果查询有数据则运行循环函数
                if (RunData.Rows.Count > 0)
                {
                    double Reach = double.Parse(RunData.Rows[RunData.Rows.Count - 1][2].ToString());
                    double Produce = double.Parse(RunData.Rows[RunData.Rows.Count - 1][1].ToString());
                    double TimeEfficiency = double.Parse(RunData.Rows[RunData.Rows.Count - 1][3].ToString());
                    double GoodProduct = double.Parse(RunData.Rows[RunData.Rows.Count - 1][4].ToString());

                    // 如果图表数据长度与数据库数据长度不符，则清空图表
                    if (RunData.Rows.Count != ChartDataList[i].LineChartData.Count)
                    {
                        ChartDataList[i].LineChartData.Clear();
                        switch (i)
                        {
                            case 0:
                                for (int num = 0; num < RunData.Rows.Count; num++)
                                {
                                    // 添加达成率图表数据
                                    ChartDataList[i].LineChartData.Add(new LineStatistics
                                    {
                                        Num = num,
                                        Value = double.Parse((double.Parse(RunData.Rows[num][2].ToString()) / 100).ToString())
                                    });
                                }
                                break;
                            case 1:
                                for (int num = 0; num < RunData.Rows.Count; num++)
                                {
                                    // 添加良率图表数据
                                    if (double.Parse(RunData.Rows[num][4].ToString()) == 0 && double.Parse(RunData.Rows[num][1].ToString()) == 0)
                                    {
                                        // 如果生产数和良品数都为0，则将良率设为0
                                        ChartDataList[i].LineChartData.Add(new LineStatistics
                                        {
                                            Num = num,
                                            Value = 0.0
                                        });
                                    }
                                    else
                                    {
                                        // 添加良率数据
                                        ChartDataList[i].LineChartData.Add(new LineStatistics
                                        {
                                            Num = num,
                                            Value = double.Parse((double.Parse(RunData.Rows[num][4].ToString()) / double.Parse(RunData.Rows[num][1].ToString())).ToString("N2"))
                                        });
                                    }
                                }
                                break;
                            case 2:
                                for (int num = 0; num < RunData.Rows.Count; num++)
                                {
                                    // 添加时间稼动率图表数据
                                    ChartDataList[i].LineChartData.Add(new LineStatistics
                                    {
                                        Num = num,
                                        Value = double.Parse((double.Parse(RunData.Rows[num][3].ToString()) / 100).ToString())
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // 修改最后一个节点数据
                        switch (i)
                        {
                            case 0:
                                ChartDataList[i].LineChartData[ChartDataList[i].LineChartData.Count - 1].Value = double.Parse((Reach / 100).ToString());
                                break;
                            case 1:
                                if (GoodProduct == 0 && Produce == 0)
                                {
                                    ChartDataList[i].LineChartData[ChartDataList[i].LineChartData.Count - 1].Value = 0;
                                }
                                else
                                {
                                    ChartDataList[i].LineChartData[ChartDataList[i].LineChartData.Count - 1].Value = double.Parse((GoodProduct / Produce).ToString("N2"));
                                }
                                break;
                            case 2:
                                ChartDataList[i].LineChartData[ChartDataList[i].LineChartData.Count - 1].Value = double.Parse((TimeEfficiency / 100).ToString());
                                break;
                            default:
                                break;
                        }
                    }
                }
            });

            //触发间隔
            timerNotice.Interval = MainWindow.Time;
            timerNotice.Start();
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
        } = (value) => value.ToString("p0");

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
