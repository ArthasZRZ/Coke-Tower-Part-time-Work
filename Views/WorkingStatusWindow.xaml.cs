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
using Visifire.Charts;


namespace WpfRibbonApplication1.Views
{
    /// <summary>
    /// WorkingStatusWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorkingStatusWindow : Window
    {
        public WorkingStatusWindow()
        {
            InitializeComponent();
            AddDataSeries();
        }

        public void AddDataSeries()
        {
            Models.WorkStatusDataSeriesGenerator DataSeriesGenerator = new Models.WorkStatusDataSeriesGenerator();
            
            DataSeriesGenerator.CSVImporter();
            StatusChart.Series.Add(DataSeriesGenerator.GetTowerTopSeries());
            StatusChart.Series.Add(DataSeriesGenerator.GetTowerBottomSeries());
            StatusChart.Series[0].Name = "塔顶";
            StatusChart.Series[1].Name = "塔底";
            StatusChart.AnimationEnabled = false;
        }
    }
}
