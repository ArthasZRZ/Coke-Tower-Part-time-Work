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
using System.Data;
using Visifire.Charts;
using Com.StellmanGreene.CSVReader;

namespace WpfRibbonApplication1
{
    /// <summary>
    /// WorkingDataImporterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorkingDataImporterWindow : Window
    {
        
        public WorkingDataImporterWindow()
        {
            InitializeComponent();
            //ShowLine();
        }

        public void DataReader( ref DataSeries dataSeries)
        {
            System.Windows.Forms.OpenFileDialog opfile = new System.Windows.Forms.OpenFileDialog();
            opfile.Filter = "CSV Files (*.csv)|*.csv";
            opfile.RestoreDirectory = true;

            DataPoint point;

            if (opfile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = opfile.FileName;
                DataTable dbtable = CSVReader.ReadCSVFile(filename, true);
                DataRow[] dbrow = dbtable.Select();
                foreach (DataRow dr in dbrow)
                {
                    point = new DataPoint();
                    point.YValue = System.Convert.ToDouble(dr[1]);
                    point.XValue = System.Convert.ToDouble(dr[0]);
                    dataSeries.DataPoints.Add(point);
                }

                //ShowImportedData.ItemsSource = dbtable.AsDataView();
            }
        }

        public void ShowLine(DataSeries dataSeries)
        {
            Title title = new Title();
            title.Text = "This is chart 1";
            dataSeries.RenderAs = RenderAs.Spline;
            dataSeries.LegendText = "X坐标";

            chart_1.Series.Add(dataSeries);

        }

        private void BtnImporter_Click_1(object sender, RoutedEventArgs e)
        {
            DataSeries dataSeries = new DataSeries();
            DataReader(ref dataSeries);
            ShowLine(dataSeries);
        }
    }
}
