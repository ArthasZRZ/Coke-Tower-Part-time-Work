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
using System.Windows.Navigation;
using System.Windows.Shapes;


//using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using NDatabase;

namespace WpfRibbonApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static WorkSpaceClass WorkSpaceInstance = null;
        //public static NoticeFromBuilding NoticeInstance = null;
        public static Models.StoreDB storeDB = null;
        public static Boolean is3Dready = false;
        public static FormParas paras = null;

        public MainWindow()
        {
            InitializeComponent();
            WorkSpaceInstance = new WorkSpaceClass();
            storeDB = new Models.StoreDB();

            //WorkSpaceInfo.DataContext = WorkSpaceInstance;
            // Insert code required on object creation below this point.
            paras = new FormParas();
            paras.RotateAngle = 180;
            paras.UsingEdges = 1;
            paras.Using3DTower = 0;
            paras.UsingVirtualHeater = 0;
            paras.Width = winform.Width;
            paras.Height = winform.Height;

            Form1 form = new Form1(paras);
            form.TopLevel = false;
            winform.Child = form;
        }
        
        #region Oridinary functions
        // This is a test function
        public void OpenProjButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //
                string WorkSpaceDir = folderBrowserDialog.SelectedPath;
                using (var odb = OdbFactory.Open(WorkSpaceDir + @"\dbInstance.ndb"))
                {
                    var queryWorkspaceInfo = odb.Query<WorkSpaceClass>();
                    var workspace = queryWorkspaceInfo.Execute<WorkSpaceClass>();
                    foreach (var wkspace in workspace)
                    {
                        //System.Windows.MessageBox.Show(wkspace.NLIST_FILENAME);
                        WorkSpaceInstance = new WorkSpaceClass(wkspace.NLIST_FILENAME, wkspace.ELIST_FILENAME, wkspace.ROOT_DIR, wkspace.DBNAME,
                            wkspace.TowerModelInstance, wkspace.HeatDoublerInstances, wkspace.Category);
                    }
                }
            }
        }

        private void BuildNewProj_Click(object sender, RoutedEventArgs e)
        {
            BuildNewProjWindow.GetInstance().ShowDialog();
        }

        private void ImportModel_Click(object sender, RoutedEventArgs e)
        {
            ImportModelWindow.GetInstance().ShowDialog();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //_3DModel.GetInstance().ShowDialog();
            is3Dready = true;
            FormParas paras = new FormParas();
            paras.RotateAngle = 0;
            paras.UsingEdges = 0;

            Form1 form = new Form1(paras);
            form.TopLevel = false;
            winform.Child = form;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WorkSpaceInstance.TowerModelInstance.RunQhullCmd(@"E:\tower.asc", @"E:\result.off");
        }

        private void button1_Copy_Click_1(object sender, RoutedEventArgs e)
        {
            WorkSpaceInstance.TowerModelInstance.Build3DPointCloud();
        }

        private void WorkingStatusButtonClick(object sender, RoutedEventArgs e)
        {
            WorkingDataImporterWindow newWindow = new WorkingDataImporterWindow();
            newWindow.Show();
        }

        private void VirtualHeat_Click_1(object sender, RoutedEventArgs e)
        {
            HeatDoubleImporter hdwindow = new HeatDoubleImporter();
            hdwindow.Show();
        }

        private void SaveProj_Click_1(object sender, RoutedEventArgs e)
        {
            OdbFactory.Delete(WorkSpaceInstance.DBNAME);
            using (var odb = OdbFactory.Open(WorkSpaceInstance.DBNAME))
            {
                odb.Store(WorkSpaceInstance);
                //MessageBox.Show(WorkSpaceInstance.Category.CategoryName);
            }

        }

        private void WorkingStatusAnalysis_Click_1(object sender, RoutedEventArgs e)
        {
            Views.WorkingStatusWindow wswindow = new Views.WorkingStatusWindow();
            wswindow.Show();
        }
        #endregion

        private void ComfirmSettingButton_Click_1(object sender, RoutedEventArgs e)
        {
            paras.RotateAngle = System.Convert.ToInt16(AngleBox.Text);
            paras.UsingEdges = SchemeBox.SelectedIndex;
            paras.StageID = BuildPhaseBox.SelectedIndex - 1;

            //MessageBox.Show(paras.UsingEdges.ToString());
            is3Dready = true;

            BuildForm();
        }

        public void BuildForm()
        {
            is3Dready = true;
            Form1 form = new Form1(paras);
            form.TopLevel = false;
            winform.Child = form;
        }

        private void TreeView_Selected_1(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> CloudMapping = new Dictionary<string, int>();
            CloudMapping.Add("Temperature", 0);
            CloudMapping.Add("X-Component of Displacement", 1);
            CloudMapping.Add("Y-Component of Displacement", 2);
            CloudMapping.Add("Z-Component of Displacement", 3);
            CloudMapping.Add("X-Component of Stress", 4);
            CloudMapping.Add("Y-Component of Stress", 5);
            CloudMapping.Add("Z-Component of Stress", 6);
            CloudMapping.Add("X-Component of Total Machine Strain", 7);
            CloudMapping.Add("Y-Component of Total Machine Strain", 8);
            CloudMapping.Add("Z-Component of Total Machine Strain", 9);
            CloudMapping.Add("X-Component of Total Machine Plastic Strain", 10);
            CloudMapping.Add("Y-Component of Total Machine Plastic Strain", 11);
            CloudMapping.Add("Z-Component of Total Machine Plastic Strain", 12);

            int I = -1;
            foreach (var dic in CloudMapping)
            {
                string k = dic.Key;
                int idx = CloudPicTreeView.SelectedItem.ToString().IndexOf(k);
                if (idx == -1)
                    continue;
                else
                {
                    I = dic.Value;
                    break;
                }
            }

            paras.BuildModel = I;
        }

        private void RunModel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("运行设置已准备！");
            paras.StartRun = 1;
        }

        private void RunModelConfigure_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("运行参数设置保持默认");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "lis files (*.lis)|*.lis";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                paras.tempFile = openFileDialog1.FileName;
            }
        }

        private void BuildCoord_Copy2_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}

