using EVMESCharts.Sqlite;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EVMESCharts.Popup
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class PlanPopup : Window
    {
        public PlanPopup()
        {
            // 使弹框位于页面正中间
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            // 查询数据库，添加初始值
            GetText();

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
        /// 获取数据库值 添加初始值
        /// </summary>
        private void GetText()
        {
            string sql = "SELECT * FROM StandardData";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);

            // 添加页面显示数据
            if (data.Rows.Count > 0)
            {
                CapacityText = int.Parse(data.Rows[data.Rows.Count - 1][1].ToString());
                DayPlanText = int.Parse(data.Rows[data.Rows.Count - 1][2].ToString());
                MonthPlanText = int.Parse(data.Rows[data.Rows.Count - 1][3].ToString());
                Capacity.Text = $"{CapacityText}";
                DayPlan.Text = $"{DayPlanText}";
                MonthPlan.Text = $"{MonthPlanText}";
            }
        }
        /// <summary>
        /// 日标准产能
        /// </summary>
        public int CapacityText
        {
            get;
            set;
        }
        /// <summary>
        /// 日计划开机时长
        /// </summary>
        public int DayPlanText
        {
            get;
            set;
        }
        /// <summary>
        /// 月计划开工天数
        /// </summary>
        public int MonthPlanText
        {
            get;
            set;
        }

        /// <summary>
        /// 点击确定按纽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKClick(object sender, RoutedEventArgs e)
        {
            // 获取输入的数据
            int capacity = int.Parse(Capacity.Text== ""? "0": Capacity.Text);
            int dayplan = int.Parse(DayPlan.Text == "" ? "0" : DayPlan.Text);
            int monthplan = int.Parse(MonthPlan.Text == "" ? "0" : MonthPlan.Text);
            var Daytime = DateTime.Today.Date.ToShortDateString();
            if (capacity == CapacityText && dayplan == DayPlanText && monthplan == MonthPlanText)
            {
                // 关闭弹框
                DialogResult = true;
                Console.WriteLine("111");
            }
            else if (capacity != 0 && dayplan != 0 && monthplan != 0)
            {
                // 清空数据库当天设置的内容
                string delsql = $"DELETE FROM StandardData WHERE Day = '{Daytime}'";
                SQLiteHelp.DeleteSql(delsql);

                // 插入新数据
                string sql = $"INSERT INTO StandardData VALUES('{Daytime}',{capacity},{dayplan},{monthplan})";
                bool IsInsert = SQLiteHelp.SQLInsert(sql);
                if (IsInsert)
                {
                    //关闭弹框
                    DialogResult = true;

                    // 保存成功提示
                    Message mes = new Message(0, "修改成功");
                    mes.ShowDialog();
                }
                else
                {
                    // 保存失败提示
                    Message mes = new Message(2, "修改失败");
                    mes.ShowDialog();
                }
            }
            else
            {
                Message mes = new Message(1,"数据不能为0");
                mes.ShowDialog();
            }
        }
    }
}
