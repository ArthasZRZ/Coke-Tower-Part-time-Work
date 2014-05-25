using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfRibbonApplication1.Views
{
    /// <summary>
    /// RunTimeSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RunTimeSettingWindow : Window
    {
        public RunTimeSettingWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
