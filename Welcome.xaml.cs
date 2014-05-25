using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfRibbonApplication1
{
	/// <summary>
	/// Welcome.xaml 的交互逻辑
	/// </summary>
	public partial class Welcome : Window
	{
		public Welcome()
		{
			this.InitializeComponent();
		}

        private void DesignModeButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
            this.Close();
        }
        private void CraftsModeButtonClick(object sender, RoutedEventArgs e)
        {
            CraftsMode win = new CraftsMode();
            win.Show();
            this.Close();
        }
	}
}