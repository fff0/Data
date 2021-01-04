using EVMESCharts.Popup;
using EVMESCharts.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// HomeDataTable.xaml 的交互逻辑
    /// </summary>
    public partial class HomeDataTable : UserControl
    {
        /// <summary>
        /// 设备列表信息
        /// </summary>
        public ObservableCollection<TableData> DevNameList
        {
            get;
            set;
        } = new ObservableCollection<TableData>();
        
        /// <summary>
        /// 汇总信息
        /// </summary>
        public ObservableCollection<Summary> SummaryList
        {
            get;
            set;
        } = new ObservableCollection<Summary>();

        /// <summary>
        /// 添加表格数据
        /// </summary>
        /// <param name="Date">时间信息</param>
        /// <param name="Type">日统计/月统计</param>
        public HomeDataTable(string Date, string Type)
        {
            InitializeComponent();
            DayDate = Date;
            // 运行添加表格数据
            AddDataTableData(Type);
            
            DataContext = this;
        }

        public string daydate;
        /// <summary>
        /// 记录查询日期
        /// </summary>
        public string DayDate
        {
            get
            {
                return daydate;
            }
            set
            {
                daydate = value;
            }
        }

        /// <summary>
        /// 添加表格数据
        /// </summary>
        /// <param name="Type">天统计/月统计</param>
        public void AddDataTableData(string Type)
        {
            var Daytime = DateTime.Today.ToShortDateString();
            // 获取当前天数
            var nowday = DateTime.Now.Day.ToString();
            // 获取当前年份
            var nowyear = DateTime.Now.Year.ToString();
            // 获取当前月份
            var nowmonth = DateTime.Now.Month.ToString();
            switch (Type)
            {
                case "Day":
                    string sql = $"SELECT Time,Produce,Reach,TimeEfficiency,GoodProduct FROM DayDeviceData WHERE Day = '{DayDate}' AND DevID = {0}";
                    DataTable data = SQLiteHelp.ExecuteQuery(sql);

                    if (data.Rows.Count > 0)
                    {
                        TableLength = data.Rows.Count;
                        for (var i = 0; i < data.Rows.Count; i++)
                        {
                            double Produce = double.Parse(data.Rows[i][1].ToString());
                            double GoodProduct = double.Parse(data.Rows[i][4].ToString());
                            double GoodValue =  0;
                            if (Produce == 0 && GoodProduct == 0)
                            {
                                GoodValue = 0;
                            }
                            else
                            {
                                GoodValue = GoodProduct / Produce;
                            }
                            if (Produce != 0)
                            {
                                DevNameList.Add(new TableData()
                                {
                                    Time = $"{data.Rows[i][0]}:00",
                                    Efficiency = (double.Parse(data.Rows[i][2].ToString()) / 100).ToString("p0"),
                                    Good = (GoodValue).ToString("p0"),
                                    //Good = 1.ToString("p0"),
                                    TimeEfficiency = $"{data.Rows[i][3].ToString()}%"
                                });
                            }
                        }
                    }
                    break;
                case "Month":
                    for (int i = 1; i < int.Parse(nowday) + 1; i++)
                    {
                        // 生成当前天数之前的日期 如果有日期没有数据，则添加图表数据为0
                        string nowtime = nowyear + "/" + nowmonth + "/" + i;
                        // 查询数据库
                        string monthsql = $"SELECT Produce,TimeEfficiency,GoodProduct FROM MonthDeviceData WHERE Day = '{nowtime}' AND DevID = {0}";
                        DataTable Monthdata = SQLiteHelp.ExecuteQuery(monthsql);
                        if (Monthdata.Rows.Count > 0)
                        {
                            double Produce = double.Parse(Monthdata.Rows[0][0].ToString());
                            double GoodProduct = double.Parse(Monthdata.Rows[0][2].ToString());
                            double GoodValue = 0;
                            if (Produce == 0 && GoodProduct == 0)
                            {
                                GoodValue = 0;
                            }
                            else
                            {
                                GoodValue = GoodProduct / Produce;
                            }
                            if (Monthdata.Rows[0][1].ToString() != "")
                            {
                                DevNameList.Add(new TableData()
                                {
                                    Time = $"{nowmonth}月{i}日",
                                    Efficiency = (double.Parse(Monthdata.Rows[0][0].ToString()) / MainWindow.DayPlan).ToString("p0"),
                                    Good = GoodValue.ToString("p0"),
                                    TimeEfficiency = $"{Monthdata.Rows[0][1].ToString()}%"
                                });
                            }
                        }
                        //else
                        //{
                        //    DevNameList.Add(new TableData()
                        //    {
                        //        Time = $"{nowmonth}月{i}日",
                        //        Efficiency = 0.ToString("p0"),
                        //        //Good = data.Rows[i][3].ToString(),
                        //        Good = 0.ToString("p0"),
                        //        TimeEfficiency = $"{0}%"
                        //    });
                        //}
                    }
                        break;
                default:
                    break;
            }

            string allsql = $"SELECT Produce,TimeEfficiency,GoodProduct FROM MonthDeviceData WHERE Day = '{Daytime}' AND DevID = {0}";
            DataTable Alldata = SQLiteHelp.ExecuteQuery(allsql);
            if (Alldata.Rows.Count > 0)
            {
                double Produce = double.Parse(Alldata.Rows[0][0].ToString());
                double GoodProduct = double.Parse(Alldata.Rows[0][2].ToString());
                double TimeEfficiency = double.Parse(Alldata.Rows[0][1].ToString());
                double GoodValue = 0;
                if (Produce == 0 && GoodProduct == 0)
                {
                    GoodValue = 0;
                }
                else
                {
                    GoodValue = GoodProduct / Produce;
                }
                SummaryList.Add(new Summary()
                {
                    Title = "汇总",
                    Efficiency = (Produce / MainWindow.DayPlan).ToString("p0"),
                    Good = GoodValue.ToString("p0"),
                    TimeEfficiency = $"{TimeEfficiency}%"
                });
            }
            else
            {
                SummaryList.Add(new Summary()
                {
                    Title = "汇总",
                    Efficiency = (0).ToString("p0"),
                    Good = 0.ToString("p0"),
                    TimeEfficiency = "0%"
                });
            }
           

            // 定时刷新表格页面
            ChangeTableData(Type);
        }

        /// <summary>
        /// 存储表格数据的长度
        /// </summary>
        public int TableLength
        {
            get;
            set;
        }

        /// <summary>
        /// System.Timers.Timer 计时
        /// </summary>
        private System.Timers.Timer timerNotice;

        /// <summary>
        /// 改变表格数据函数
        /// </summary>
        /// <param name="Type">天统计/月统计</param>
        public void ChangeTableData(string Type)
        {
            string Day = DateTime.Today.ToShortDateString();
            // 只刷新天数据表格
            if (Type == "Day" && DayDate == Day)
            {
                timerNotice = new System.Timers.Timer();
                // 每隔一段时间触发函数
                timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
                {
                    string sql = $"SELECT Time,Produce,Reach,TimeEfficiency,GoodProduct FROM DayDeviceData WHERE Day = '{DayDate}' AND DevID = {0}";
                    DataTable Rundata = SQLiteHelp.ExecuteQuery(sql);
                    
                    // 判断有数据才刷新页面
                    if (Rundata.Rows.Count > 0)
                    {
                        double Produce = double.Parse(Rundata.Rows[Rundata.Rows.Count - 1][1].ToString());
                        string TimeEfficiency = Rundata.Rows[Rundata.Rows.Count - 1][3].ToString();
                        string Reach = Rundata.Rows[Rundata.Rows.Count - 1][2].ToString();
                        double GoodProduct = double.Parse(Rundata.Rows[Rundata.Rows.Count - 1][4].ToString());

                        double GoodValue = 0;
                        if (GoodProduct == 0 && Produce == 0)
                        {
                            GoodValue = 0;
                        }
                        else
                        {
                            GoodValue = GoodProduct / Produce;
                        }
                        // 当运行数据大于表格数据长度时，添加一个表格
                        if (Rundata.Rows.Count > TableLength)
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                System.Threading.SynchronizationContext.SetSynchronizationContext(new
                                    System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                                System.Threading.SynchronizationContext.Current.Post(p1 =>
                                {
                                    DevNameList.Clear();
                                    for (int i = 0; i < Rundata.Rows.Count ; i++)
                                    {
                                        double Efficiency = double.Parse(Rundata.Rows[i][2].ToString());
                                        double GoodProduce = double.Parse(Rundata.Rows[i][4].ToString());
                                        double NowProduce = double.Parse(Rundata.Rows[i][1].ToString());
                                        double value = 0;
                                        if (GoodProduce == 0 && NowProduce == 0)
                                        {
                                            value = 0;
                                        }
                                        else
                                        {
                                            value = GoodProduce / NowProduce;
                                        }
                                        if (NowProduce != 0)
                                        {
                                            //添加表格信息
                                            DevNameList.Add(new TableData()
                                            {
                                                Time = $"{Rundata.Rows[i][0]}:00",
                                                Efficiency = (Efficiency / 100).ToString("p0"),
                                                Good = (value).ToString("p0"),
                                                //Good = 1.ToString("p0"),
                                                TimeEfficiency = $"{Rundata.Rows[i][3]}%"
                                            });
                                        }
                                    }
                                    TableLength = Rundata.Rows.Count;
                                }, null);
                            });
                            
                        }
                        else
                        {
                            if (Produce > 0)
                            {
                                int Hour = int.Parse(DateTime.Now.Hour.ToString());
                                DevNameList[DevNameList.Count - 1].Time = $"{Hour}:00";
                                // 修改最后一个表格数据
                                DevNameList[DevNameList.Count - 1].Efficiency = (double.Parse(Reach) / 100).ToString("p0");
                                DevNameList[DevNameList.Count - 1].Good = (GoodValue).ToString("p0");
                                DevNameList[DevNameList.Count - 1].TimeEfficiency = $"{TimeEfficiency}%";
                            }
                        }
                    }


                    string allsql = $"SELECT Produce,TimeEfficiency,GoodProduct FROM MonthDeviceData WHERE Day = '{DayDate}' AND DevID = {0}";
                    DataTable Alldata = SQLiteHelp.ExecuteQuery(allsql);
                    if (Alldata.Rows.Count > 0)
                    {
                        double Produce = double.Parse(Alldata.Rows[0][0].ToString());
                        double GoodProduct = double.Parse(Alldata.Rows[0][2].ToString());
                        double TimeEfficiency = double.Parse(Alldata.Rows[0][1].ToString());
                        double GoodValue = 0;
                        if (Produce == 0 && GoodProduct == 0)
                        {
                            GoodValue = 0;
                        }
                        else
                        {
                            GoodValue = GoodProduct / Produce;
                        }
                        SummaryList[0].Title = "汇总";
                        SummaryList[0].Efficiency = (Produce / MainWindow.DayPlan).ToString("p0");
                        SummaryList[0].Good = GoodValue.ToString("p0");
                        SummaryList[0].TimeEfficiency = $"{TimeEfficiency}%";
                    }
                });
                //触发间隔
                timerNotice.Interval = MainWindow.Time;
                timerNotice.Start();
            }
        }
        /// <summary>
        /// 设备信息字段
        /// </summary>
        public class TableData : INotifyPropertyChanged
        {
            public string time;
            /// <summary>
            /// 时间
            /// </summary>
            public string Time
            {
                get
                {
                    return time;
                }
                set
                {
                    time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }

            public string efficiency;
            /// <summary>
            /// 效率
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

            public string good;
            /// <summary>
            /// 良率
            /// </summary>
            public string Good
            {
                get
                {
                    return good;
                }
                set
                {
                    good = value;
                    OnPropertyChanged(nameof(Good));
                }
            }

            public string timeefficiency;
            /// <summary>
            /// 时间稼动率
            /// </summary>
            public string TimeEfficiency
            {
                get
                {
                    return timeefficiency;
                }
                set
                {
                    timeefficiency = value;
                    OnPropertyChanged(nameof(TimeEfficiency));
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
        /// 设备信息字段
        /// </summary>
        public class Summary : INotifyPropertyChanged
        {
            public string title;
            /// <summary>
            /// 标题
            /// </summary>
            public string Title
            {
                get
                {
                    return title;
                }
                set
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }

            public string efficiency;
            /// <summary>
            /// 效率
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

            public string good;
            /// <summary>
            /// 良率
            /// </summary>
            public string Good
            {
                get
                {
                    return good;
                }
                set
                {
                    good = value;
                    OnPropertyChanged(nameof(Good));
                }
            }

            public string timeefficiency;
            /// <summary>
            /// 时间稼动率
            /// </summary>
            public string TimeEfficiency
            {
                get
                {
                    return timeefficiency;
                }
                set
                {
                    timeefficiency = value;
                    OnPropertyChanged(nameof(TimeEfficiency));
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
}
