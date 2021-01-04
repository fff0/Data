using EVMESCharts.DataSource;
using System;
using System.Collections.Generic;
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
    /// DataTableView.xaml 的交互逻辑
    /// </summary>
    public partial class DataTableView : UserControl
    {
        public DataTableView()
        {
            InitializeComponent();

            this.DataGrid.DataContext = new WarningList();

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
    }
}
