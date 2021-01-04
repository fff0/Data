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
using System.Threading;
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
    /// RunStatus.xaml 的交互逻辑
    /// </summary>
    public partial class RunStatus : UserControl, INotifyPropertyChanged
    {
        public RunStatus()
        {
            InitializeComponent();

            // 格式化饼状图标签字符串
            //PointLabel = chartPoint =>
            //    string.Format("{1:P}", chartPoint.Y, chartPoint.Participation);
            PointLabel = chartPoint =>
               string.Format($"{Math.Floor(chartPoint.Y / 3600)}时{Math.Floor((chartPoint.Y - (Math.Floor(chartPoint.Y / 3600) * 3600)) / 60)}分 {chartPoint.Participation:P}");

            //添加初始化饼状图数据
            for (int i = 0; i < MainWindow.TimeList.Length; i++)
            {
                BrushConverter brushconverter = new BrushConverter();
                // 添加数据
                UserPieChart.Add(new PieSeries
                {
                    Title = MainWindow.TimeList[i % MainWindow.FaultList.Length],
                    Values = new ChartValues<double>(),
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    Fill = (Brush)brushconverter.ConvertFromString(MainWindow.TimeColorList[i % MainWindow.TimeColorList.Length]),
                    FontSize = 24,
                });
            }

            // 添加图表数据
            SetChartValue();

            // 设置页面主题颜色
            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            DataContext = this;
        }

        /// <summary>
        /// 设置页面展示数据
        /// </summary>
        private void SetChartValue()
        {
            var DayTime = DateTime.Today.ToShortDateString();

            // 添加饼状图数据
            string insertsql = $"SELECT TimeName,Hour,Minute,Second FROM TimeData WHERE Day = '{DayTime}'";
            //执行查询操作输出DataTable
            DataTable Insertdata = SQLiteHelp.ExecuteQuery(insertsql);

            if (Insertdata.Rows.Count > 0)
            {
                // 上电时间 时/分/秒
                int StartHour = int.Parse(Insertdata.Rows[0][1].ToString());
                int StartMin = int.Parse(Insertdata.Rows[0][2].ToString());
                int StartS = int.Parse(Insertdata.Rows[0][3].ToString());

                // 运行时间 时/分/秒
                int Hour = int.Parse(Insertdata.Rows[1][1].ToString());
                int Min = int.Parse(Insertdata.Rows[1][2].ToString());
                int Second = int.Parse(Insertdata.Rows[1][3].ToString());

                // 停机时间 时/分/秒
                int StopHour = int.Parse(Insertdata.Rows[2][1].ToString());
                int StopMin = int.Parse(Insertdata.Rows[2][2].ToString());
                int StopSecond = int.Parse(Insertdata.Rows[2][3].ToString());

                // 故障时间 时/分/秒
                int FaultHour = int.Parse(Insertdata.Rows[3][1].ToString());
                int FaultMin = int.Parse(Insertdata.Rows[3][2].ToString());
                int FaultSecond = int.Parse(Insertdata.Rows[3][3].ToString());

                // 添加饼状图数据
                for (int i = 0; i < UserPieChart.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            // 添加运行时间数据
                            int Time = Hour * 60 * 60 + Min * 60 + Second;

                            if (Insertdata.Rows.Count > 0)
                            {
                                UserPieChart[i].Values = new ChartValues<double>
                                {
                                    (double)Time
                                };
                            }
                            else
                            {
                                UserPieChart[i].Values = new ChartValues<double> { 0 };
                            }

                            break;
                        case 1:
                            // 添加停机时间数据
                            int StopTime = StopHour * 60 * 60 + StopMin * 60 + StopSecond;

                            if (Insertdata.Rows.Count > 0)
                            {
                                UserPieChart[i].Values = new ChartValues<double>
                                {
                                    (double)StopTime
                                };
                            }
                            else
                            {
                                UserPieChart[i].Values = new ChartValues<double> { 0 };
                            }
                            break;
                        case 2:
                            // 添加故障时间数据
                            int FaultTime = FaultHour * 60 * 60 + FaultMin * 60 + FaultSecond;

                            if (Insertdata.Rows.Count > 0)
                            {
                                UserPieChart[i].Values = new ChartValues<double>
                                {
                                    (double)FaultTime
                                };
                            }
                            else
                            {
                                UserPieChart[i].Values = new ChartValues<double> { 0 };
                            }
                            break;
                        default:
                            break;
                    }
                }

                // 添加故障率信息
                double FTime = FaultHour * 60 * 60 + FaultMin * 60 + FaultSecond;
                double StartTime = StartHour * 60 * 60 + StartMin * 60 + StartS;
                FaultValue = double.Parse((FTime / StartTime).ToString("N2")) * 100;

                // 添加页面下方上电信息
                TimeText = $"总上电时间：{StartHour}时{StartMin}分{StartS}秒";
            }


            // 添加当日计划生产数据
            PlanNumber = $"当日计划生产数：{MainWindow.DayPlan}";

            //添加生产数据
            string sql = $"SELECT DevID,Produce,GoodProduct FROM MonthDeviceData WHERE Day = '{DayTime}' AND DevID = {0}";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);

            // 判断是否有数据
            if (data.Rows.Count > 0)
            {
                double Produce = double.Parse(data.Rows[0][1].ToString());
                double GoodProduct = double.Parse(data.Rows[0][2].ToString());
                ProduceNumber = $"{Produce}";

                string prodata = (Produce / MainWindow.DayPlan).ToString("N2");
                string gooddata = "0";
                if (Produce > 0)
                {
                    gooddata = (GoodProduct / Produce).ToString("N2");
                }

                // 添加生产进度信息
                ProduceValue = double.Parse(prodata) * 100;

                // 添加良率信息
                GoodValue = double.Parse(gooddata) * 100;
            }
            else
            {
                // 添加生产进度信息
                ProduceValue = 0;
                // 添加良率信息
                GoodValue = 0;
            }

            // 定时刷新页面
            ChangeChartValue();
        }


        /// <summary>
        /// System.Timers.Timer 计时
        /// </summary>
        private System.Timers.Timer timerNotice;

        /// <summary>
        /// 定时器刷新页面
        /// </summary>
        public void ChangeChartValue()
        {
            var DayTime = DateTime.Today.ToShortDateString();

            timerNotice = new System.Timers.Timer();
            // 每隔一段时间触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                // 添加当日计划生产数据
                PlanNumber = $"当日计划生产数：{MainWindow.DayPlan}";
                // 添加饼状图数据
                string insertsql = $"SELECT TimeName,Hour,Minute,Second FROM TimeData WHERE Day = '{DayTime}'";
                //执行查询操作输出DataTable
                DataTable Insertdata = SQLiteHelp.ExecuteQuery(insertsql);

                string sql = $"SELECT DevID,Produce,GoodProduct FROM MonthDeviceData WHERE Day = '{DayTime}' AND DevID = {0}";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);
                if (data.Rows.Count > 0)
                {
                    double Produce = double.Parse(data.Rows[0][1].ToString());
                    double GoodProduct = double.Parse(data.Rows[0][2].ToString());
                    ProduceNumber = $"{Produce}";

                    string prodata = (Produce / MainWindow.DayPlan).ToString("N2");
                    string gooddata = "0";
                    if (Produce > 0)
                    {
                        gooddata = (GoodProduct / Produce).ToString("N2");
                    }

                    // 添加生产进度信息
                    ProduceValue = double.Parse(prodata) * 100;

                    // 添加良率信息
                    GoodValue = double.Parse(gooddata) * 100;
                }
                else
                {
                    // 添加生产进度信息
                    ProduceValue = 0;

                    // 添加良率信息
                    GoodValue = 0;
                }

                // 判断是否有数据
                if (Insertdata.Rows.Count > 0)
                {
                    // 上电时间 时/分/秒
                    int StartHour = int.Parse(Insertdata.Rows[0][1].ToString());
                    int StartMin = int.Parse(Insertdata.Rows[0][2].ToString());
                    int StartS = int.Parse(Insertdata.Rows[0][3].ToString());

                    // 运行时间 时/分/秒
                    int Hour = int.Parse(Insertdata.Rows[1][1].ToString());
                    int Min = int.Parse(Insertdata.Rows[1][2].ToString());
                    int Second = int.Parse(Insertdata.Rows[1][3].ToString());

                    // 停机时间 时/分/秒
                    int StopHour = int.Parse(Insertdata.Rows[2][1].ToString());
                    int StopMin = int.Parse(Insertdata.Rows[2][2].ToString());
                    int StopSecond = int.Parse(Insertdata.Rows[2][3].ToString());

                    // 故障时间 时/分/秒
                    int FaultHour = int.Parse(Insertdata.Rows[3][1].ToString());
                    int FaultMin = int.Parse(Insertdata.Rows[3][2].ToString());
                    int FaultSecond = int.Parse(Insertdata.Rows[3][3].ToString());
                    // 故障时间（秒）
                    double FTime = FaultHour * 60 * 60 + FaultMin * 60 + FaultSecond;
                    // 上电时间（秒）
                    double StartTime = StartHour * 60 * 60 + StartMin * 60 + StartS;
                    // 运行时间（秒）
                    double RunTime = Hour * 60 * 60 + Min * 60 + Second;
                    // 停机时间（秒）
                    double StopTime = StopHour * 60 * 60 + StopMin * 60 + StopSecond;

                    //添加故障率数据
                    FaultValue = double.Parse((FTime / StartTime).ToString("N2")) * 100;

                    //添加上电信息数据
                    TimeText = $"总上电时间：{StartHour}时{StartMin}分{StartS}秒";

                    // 添加饼状图数据
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                // 改变运行时长数据
                                UserPieChart[i].Values[0] = RunTime;
                                break;
                            case 1:
                                // 改变停机时长数据
                                UserPieChart[i].Values[0] = StopTime;
                                break;
                            case 2:
                                //改变停机时长数据
                                UserPieChart[i].Values[0] = FTime;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    // 添加无数据时的默认数据
                    FaultValue = 0;
                    TimeText = $"总上电时间：暂无数据";
                }
            });
            //触发间隔
            timerNotice.Interval = MainWindow.Time;
            timerNotice.Start();
        }

        /// <summary>
        /// 添加良品数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGoodClick(object sender, RoutedEventArgs e)
        {
            // 获取日期
            var NowDate = DateTime.Today.ToShortDateString();

            // 打开输入弹框
            AddGoodNumber addgoodnumber = new AddGoodNumber(NowDate);
            addgoodnumber.ShowDialog();
        }

        /// <summary>
        /// 饼状图标签
        /// </summary>
        public Func<ChartPoint, string> PointLabel
        {
            get;
            set;
        }

        /// <summary>
        /// 饼状图数据
        /// </summary>
        public SeriesCollection UserPieChart
        {
            get;
            set;
        } = new SeriesCollection();

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

        private double produce;
        /// <summary>
        /// 生产进度条数据
        /// </summary>
        public double ProduceValue
        {
            get
            {
                return produce;
            }
            set
            {
                produce = value;
                OnPropertyChanged(nameof(ProduceValue));
            }
        }

        private double good;
        /// <summary>
        /// 良率进度条数据
        /// </summary>
        public double GoodValue
        {
            get
            {
                return good;
            }
            set
            {
                good = value;
                OnPropertyChanged(nameof(GoodValue));
            }
        }

        private double fault;
        /// <summary>
        /// 故障率进度条数据
        /// </summary>
        public double FaultValue
        {
            get
            {
                return fault;
            }
            set
            {
                fault = value;
                OnPropertyChanged(nameof(FaultValue));
            }
        }

        public string producenumber;
        /// <summary>
        /// 页面绑定的生产数
        /// </summary>
        public string ProduceNumber
        {
            get
            {
                return producenumber;
            }
            set
            {
                producenumber = value;
                OnPropertyChanged(nameof(ProduceNumber));
            }
        }

        public string plannumber;
        /// <summary>
        /// 页面绑定的计划数
        /// </summary>
        public string PlanNumber
        {
            get
            {
                return plannumber;
            }
            set
            {
                plannumber = value;
                OnPropertyChanged(nameof(PlanNumber));
            }
        }

        public string timetext;
        /// <summary>
        /// 页面绑定的上电时间信息
        /// </summary>
        public string TimeText
        {
            get
            {
                return timetext;
            }
            set
            {
                timetext = value;
                OnPropertyChanged(nameof(TimeText));
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
