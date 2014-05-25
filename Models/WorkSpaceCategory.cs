using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfRibbonApplication1
{
    public class Items
    {
        public string ItemName;
        public int id;

        public Items() { }
        public Items(string itname, int Id) { itname = ItemName; id = Id; }
    }
    public class Product
    {
        public string ProductName;
        public int ProductID;
        public ObservableCollection<Items> ItemList;

        public Product() { }
        public Product(string name, int id, ObservableCollection<Items> itlist)
        {
            ProductName = name;
            ProductID = id;
            ItemList = itlist;
        }
    }
    public class WorkSpaceCategory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private string categoryName;
        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryName"));
            }
        }
        
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Products"));
            }
        }

        public void CategoryAttribute(string categoryName, ObservableCollection<Product> products){
            CategoryName = categoryName;
            Products = products;
        }
    }
}
