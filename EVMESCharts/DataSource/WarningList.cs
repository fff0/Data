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
using System.Timers;

namespace EVMESCharts.DataSource
{
    public class WarningList: INotifyPropertyChanged
    {
        /// <summary>
        /// 设备告警信息数据
        /// </summary>
        public WarningList()
        {
            //添加设备告警信息
            Init();
        }
        /// <summary>
        /// 设备告警信息
        /// </summary>
        public ObservableCollection<UserModel> UserWarningList
        {
            get;
            set;
        }

        /// <summary>
        /// 告警信息长度
        /// </summary>
        public int WarningLength
        {
            get;
            set;
        }
        /// <summary>
        /// System.Timers.Timer 设备告警信息计时
        /// </summary>
        private System.Timers.Timer timerNotice;
        
        /// <summary>
        /// 添加设备告警信息
        /// </summary>
        private void Init()
        {
            UserWarningList = new ObservableCollection<UserModel>();
            
            //添加设备告警信息
            
            var Daytime = DateTime.Today.Date.ToShortDateString();
            //创建查询数据库语句
            string sql = $"SELECT * FROM WarningData WHERE Day = '{Daytime}'";
            //执行查询操作
            DataTable dt = SQLiteHelp.ExecuteQuery(sql);
           
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string time = dr["Time"].ToString();
                    //添加设备日志信息
                    UserWarningList.Add(new UserModel()
                    {
                        Time = $"{time}:00",
                        Device = MainWindow.DeviceList[int.Parse(dr["DevID"].ToString())],
                        Content = $"{MainWindow.WarningTypeList[int.Parse(dr["Content"].ToString())]},完成{double.Parse(dr["ReachRate"].ToString()).ToString("p0")}"
                    });
                }
            }
            WarningLength = dt.Rows.Count;  // 记录初始告警列表长度

            timerNotice = new System.Timers.Timer();
            //间隔触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                //创建查询数据库语句
                string runsql = $"SELECT * FROM WarningData WHERE Day = '{Daytime}'";
                //执行查询操作
                DataTable rundt = SQLiteHelp.ExecuteQuery(sql);
                if (rundt.Rows.Count == 0)
                {
                    WarningLength = 0;
                }
                else if (rundt.Rows.Count > WarningLength)
                {
                    for (int i = 0; i < rundt.Rows.Count - WarningLength; i++)
                    {
                        // 获取数据库中新增加的数据信息
                        var runtime = rundt.Rows[rundt.Rows.Count - i - 1][1].ToString();             //时间
                        var rundevname = rundt.Rows[rundt.Rows.Count - i - 1][2].ToString();          //设备名
                        var runcontent = rundt.Rows[rundt.Rows.Count - i - 1][3].ToString();          //告警内容
                        var runreachrate = rundt.Rows[rundt.Rows.Count - i - 1][4].ToString();        //完成度
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            System.Threading.SynchronizationContext.SetSynchronizationContext(new
                                System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                            System.Threading.SynchronizationContext.Current.Post(p1 =>
                            {
                                //添加设备日志信息
                                UserWarningList.Add(new UserModel()
                                {
                                    Time = $"{runtime}:00",
                                    Device = MainWindow.DeviceList[int.Parse(rundevname)],
                                    //Content = MainWindow.WarningTypeList[int.Parse(runcontent)]
                                    Content = $"{MainWindow.WarningTypeList[int.Parse(runcontent)]},完成{double.Parse(runreachrate).ToString("p0")}"
                                });
                            }, null);
                        });
                    }
                    WarningLength = rundt.Rows.Count;
                }
            });
            //触发间隔
            timerNotice.Interval = MainWindow.Time;
            timerNotice.Start();
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
    /// 设备告警信息字段
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 时间信息
        /// </summary>
        public string Time
        {
            get;
            set;
        }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Device
        {
            get;
            set;
        }
        /// <summary>
        /// 显示内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }
    }
}
