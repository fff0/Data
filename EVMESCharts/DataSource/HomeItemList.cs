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
    /// <summary>
    /// 设备信息数据
    /// </summary>
    class HomeItemList: INotifyPropertyChanged
    {
        public HomeItemList()
        {
            InitProductionList();
        }
        /// <summary>
        /// 设备信息
        /// </summary>
        public ObservableCollection<PList> EquipmentList
        {
            get;
            set;
        }

        /// <summary>
        /// 记录各个设备的总产量信息
        /// </summary>
        public List<int> DevProduceList
        {
            get;
            set;
        } = new List<int>();

        /// <summary>
        /// System.Timers.Timer 设备告警信息计时
        /// </summary>
        private System.Timers.Timer timerNotice;
        /// <summary>
        /// 添加设备信息数据
        /// </summary>
        public void InitProductionList()
        {
            // 获取时间
            var DayTime = DateTime.Today.Date.ToShortDateString();
            var Hour = DateTime.Now.Hour.ToString();

            // 初始化设备信息
            EquipmentList = new ObservableCollection<PList>();

            // 获取当前设备编号列表
            string devlidtsql = "SELECT ID FROM Equipment";
            DataTable devdata = SQLiteHelp.ExecuteQuery(devlidtsql);
            var DevNumberList = SQLiteHelp.NumberList(devdata, "ID");

            // 生成所有数据
            for (int i = 0; i < DevNumberList.Count; i++)
            {
                //初始化数据
                DevProduceList.Add(0);
                //创建查询数据库语句
                string Sql = 
                    "SELECT DevID,sum(Produce),sum(Electric),sum(Gas),sum(GoodProduct) " +
                    "FROM DayDeviceData " +
                    $" WHERE DevID = {i} AND Day = '{DayTime}'";
                DataTable data = SQLiteHelp.ExecuteQuery(Sql);
                string timesql = $"SELECT TimeEfficiency FROM DayDeviceData WHERE  DevID = {i} AND Day = '{DayTime}' AND Time = {Hour}";
                DataTable timedata = SQLiteHelp.ExecuteQuery(timesql);

                string timeefficiency = "0";
                // 判断当前时间是否有稼动率数据
                if (timedata.Rows.Count > 0)
                {
                    timeefficiency = timedata.Rows[0][0].ToString();
                }

                // 判断是否有数据
                if (data.Rows[0][1].ToString() != "")
                {
                    // 添加初始化页面显示设备详细数据 （当天数据）
                    EquipmentList.Add(new PList()
                    {
                        EquTitle = MainWindow.DeviceList[i],
                        ProductionSum = data.Rows[0][1].ToString(),
                        Reach = (double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan).ToString("p2"),
                        Efficiency = (double.Parse(data.Rows[0][1].ToString()) / MainWindow.DayPlan).ToString("p2"),
                        TimeEfficiency = $"{timeefficiency}%",
                        Electricity = data.Rows[0][2].ToString(),
                        Gas = data.Rows[0][3].ToString(),
                        //GoodProduce = (double.Parse(data.Rows[0][4].ToString()) / double.Parse(data.Rows[0][1].ToString())).ToString("p2"),
                        //GoodSum = data.Rows[0][4].ToString()
                        GoodProduce = (1).ToString("p2"),
                        GoodSum = data.Rows[0][1].ToString()
                    });
                    DevProduceList[i] = int.Parse(data.Rows[0][1].ToString());
                }
                else
                {
                    // 添加初始化页面显示设备详细数据 （当天数据）
                    EquipmentList.Add(new PList()
                    {
                        EquTitle = MainWindow.DeviceList[i],
                        ProductionSum = 0.ToString(),
                        Reach = (0).ToString("p2"),
                        Efficiency = (0).ToString("p2"),
                        TimeEfficiency = (0).ToString("p0"),
                        Electricity = 0.ToString(),
                        Gas = 0.ToString(),
                        GoodProduce = (1).ToString("p2"),
                        GoodSum = 0.ToString()
                    });
                    DevProduceList[i] = 0;
                }
            }

            timerNotice = new System.Timers.Timer();
            //间隔触发函数
            timerNotice.Elapsed += new System.Timers.ElapsedEventHandler((o, e) =>
            {
                string sql = "SELECT DevID FROM DayDeviceData";
                DataTable data = SQLiteHelp.ExecuteQuery(sql);

                //获取当前设备编号列表
                var NowNumberList = SQLiteHelp.NumberList(data, "DevID");
                var NowDayTime = DateTime.Today.Date.ToShortDateString();
                var NowHour = DateTime.Now.Hour.ToString();
                for (int i = 0; i < NowNumberList.Count; i++)
                {
                    //创建查询数据库语句
                    string runsql = "SELECT DevID,sum(Produce),sum(Electric),sum(Gas),sum(GoodProduct) " +
                                   "FROM DayDeviceData " +
                                   $" WHERE DevID = {i} AND Day = '{NowDayTime}'";
                    DataTable rundata = SQLiteHelp.ExecuteQuery(runsql);
                    string timesql = $"SELECT TimeEfficiency FROM DayDeviceData WHERE  DevID = {i} AND Day = '{NowDayTime}' AND Time = {NowHour}";
                    DataTable timedata = SQLiteHelp.ExecuteQuery(timesql);

                    string timeefficiency = "0";
                    // 判断当前时间是否有稼动率数据
                    if (timedata.Rows.Count > 0)
                    {
                        timeefficiency = timedata.Rows[0][0].ToString();
                    }
                    
                    // 判断当前时间是否有数据，和上一次读取的数据是否相同
                    if (rundata.Rows[0][1].ToString() != "" && int.Parse(rundata.Rows[0][1].ToString()) != DevProduceList[i])
                    {
                        EquipmentList[i].ProductionSum = rundata.Rows[0][1].ToString();
                        EquipmentList[i].Reach = (double.Parse(rundata.Rows[0][1].ToString()) / MainWindow.DayPlan).ToString("p2");
                        EquipmentList[i].Efficiency = (double.Parse(rundata.Rows[0][1].ToString()) / MainWindow.DayPlan).ToString("p2");
                        EquipmentList[i].TimeEfficiency = $"{timeefficiency}%";
                        EquipmentList[i].Electricity = rundata.Rows[0][2].ToString();
                        EquipmentList[i].Gas = rundata.Rows[0][3].ToString();
                        //EquipmentList[i].GoodProduce = (double.Parse(rundata.Rows[0][4].ToString()) / double.Parse(rundata.Rows[0][1].ToString())).ToString("p2");
                        //EquipmentList[i].GoodSum = rundata.Rows[0][4].ToString();
                        EquipmentList[i].GoodProduce = (1).ToString("p2");
                        EquipmentList[i].GoodSum = rundata.Rows[0][1].ToString();
                    }
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
    /// 设备信息格式
    /// </summary>
    public class PList : INotifyPropertyChanged
    {
        /// <summary>
        /// 设备名
        /// </summary>
        public string EquTitle
        {
            get;
            set;
        }

        public string sum;
        /// <summary>
        /// 生产总数
        /// </summary>
        public string ProductionSum
        {
            get
            {
                return sum;
            }
            set
            {
                sum = value;
                OnPropertyChanged(nameof(ProductionSum));
            }
        }

        public string reach;
        /// <summary>
        /// 达成率
        /// </summary>
        public string Reach
        {
            get
            {
                return reach;
            }
            set
            {
                reach = value;
                OnPropertyChanged(nameof(Reach));
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

        public string time;
        /// <summary>
        /// 时间稼动率
        /// </summary>
        public string TimeEfficiency
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                OnPropertyChanged(nameof(TimeEfficiency));
            }
        }

        public string electricity;
        /// <summary>
        /// 电消耗
        /// </summary>
        public string Electricity
        {
            get
            {
                return electricity;
            }
            set
            {
                electricity = value;
                OnPropertyChanged(nameof(Electricity));
            }
        }

        public string gas;
        /// <summary>
        /// 气消耗
        /// </summary>
        public string Gas
        {
            get
            {
                return gas;
            }
            set
            {
                gas = value;
                OnPropertyChanged(nameof(Gas));
            }
        }

        public string good;
        /// <summary>
        /// 良品率
        /// </summary>
        public string GoodProduce
        {
            get
            {
                return good;
            }
            set
            {
                good = value;
                OnPropertyChanged(nameof(GoodProduce));
            }
        }

        public string goodsum;
        /// <summary>
        /// 良品数
        /// </summary>
        public string GoodSum
        {
            get
            {
                return goodsum;
            }
            set
            {
                goodsum = value;
                OnPropertyChanged(nameof(GoodSum));
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
