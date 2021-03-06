﻿using System;
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
    /// SelectModeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectModeWindow : Window
    {
        public SelectModeWindow()
        {
            InitializeComponent();
        }
        private void CraftM_Click_1(object sender, RoutedEventArgs e)
        {
            CraftsMode win = new CraftsMode();
            win.Show();
            this.Close();
        }

        private void DesignM_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();      
            win.Show();
            this.Close();
        }
    }
}
