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

namespace WpfRibbonApplication1
{
    /// <summary>
    /// OpenProjWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OpenProjWindow : Window
    {
        private static OpenProjWindow staticInstance = null;
        public OpenProjWindow()
        {
            this.InitializeComponent();
            this.Closed += WindowOnClosed;
        }

        public static OpenProjWindow GetInstance()
        {
            if (staticInstance == null)
            {
                staticInstance = new OpenProjWindow();
            }

            return staticInstance;
        }

        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowOnClosed(object sender, System.EventArgs e)
        {
            staticInstance = null;
        }
    }
}
