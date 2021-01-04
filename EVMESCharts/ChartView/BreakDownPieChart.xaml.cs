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

namespace EVMESCharts.ChartView
{
    /// <summary>
    /// 饼状图数据
    /// </summary>
    public partial class BreakDownPieChart : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 循环刷新函数
        /// </summary>
        private System.Timers.Timer timerNotice = null;
        /// <summary>
        /// 故障饼状图图表
        /// </summary>
        /// <param name="Type">小时/天/月</param>
        /// <param name="Position">标签显示位置</param>
        public BreakDownPieChart(string Type,string Position)
        {
            InitializeComponent();
            // 格式化饼状图标签字符串
            PointLabel = chartPoint =>
                string.Format("{0}", chartPoint.Y, chartPoint.Participation);

            //添加初始化饼状图数据
            for (int i = 0; i < MainWindow.FaultList.Length; i++)
            {
                BrushConverter brushconverter = new BrushConverter();
                // 添加数据
                UserPieChart.Add(new PieSeries
                {
                    Title = MainWindow.FaultList[i % MainWindow.FaultList.Length],
                    Values = new ChartValues<double>(),
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    Fill = (Brush)brushconverter.ConvertFromString(MainWindow.ColorList[i % MainWindow.ColorList.Length])
                });
            }
            // 运行获取图表数据
            GetPieChartData(Type,Position);

            FontColor = MainWindow.WindowFontColor;

            this.DataContext = this;
        }

        /// <summary>
        /// 图表显示颜色
        /// </summary>
        public string FontColor
        {
            get;
            set;
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

        public string legend;
        /// <summary>
        /// 记录饼状图文本标签的位置
        /// </summary>
        public string UserLegendLocation
        {
            get
            {
                return legend;
            }
            set
            {
                legend = value;
                OnPropertyChanged(nameof(UserLegendLocation));
            }
        }

        /// <summary>
        /// 获取图表显示数据
        /// </summary>
        /// <param name="Type"></param>
        public void GetPieChartData(string Type,string Position)
        {
            // 图表标签显示位置
            switch (Position)
            {
                case "Bottom":
                    UserLegendLocation = "Bottom";
                    break;
                case "Right":
                    UserLegendLocation = "Right";
                    break;
                default:
                    UserLegendLocation = "Bottom";
                    break;
            }
            //时间
            var DayTime = DateTime.Today.Date.ToShortDateString();
            var HourTime = DateTime.Now.Hour.ToString();

            // 添加图表数据
            switch (Type)
            {
                case "Hour":
                    string sql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}' AND Time = {HourTime}";
                    //执行查询操作输出DataTable
                    DataTable data = SQLiteHelp.ExecuteQuery(sql);
                    //排序DataTable
                    DataView dv = data.DefaultView;
                    dv.Sort = "FaultID Asc";
                    DataTable hourdata = dv.ToTable();
                    // 添加饼状图数据
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        //当当天存在数据时
                        if (hourdata.Rows.Count > 0)
                        {
                            UserPieChart[i].Values = new ChartValues<double>
                                {
                                    double.Parse(hourdata.Rows[i][1].ToString())
                                };
                        }
                        else
                        {
                            UserPieChart[i].Values = new ChartValues<double>() { 0 };
                        }
                    }

                    // 循环函数（刷新数据）
                    timerNotice = new System.Timers.Timer();
                    timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, eea) =>
                    {
                        // 刷新图表数据
                        ChangePieData("Hour");
                    });
                    timerNotice.Interval = MainWindow.Time;
                    timerNotice.Start();
                   
                    break;
                    
                case "Day":
                    //图表数据
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        //对相同的故障类型累加
                        string daysql = $"SELECT FaultID,sum(Content) FROM FaultData WHERE Day = '{DayTime}' AND FaultID = {i}";
                        //查询数据库数据
                        DataTable daydata = SQLiteHelp.ExecuteQuery(daysql);
                        if (daydata.Rows[0][1].ToString() != "")
                        {
                            //添加饼状图数据
                            UserPieChart[i].Values = new ChartValues<double>
                            {
                                double.Parse(daydata.Rows[0][1].ToString())
                            };
                        }
                        else
                        {
                            //添加饼状图数据
                            UserPieChart[i].Values = new ChartValues<double> { 0 };
                        }
                    }

                    // 循环刷新
                    timerNotice = new System.Timers.Timer();
                    timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, eea) =>
                    {
                        // 刷新图表数据
                        ChangePieData("Day");
                    });
                    timerNotice.Interval = MainWindow.Time;
                    timerNotice.Start();
                    break;

                case "Month":
                    // 获取当前天数
                    var nowday = DateTime.Now.Day.ToString();
                    // 获取当前年份
                    var nowyear = DateTime.Now.Year.ToString();
                    // 获取当前月份
                    var nowmonth = DateTime.Now.Month.ToString();

                    // 添加按月统计各故障饼状图数据
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        int sum = 0;
                        // 循环当前的天数，累加得到当月数据
                        for (int j = 1; j < int.Parse(nowday) + 1; j++)
                        {
                            // 生成当前天数之前的日期
                            string nowtime = nowyear + "/" + nowmonth + "/" + j;
                            string monthsql = $"SELECT Day,sum(Content) FROM FaultData WHERE Day = '{nowtime}' AND FaultID = {i}";
                            DataTable monthdata = SQLiteHelp.ExecuteQuery(monthsql);
                            // 判断是否有数据
                            if (monthdata.Rows.Count > 0)
                            {
                                // 判断是否有数据累加 （无数据但有累加值时，读出的DataTable长度为1）
                                if (monthdata.Rows[0][1].ToString() == "")
                                {
                                    sum += 0;
                                }
                                else
                                {
                                    sum += int.Parse(monthdata.Rows[0][1].ToString());
                                }
                            }
                            else
                            {
                                sum += 0;
                            }
                        }
                        UserPieChart[i].Values = new ChartValues<double> { sum };
                    }
                    // 循环刷新
                    timerNotice = new System.Timers.Timer();
                    timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, eea) =>
                    {
                        // 刷新图表数据
                        ChangePieData("Month");
                    });
                    timerNotice.Interval = MainWindow.Time;
                    timerNotice.Start();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 刷新饼状图数据
        /// </summary>
        /// <param name="Type"></param>
        public void ChangePieData(string Type)
        {
            //图表数据
            var DayTime = DateTime.Today.Date.ToShortDateString();
            var HourTime = DateTime.Now.Hour.ToString();

            switch (Type)
            {
                case "Hour":
                    string sql = $"SELECT FaultID,Content FROM FaultData WHERE Day = '{DayTime}' AND Time = {HourTime}";
                    //执行查询操作输出DataTable
                    DataTable data = SQLiteHelp.ExecuteQuery(sql);
                    //排序DataTable
                    DataView dv = data.DefaultView;
                    dv.Sort = "FaultID Asc";
                    DataTable hourdata = dv.ToTable();
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        //当当天存在数据时
                        if (hourdata.Rows.Count > 0)
                        {
                            try
                            {
                                // 修改数据
                                UserPieChart[i].Values[0] = double.Parse(hourdata.Rows[i][1].ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    break;

                case "Day":
                    //图表数据
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        //对相同的故障类型累加
                        string daysql = $"SELECT FaultID,sum(Content) FROM FaultData WHERE Day = '{DayTime}' AND FaultID = {i}";
                        //查询数据库数据
                        DataTable daydata = SQLiteHelp.ExecuteQuery(daysql);
                        // 判断是否有累加数据
                        if (daydata.Rows[0][1].ToString() != "")
                        {
                            try
                            {
                                //添加饼状图数据
                                UserPieChart[i].Values[0] = double.Parse(daydata.Rows[0][1].ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    break;

                case "Month":
                    // 获取当前天数
                    var nowday = DateTime.Now.Day.ToString();
                    // 获取当前年份
                    var nowyear = DateTime.Now.Year.ToString();
                    // 获取当前月份
                    var nowmonth = DateTime.Now.Month.ToString();

                    // 添加按月统计各故障饼状图数据
                    for (int i = 0; i < UserPieChart.Count; i++)
                    {
                        double sum = 0;
                        for (int j = 1; j < int.Parse(nowday) + 1; j++)
                        {
                            // 生成当前天数之前的日期
                            string nowtime = nowyear + "/" + nowmonth + "/" + j;
                            string monthsql = $"SELECT Day,sum(Content) FROM FaultData WHERE Day = '{nowtime}' AND FaultID = {i}";
                            DataTable monthdata = SQLiteHelp.ExecuteQuery(monthsql);
                            // 判断当月是否有数据
                            if (monthdata.Rows.Count > 0)
                            {
                                // 判断当月是否有数据累加
                                if (monthdata.Rows[0][1].ToString() == "")
                                {
                                    sum += 0;
                                }
                                else
                                {
                                    sum += int.Parse(monthdata.Rows[0][1].ToString());
                                }
                            }
                            else
                            {
                                sum += 0;
                            }
                        }
                        try
                        {
                            // 改变数据
                            UserPieChart[i].Values[0] = sum;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    break;
                default:
                    break;
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
