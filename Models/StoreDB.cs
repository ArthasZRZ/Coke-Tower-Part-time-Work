using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WpfRibbonApplication1;
using NDatabase;

namespace WpfRibbonApplication1.Models
{   
    public class StoreDB
    {
        //private string ConnectionString = Properties.Settings.Default.StoreDataBase;

        public StoreDB() { }
        public void StoreData_VirtualHeater(HeatDoubler HD, int isInit)
        {
            //MessageBox.Show(MainWindow.WorkSpaceInstance.DBNAME);
            if (isInit == 1)
            {
                OdbFactory.Delete(MainWindow.WorkSpaceInstance.DBNAME);
            }
            using (var odb = OdbFactory.Open(MainWindow.WorkSpaceInstance.DBNAME))
            {
                odb.Store(HD);
            }
        }
        public WorkSpaceCategory GetCategoriedAndProducts()
        {
            using (var odb = OdbFactory.Open(MainWindow.WorkSpaceInstance.DBNAME))
            {
                var queryWorkCatInfo = odb.Query<WorkSpaceCategory>();
                var workcat = queryWorkCatInfo.Execute<WorkSpaceCategory>();
                WorkSpaceCategory cat = new WorkSpaceCategory();
                foreach (var wkcat in workcat)
                {
                    //MessageBox.Show(wkcat.Products[0].ProductName);
                    cat.CategoryAttribute(wkcat.CategoryName, wkcat.Products);
                }
                return (cat);
            }
        }
    }
}
