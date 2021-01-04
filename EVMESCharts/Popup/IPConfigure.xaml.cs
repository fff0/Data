using EVMESCharts.ModBus;
using EVMESCharts.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace EVMESCharts.Popup
{
    /// <summary>
    /// IPConfigure.xaml 的交互逻辑
    /// </summary>
    public partial class IPConfigure : Window
    {
        public IPConfigure()
        {
            // 使弹框位于页面正中间
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            //获取IP
            GetIP();

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
        /// 获取数据库中的IP地址
        /// </summary>
        private void GetIP()
        {
            string sql = "SELECT * FROM IPData";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);
            if (data.Rows.Count > 0)
            {
                int ip1 = int.Parse(data.Rows[data.Rows.Count - 1][0].ToString());
                int ip2 = int.Parse(data.Rows[data.Rows.Count - 1][1].ToString());
                int ip3 = int.Parse(data.Rows[data.Rows.Count - 1][2].ToString());
                int ip4 = int.Parse(data.Rows[data.Rows.Count - 1][3].ToString());
                int port = int.Parse(data.Rows[data.Rows.Count - 1][4].ToString());
                IP1.Text = $"{ip1}";
                IP2.Text = $"{ip2}";
                IP3.Text = $"{ip3}";
                IP4.Text = $"{ip4}";
                Port.Text = $"{port}";
                IP = $"{ip1}.{ip2}.{ip3}.{ip4}:{port}";
            }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            get;
            set;
        }
        /// <summary>
        /// OK按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKClick(object sender, RoutedEventArgs e)
        {
            if (IP1.Text == "" || IP2.Text == "" || IP3.Text == "" || IP4.Text == "" || Port.Text == "")
            {
                Message mes = new Message(1,"IP地址不能为空");
                mes.ShowDialog();
                //MessageBox.Show("IP地址不能为空");
            }
            else
            {
                int ip1 = int.Parse(IP1.Text);
                int ip2 = int.Parse(IP2.Text);
                int ip3 = int.Parse(IP3.Text);
                int ip4 = int.Parse(IP4.Text);
                int port = int.Parse(Port.Text);
                string IPText = $"{ip1}.{ip2}.{ip3}.{ip4}:{port}";
                if (ip1 > 255 || ip2 > 255 || ip3 > 255 || ip4 > 255 || ip1 < 0 || ip2 < 0 || ip3 < 0 || ip4 < 0)
                {
                    Message mes = new Message(1,"IP地址错误");
                    mes.ShowDialog();
                }
                else if (IPText == IP)
                {
                    // 关闭弹框
                    DialogResult = true;

                    // 运行读取Modbus数据函数
                    //ReadModBusIP();
                    RunGetModBus();
                }
                else
                {
                    // 清空数据库内容
                    string delsql = "DELETE FROM IPData";
                    SQLiteHelp.DeleteSql(delsql);

                    // 插入新数据
                    string sql = $"INSERT INTO IPData VALUES('{ip1}',{ip2},{ip3},{ip4},{port})";

                    bool IsInsert = SQLiteHelp.SQLInsert(sql);

                    if (IsInsert)
                    {
                        // 关闭弹框
                        DialogResult = true;
                        // 保存成功提示
                        Message mes = new Message(0, "IP地址修改成功");
                        mes.ShowDialog();

                        // 运行读取Modbus IP数据函数
                        //ReadModBusIP();
                        RunGetModBus();
                    }
                    else
                    {
                        // 保存失败提示
                        Message mes = new Message(2, "IP地址修改失败");
                        mes.ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// 读取ModBus数据
        /// </summary>
        /// <returns></returns>
        public Task RunGetModBus()
        {
            bool IsRunning = true;

            return Task.Factory.StartNew(() => 
            {
                // 设置线程名称
                Thread.CurrentThread.Name = "存储数据";
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Priority = ThreadPriority.Normal;

                while (IsRunning)
                {
                    Thread.Sleep(2000);
                    try
                    {
                        ModBusHelp.ReadModBus();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("读取MOdBus数据失败，请检查IP地址是否正确");
                        IsRunning = false;
                    }
                }
            });
        }
    }
}
