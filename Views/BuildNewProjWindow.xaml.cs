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
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace WpfRibbonApplication1
{
    /// <summary>
    /// BuildNewProjWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BuildNewProjWindow : Window
    {
        private static BuildNewProjWindow staticInstance = null;
        public BuildNewProjWindow()
        {
            this.InitializeComponent();
            System.Windows.Controls.ComboBox CBModel = ModelTypeComboBox;
            System.Windows.Controls.ComboBox CBSF = StructureFramework;
            //CBModel.Items.Add("<请选择模型>");
            //CBSF.Items.Add("<请选择结构模板>");
            
            this.Closed += WindowOnClosed; 
        }
        public static BuildNewProjWindow GetInstance()
        {
            if (staticInstance == null)
            {
                staticInstance = new BuildNewProjWindow();
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

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox TBModel = ModelNameTextBox;
            System.Windows.Controls.TextBox TBWorkSpace = WorkSpaceTextBox;
            WorkSpaceTextBox.Text = ModelNameTextBox.Text;
        }

        private void ComfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // This Button is going to build a workspace class in this project
            System.Windows.Controls.TextBox TBPosition = PositionTextBox;
            System.Windows.Controls.TextBox TBWorkSpace = WorkSpaceTextBox;
            if (TBPosition.Text == "" || TBWorkSpace.Text == "")
            {
                MessageBox.Show("有空白区域没有填写！");
            }
            else
            {
                MainWindow.WorkSpaceInstance.NAME = TBWorkSpace.Text;
                string DirString = "";

                DirString += TBPosition.Text;
                DirString += @"\";
                DirString += TBWorkSpace.Text;
                // This is the root dir
                Directory.CreateDirectory(@DirString);
                MainWindow.WorkSpaceInstance.ROOT_DIR = DirString;
                MainWindow.WorkSpaceInstance.NLIST_FILENAME = System.IO.Path.Combine(DirString, "NLIST.lis");
                MainWindow.WorkSpaceInstance.ELIST_FILENAME = System.IO.Path.Combine(DirString, "ELIST.lis");
                MessageBox.Show("在" + @DirString + "建立了一个新的工作区");
                FileCreator();

                ObservableCollection<Product> Products = new ObservableCollection<Product>();
                Products.Add(new Product("坐标系", 0, null));
                Products.Add(new Product("部件", 0, null));
                Products.Add(new Product("虚拟传感器", 0, null));
                Products.Add(new Product("特性", 0, null));
                Products.Add(new Product("工况", 0, null));
                Products.Add(new Product("分析", 0, null));
                Products.Add(new Product("结果", 0, null));

                MainWindow.WorkSpaceInstance.Category.CategoryAttribute(MainWindow.WorkSpaceInstance.NAME, Products);

                this.Close();
            }
        }

        private void LookupFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            System.Windows.Controls.TextBox TBPosition = PositionTextBox;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TBPosition.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void FileCreator()
        {
            //File.Create(MainWindow.WorkSpaceInstance.ROOT_DIR + @"\VirtualHeater.ndb");
            //MessageBox.Show("db file created");
            MainWindow.WorkSpaceInstance.DBNAME = MainWindow.WorkSpaceInstance.ROOT_DIR + @"\dbInstance.ndb";
        }
    }
    
}
