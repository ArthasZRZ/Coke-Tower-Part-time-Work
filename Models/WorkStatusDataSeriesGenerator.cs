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
using System.Data;
using Com.StellmanGreene.CSVReader;

namespace WpfRibbonApplication1.Models
{
    public class WorkStatusDataSeriesGenerator
    {
        string csv_file_name = System.AppDomain.CurrentDomain.BaseDirectory + @"\CalDatas\DataSeries.csv";

        List<double> TowerTopTemperature = new List<double>();
        List<double> TowerBottomTemperature = new List<double>();
        List<int> TimeList = new List<int>();

        public WorkStatusDataSeriesGenerator() { }

        public void CSVImporter(){
            DataTable dbtable = CSVReader.ReadCSVFile(csv_file_name, true);
            DataRow[] dbrow = dbtable.Select();
            foreach (DataColumn dbcol in dbtable.Columns)
            {
                if (dbrow[1][dbcol].ToString() != "" && dbrow[2][dbcol].ToString() != "")
                {
                    TowerTopTemperature.Add(System.Convert.ToDouble(dbrow[0][dbcol]));
                    TowerBottomTemperature.Add(System.Convert.ToDouble(dbrow[1][dbcol]));
                    //MessageBox.Show(dbrow[2][dbcol].ToString());

                    TimeList.Add(System.Convert.ToInt32(dbrow[2][dbcol]));
                }
            }
        }

        public DataSeries GetTowerTopSeries()
        {
            DataSeries dataseries = new DataSeries();
            dataseries.RenderAs = RenderAs.Spline;
            dataseries.Legend = "塔顶";
            DataPoint point;

            for (int i = 0; i < TowerTopTemperature.Count(); i++)
            {
                point = new DataPoint();
                point.YValue = TowerTopTemperature[i];
                point.XValue = TimeList[i];
                point.MarkerEnabled = false;
                dataseries.DataPoints.Add(point);
            }
            
            return dataseries;
        }
        public DataSeries GetTowerBottomSeries()
        {
            DataSeries dataseries = new DataSeries();
            dataseries.RenderAs = RenderAs.Spline;
            dataseries.Legend = "塔顶";
            DataPoint point;

            for (int i = 0; i < TowerTopTemperature.Count(); i++)
            {
                point = new DataPoint();
                point.YValue = TowerBottomTemperature[i];
                point.XValue = TimeList[i];
                point.MarkerEnabled = false;
                dataseries.DataPoints.Add(point);
            }

            return dataseries;
        }

        public DataSeries GetSmallBigBlowSteamDataSeries()
        {
            DataSeries dataSeries = new DataSeries();
            dataSeries.RenderAs = RenderAs.Spline;
            dataSeries.LegendText = "小大吹汽";

            int y = 0;
            

            return dataSeries;
        }
        public DataSeries GetEnterOilReceiveCokeDataSeries()
        {
            DataSeries dataseries = new DataSeries();
            dataseries.RenderAs = RenderAs.Spline;
            dataseries.Legend = "进油结焦";
            DataPoint point;
            int y = 0;
            for (int time = 0; time < 50; time++)
            {
                point = new DataPoint();
                point.YValue = (-2.36 * (time / 3600) * (time / 3600) - 49 * time / 9000 + 295.8) * (y + 6.529) / 35.976 + 2.36 * (time / 3600) * (time / 3600) + 49 * time / 9000 + 107.2;
                point.XValue = time;
                dataseries.DataPoints.Add(point);
            }
            return dataseries;
        }
    }
}
