using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace EVMESCharts.Popup
{
    /// <summary>
    /// Message.xaml 的交互逻辑
    /// </summary>
    public partial class Message : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Message 弹框
        /// </summary>
        /// <param name="Type">弹框提示类型  0:绿色 成功弹框  1:橙色 警告弹框   2：红色 错误弹框</param>
        /// <param name="message">页面显示的提示文本</param>
        public Message(int Type, string message)
        {
            // 使弹框位于页面正中间
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            // 添加页面显示的文字
            MessageText = message;
            OnPropertyChanged(nameof(MessageText));

            switch (Type)
            {
                case 0:
                    BorderColor = "#4caf50";
                    OnPropertyChanged(nameof(BorderColor));
                    break;
                case 1:
                    BorderColor = "#ffc346";
                    OnPropertyChanged(nameof(BorderColor));
                    break;
                case 2:
                    BorderColor = "#ff4646";
                    OnPropertyChanged(nameof(BorderColor));
                    break;
                default:
                    break;
            }

            FontColor = MainWindow.WindowFontColor;
            BgColor = MainWindow.WindowBgColor;

            DataContext = this;

            // 鼠标左键拖动窗口
            DragWindows();
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
        /// 拖动窗口
        /// </summary>
        private void DragWindows()
        {
            this.Grid.MouseLeftButtonDown += (o, e) =>
            {
                DragMove();
            };
        }

        /// <summary>
        /// 提示文字
        /// </summary>
        public string MessageText
        {
            get;
            set;
        }

        /// <summary>
        /// 显示边框颜色
        /// </summary>
        public string BorderColor
        {
            get;
            set;
        }

        /// <summary>
        /// 窗体弹框动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 创建动画
            var LoadAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.Parse("0:0:1")),
                From = 0.1,
                To = 1,
                EasingFunction = new ElasticEase()
                {
                    // 弹框缓动动画
                    EasingMode = EasingMode.EaseOut,
                    Springiness = 15
                }
            };
            var LoadClock = LoadAnimation.CreateClock();
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, LoadClock);
        }

        /// <summary>
        /// 确定按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 弹框关闭的动画
            var UnLoadAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.Parse("0:0:0.3")),
                From = 1,
                To = 0.01
            };
            var LoadClock = UnLoadAnimation.CreateClock();
            LoadClock.Completed += (a, b) =>
            {
                // 关闭弹框
                DialogResult = true;
            };
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, LoadClock);
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
