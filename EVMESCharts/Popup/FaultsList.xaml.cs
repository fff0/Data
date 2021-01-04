using EVMESCharts.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace EVMESCharts.Popup
{
    /// <summary>
    /// FaultsList.xaml 的交互逻辑
    /// </summary>
    public partial class FaultsList : Window
    {
        public FaultsList()
        {
            // 使弹框位于页面正中间
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();

            var DayTime = DateTime.Today.ToShortDateString();
            TableTitle = $"{DayTime} 详细故障信息";

            AddFaultData(DayTime);

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

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
        /// 添加故障排名前十的列表
        /// </summary>
        public void AddFaultData(string Date)
        {
            DataTable dt = new DataTable("Fault");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Number", typeof(int));

            var DayTime = DateTime.Today.ToShortDateString();

            string Allsql = $"SELECT ID, Number FROM DetailedFault WHERE Day = '{DayTime}'";
            DataTable AllTable = SQLiteHelp.ExecuteQuery(Allsql);
            if (AllTable.Rows.Count > 0)
            {
                for (int i = 0; i < MainWindow.FaultMessageList.Length; i++)
                {
                    string sql = $"SELECT ID, sum(Number) FROM DetailedFault WHERE Day = '{DayTime}' AND ID = {i}";
                    DataTable data = SQLiteHelp.ExecuteQuery(sql);
                    dt.Rows.Add(
                        data.Rows[0][0],
                        data.Rows[0][1]
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
                    FaultList.Add(new FaultFormat()
                    {
                        FaultName = MainWindow.FaultMessageList[int.Parse(NewTable.Rows[i][0].ToString())],
                        FaultNumber = NewTable.Rows[i][1].ToString(),
                        Ratio = (double.Parse(NewTable.Rows[i][1].ToString()) / TotalValue).ToString("P2")
                    });
                }
            }
        }

        /// <summary>
        /// 点击确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKClick(object sender, RoutedEventArgs e)
        {
            // 关闭弹框
            DialogResult = true;

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
        public class FaultFormat
        {
            /// <summary>
            /// 故障名字
            /// </summary>
            public string FaultName
            {
                get;
                set;
            }

            /// <summary>
            /// 故障次数
            /// </summary>
            public string FaultNumber
            {
                get;
                set;
            }

            /// <summary>
            /// 所占比例
            /// </summary>
            public string Ratio
            {
                get;
                set;
            }
        }
    }
}
