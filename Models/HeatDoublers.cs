using System;
using System.Collections.Generic;
using System.Linq;
using Com.StellmanGreene.CSVReader;
using System.Text;
using NDatabase;
using System.Data;

namespace WpfRibbonApplication1.Models
{
    public class HeatDoubler
    {
        public string Name { set; get; }
        public double X, Y, Z;
        public string Xstr { set; get; }
        public string Ystr { set; get; }
        public string Zstr { set; get; }
        
        public HeatDoubler(){}
        public HeatDoubler(string Name, double X, double Y, double Z)
        {
            this.Name = Name;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Xstr = X.ToString();
            this.Ystr = Y.ToString();
            this.Zstr = Z.ToString();
        }
    }
    public class HeatDoublers
    {
        public List<HeatDoubler> list = null;
        public int listSize = 0;

        public HeatDoublers() 
        {
            list = new List<HeatDoubler>();
            listSize = 0;
        }
        public void HeatDoublerAppend(HeatDoubler hd)
        {
            list.Add(hd);
            listSize++;
        }
        public int HeatDoublerBuilder(string filename)
        {
            DataTable dbtable = CSVReader.ReadCSVFile(filename, true);
            DataRow[] dbrow = dbtable.Select();

            foreach (DataRow dr in dbrow)
            {
                Models.HeatDoubler hdrow = new Models.HeatDoubler(dr[0].ToString(), System.Convert.ToDouble(dr[1]),
                    System.Convert.ToDouble(dr[2]), System.Convert.ToDouble(dr[3]));
                this.HeatDoublerAppend(hdrow);

                //MainWindow.storeDB.StoreData_VirtualHeater(hdrow, 0);
            }
            //ShowImportedData.ItemsSource = dbtable.AsDataView();

            //WorkSpaceInstance.HeatDoublerInstances = hdlist;
            //paras.UsingVirtualHeater = 1;

            return 1;
        }
    }
}
