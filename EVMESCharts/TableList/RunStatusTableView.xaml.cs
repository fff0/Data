using EVMESCharts.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// RunStatusTableView.xaml 的交互逻辑
    /// </summary>
    public partial class RunStatusTableView : UserControl, INotifyPropertyChanged
    {
        public RunStatusTableView()
        {
            InitializeComponent();

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            var DayTime = DateTime.Today.ToShortDateString();
            AddFaultData(DayTime);

            AxisXMin = 0;
            AxisXMax = 10;

            DataContext = this;
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
        /// 故障列表信息
        /// </summary>
        public ObservableCollection<FaultFormat> FaultList
        {
            get;
            set;
        } = new ObservableCollection<FaultFormat>();

        /// <summary>
        /// X轴最小值
        /// </summary>
        public int AxisXMin
        {
            get;
            set;
        }
        /// <summary>
        /// X轴最大值
        /// </summary>
        public int AxisXMax
        {
            get;
            set;
        }

        /// <summary>
        /// X轴格式化字符串
        /// </summary>
        public string XFormatter
        {
            get;
            set;
        }


        private System.Timers.Timer timerNotice = null;
        /// <summary>
        /// 添加故障排名前十的列表
        /// </summary>
        public void AddFaultData(string Date)
        {
            DataTable dt = new DataTable("Fault");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Number", typeof(int));
            dt.Columns.Add("Duration", typeof(int));
            
            string allsql = $"SELECT * FROM DetailedFault WHERE Day = '{Date}'";
            DataTable Alldata = SQLiteHelp.ExecuteQuery(allsql);

            // 判断是否有数据
            if (Alldata.Rows.Count > 0)
            {
                var DayTime = DateTime.Today.ToShortDateString();
                for (int i = 0; i < MainWindow.FaultMessageList.Length; i++)
                {
                    string sql = $"SELECT ID, sum(Number),sum(Duration) FROM DetailedFault WHERE Day = '{DayTime}' AND ID = {i}";
                    DataTable data = SQLiteHelp.ExecuteQuery(sql);
                    dt.Rows.Add(
                        data.Rows[0][0],
                        data.Rows[0][1],
                        data.Rows[0][2]
                        );
                }

                // 排序表格
                DataView dv = dt.DefaultView;
                dv.Sort = "Number DESC";
                dt = dv.ToTable();

                // 取表格前几位
                DataTable NewTable = DtSelectTop(10, dt);

                // 获取故障总数
                string ratiosql = $"SELECT sum(Number) FROM DetailedFault WHERE Day = '{DayTime}'";
                DataTable ToyalData = SQLiteHelp.ExecuteQuery(ratiosql);

                int TotalValue = 0;
                if (ToyalData.Rows[0][0].ToString() != "")
                {
                    TotalValue = int.Parse(ToyalData.Rows[0][0].ToString());
                }

                // 添加表格体数据
                for (int i = 0; i < NewTable.Rows.Count; i++)
                {
                    // 格式化时间信息
                    double FaultTime = double.Parse(NewTable.Rows[i][2].ToString());
                    string TimeFormate = $"{Math.Floor(FaultTime / 3600)}时{Math.Floor((FaultTime - (Math.Floor(FaultTime / 3600) * 3600)) / 60)}分";

                    // 添加表格信息
                    FaultList.Add(new FaultFormat()
                    {
                        FaultName = MainWindow.FaultMessageList[int.Parse(NewTable.Rows[i][0].ToString())],
                        FaultNumber = NewTable.Rows[i][1].ToString(),
                        Ratio = (double.Parse(NewTable.Rows[i][1].ToString()) / TotalValue).ToString("P2"),
                        Time = TimeFormate,
                    });
                }
            }



            // 循环刷新
            timerNotice = new System.Timers.Timer();
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, eea) =>
            {
                ChangeTableData();
            });
            timerNotice.Interval = MainWindow.Time * 10;
            timerNotice.Start();
        }
        /// <summary>
        /// 取表格前几位
        /// </summary>
        /// <param name="TopItem">取几位</param>
        /// <param name="dt">表格</param>
        /// <returns></returns>
        public DataTable DtSelectTop(int TopItem, DataTable dt)
        {
            if (dt.Rows.Count < TopItem) return dt;

            DataTable NewTable = dt.Clone();
            DataRow[] rows = dt.Select("1=1");
            for (int i = 0; i < TopItem; i++)
            {
                NewTable.ImportRow((DataRow)rows[i]);
            }
            return NewTable;
        }

        /// <summary>
        /// 改变表格函数
        /// </summary>
        public void ChangeTableData()
        {
            var DayTime = DateTime.Today.ToShortDateString();

            // 添加表格列，排序用
            DataTable dt = new DataTable("Fault");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Number", typeof(int));
            dt.Columns.Add("Duration", typeof(int));

            string allsql = $"SELECT * FROM DetailedFault WHERE Day = '{DayTime}'";
            DataTable Alldata = SQLiteHelp.ExecuteQuery(allsql);

            // 判断是否有数据
            if (Alldata.Rows.Count > 0)
            {
                for (int i = 0; i < MainWindow.FaultMessageList.Length; i++)
                {
                    string sql = $"SELECT ID, sum(Number),sum(Duration) FROM DetailedFault WHERE Day = '{DayTime}' AND ID = {i}";
                    DataTable data = SQLiteHelp.ExecuteQuery(sql);
                    dt.Rows.Add(
                        data.Rows[0][0],
                        data.Rows[0][1],
                        data.Rows[0][2]
                        );
                }

                // 排序表格
                DataView dv = dt.DefaultView;
                dv.Sort = "Number DESC";
                dt = dv.ToTable();

                // 取表格前几位
                DataTable NewTable = DtSelectTop(10, dt);

                // 获取故障总数
                string ratiosql = $"SELECT sum(Number) FROM DetailedFault WHERE Day = '{DayTime}'";
                DataTable ToyalData = SQLiteHelp.ExecuteQuery(ratiosql);

                int TotalValue = 0;
                if (ToyalData.Rows[0][0].ToString() != "")
                {
                    TotalValue = int.Parse(ToyalData.Rows[0][0].ToString());
                }

                // 添加表格体数据
                for (int i = 0; i < NewTable.Rows.Count; i++)
                {
                    // 格式化时间信息
                    double FaultTime = double.Parse(NewTable.Rows[i][2].ToString());
                    string TimeFormate = $"{Math.Floor(FaultTime / 3600)}时{Math.Floor((FaultTime - (Math.Floor(FaultTime / 3600) * 3600)) / 60)}分";

                    // 修改表格信息
                    FaultList[i].FaultName = MainWindow.FaultMessageList[int.Parse(NewTable.Rows[i][0].ToString())];
                    FaultList[i].FaultNumber = NewTable.Rows[i][1].ToString();
                    FaultList[i].Ratio = (double.Parse(NewTable.Rows[i][1].ToString()) / TotalValue).ToString("P2");
                    FaultList[i].Time = TimeFormate;
                }
                OnPropertyChanged(nameof(FaultList));
            }

        }
        /// <summary>
        /// 表格标题
        /// </summary>
        public string TableTitle
        {
            get;
            set;
        }

        // <summary>
        /// 良品数据表格数据格式
        /// </summary>
        public class FaultFormat : INotifyPropertyChanged
        {
            public string faultname;
            /// <summary>
            /// 故障名字
            /// </summary>
            public string FaultName
            {
                get
                {
                    return faultname;
                }
                set
                {
                    faultname = value;
                    OnPropertyChanged(nameof(FaultName));
                }
            }

            public string faultnumber;
            /// <summary>
            /// 故障次数
            /// </summary>
            public string FaultNumber
            {
                get
                {
                    return faultnumber;
                }
                set
                {
                    faultnumber = value;
                    OnPropertyChanged(nameof(FaultNumber));
                }
            }

            public string ratio;
            /// <summary>
            /// 所占比例
            /// </summary>
            public string Ratio
            {
                get
                {
                    return ratio;
                }
                set
                {
                    ratio = value;
                    OnPropertyChanged(nameof(Ratio));
                }
            }

            public string time;
            /// <summary>
            /// 故障时间
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
