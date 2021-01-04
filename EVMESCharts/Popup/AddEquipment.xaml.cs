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
    /// AddEquipment.xaml 的交互逻辑
    /// </summary>
    public partial class AddEquipment : Window
    {
        public AddEquipment()
        {
            // 使弹框位于页面正中间
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            // 添加初始化信息
            AddSource();

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
        /// 设备列表信息
        /// </summary>
        public ObservableCollection<DeviceName> DevNameList
        {
            get;
            set;
        } = new ObservableCollection<DeviceName>();

        /// <summary>
        /// OK按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKClick(object sender, RoutedEventArgs e)
        {
            bool IsInsert = true;
            for (int i = 0; i < DevNameList.Count; i++)
            {
                if (DevNameList[i].ID == "" || DevNameList[i].Name == "")
                {
                    // 提示框信息
                    Message message = new Message(1,"ID和设备名称不能为空");
                    message.ShowDialog();
                    return;
                }
            }
            // 清空数据库内容
            string delsql = "DELETE FROM Equipment";
            SQLiteHelp.DeleteSql(delsql);

            // 添加设备信息
            for (int i = 0; i < DevNameList.Count; i++)
            {
                string sql = $"INSERT INTO Equipment VALUES ('{i}','{DevNameList[i].Name}')";
                IsInsert = SQLiteHelp.SQLInsert(sql);
            }
            // 关闭弹框
            DialogResult = true;

            if (IsInsert)
            {
                // 保存成功提示
                Message mes = new Message(0, "添加修改设备成功");
                mes.ShowDialog();
            }
            else
            {
                // 保存失败提示
                Message mes = new Message(2, "添加失败");
                mes.ShowDialog();
            }

        }

        /// <summary>
        /// 添加设备初始信息
        /// </summary>
        private void AddSource()
        {
            string sql = "SELECT * FROM Equipment";
            DataTable data = SQLiteHelp.ExecuteQuery(sql);

            if (data.Rows.Count > 0)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DevNameList.Add(new DeviceName()
                    {
                        ID = data.Rows[i][0].ToString(),
                        Name = data.Rows[i][1].ToString()
                    });
                }
            }
        }
        
        /// <summary>
        /// 新增一行数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRow(object sender, RoutedEventArgs e)
        {
            DevNameList.Add(new DeviceName()
            {
                ID= $"{DevNameList.Count}",
                Name = ""
            });
        }

        ///// <summary>
        ///// 获取鼠标选中的行
        ///// </summary>
        ///// <param name="dg">表格体</param>
        ///// <param name="rowIndex"></param>
        ///// <param name="columnIndex"></param>
        ///// <returns></returns>
        //private bool GetCellXY(DataGrid dg, ref int rowIndex, ref int columnIndex)
        //{
        //    var cells = dg.SelectedCells;
        //    if (cells.Any())
        //    {
        //        rowIndex = dg.Items.IndexOf(cells.First().Item);
        //        columnIndex = cells.First().Column.DisplayIndex;

        //        string devlidtsql = "SELECT DevID FROM DayDeviceData";
        //        DataTable devdata = SQLiteHelp.ExecuteQuery(devlidtsql);

        //        //获取当前设备编号列表
        //        var DevNumberList = SQLiteHelp.NumberList(devdata, "DevID");

        //        foreach (var id in DevNumberList)
        //        {
        //            if (int.Parse(DevNameList[rowIndex].ID) == id)
        //            {
        //                MessageBox.Show("无法删除设备，该设备已有运行数据，只支持修改设备名称");
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 获取选中的值
        ///// </summary>
        ///// <param name="dg"></param>
        ///// <returns></returns>
        //private string GetSelectedCellsValue(DataGrid dg)
        //{
        //    var cells = dg.SelectedCells;
        //    StringBuilder sb = new StringBuilder();
        //    if (cells.Any())
        //    {
        //        foreach (var cell in cells)
        //        {
        //            sb.Append((cell.Column.GetCellContent(cell.Item) as TextBlock).Text);
        //            sb.Append(" ");
        //        }
        //    }
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// 删除选中的行
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void DeleteRow(object sender, RoutedEventArgs e)
        //{
        //    int rowIndex = 0;
        //    int columnIndex = 0;
        //    if (GetCellXY(DataGrid, ref rowIndex, ref columnIndex))
        //    {
        //        DevNameList.RemoveAt(rowIndex);
        //    }
        //}

        /// <summary>
        /// 删除末行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRow(object sender, RoutedEventArgs e)
        {
            if (DevNameList.Count > 0)
            {
                string devlidtsql = "SELECT DevID FROM DayDeviceData";
                DataTable devdata = SQLiteHelp.ExecuteQuery(devlidtsql);

                //获取当前设备编号列表
                var DevNumberList = SQLiteHelp.NumberList(devdata, "DevID");
                
                foreach (var id in DevNumberList)
                {
                    // 如果数据库中存在要删除设备的数据，则无法删除
                    if (int.Parse(DevNameList[DevNameList.Count - 1].ID) == id)
                    {
                        Message mes = new Message(2,"无法删除设备，该设备已有运行数据，只支持修改设备名称");
                        mes.ShowDialog();
                        return;
                    }
                }
                DevNameList.RemoveAt(DevNameList.Count - 1);
            }
        }

        /// <summary>
        /// 设备信息字段
        /// </summary>
        public class DeviceName
        {
            /// <summary>
            /// 设备编号
            /// </summary>
            public string ID
            {
                get;
                set;
            }
            /// <summary>
            /// 设备名称
            /// </summary>
            public string Name
            {
                get;
                set;
            }
        }
    }
}
