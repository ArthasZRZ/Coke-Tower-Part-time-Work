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

            VTKFormRender form = new VTKFormRender(paras, null, MainWindow.WorkSpaceInstance);
            form.TopLevel = false;
            winform.Child = form;
        }
               
        //Communication with the file system
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

                        //Test Import Successful
                        
                    }
                }
            }
        }
        private void SaveProj_Click_1(object sender, RoutedEventArgs e)
        {
            //The "save project" routine is based on the directory
            //Delete the old database -- This is not optimized
            OdbFactory.Delete(WorkSpaceInstance.DBNAME);
            //Store the WorkSpaceInstance in the database
            //MessageBox.Show(WorkSpaceInstance.DBNAME);
            using (var odb = OdbFactory.Open(WorkSpaceInstance.DBNAME))
            {
                odb.Store(WorkSpaceInstance);
                //MessageBox.Show(WorkSpaceInstance.Category.CategoryName);
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

        private void WorkingStatusAnalysis_Click_1(object sender, RoutedEventArgs e)
        {
            Views.WorkingStatusWindow wswindow = new Views.WorkingStatusWindow();
            wswindow.Show();
        }

        private void ComfirmSettingButton_Click_1(object sender, RoutedEventArgs e)
        {
            /*
             * This is the interface between the main window and form window
             * 1. Set the arguments which will be needed:
             *    Argument list:
             *    1. RotateAngle
             *    2. UsingEdges
             *    3. StageID
             *    4. is3DReady
             */
            
            //First: Get the arguments from the user's setting
            paras.RotateAngle = System.Convert.ToInt16(AngleBox.Text);
            paras.UsingEdges = SchemeBox.SelectedIndex;
            paras.StageID = BuildPhaseBox.SelectedIndex - 1;
            paras.Height = winform.Height;
            paras.Width = winform.Width;

            is3Dready = true;

            //Second: Pre-Build Model
            Models.VTKFormPreExecutor preExecutor = new Models.VTKFormPreExecutor();
            int EnvCheck = preExecutor.FormEnvironmentChecker(paras);
            if (EnvCheck == 1)
            {
                // The Environment Check has been passed
                // First: Build the environment 
                if (WorkSpaceInstance.Env.BuildLisFileExecutor(paras) == -1)
                {
                    MessageBox.Show("文件不存在");
                    return;
                }

                // Second: Get the model
                TowerModel newTowerModel = preExecutor.TowerModelGetter( WorkSpaceInstance.Env.LisFileExecutor, 
                                                                         WorkSpaceInstance.Env.TowerModelList,
                                                                         WorkSpaceInstance.TowerModelInstance,
                                                                         paras );
                BuildForm(newTowerModel);
            }
            else if (EnvCheck == -1)
            {
                //Build Origin model
                BuildForm(WorkSpaceInstance.TowerModelInstance);
            }
        }

        public void BuildForm(TowerModel model)
        {
            is3Dready = true;
            VTKFormRender form = new VTKFormRender(paras, model, MainWindow.WorkSpaceInstance);
            form.TopLevel = false;
            winform.Child = form;
        }

        /*
         * BuildModel string will be decided here
         */
        private void TreeView_Selected_1(object sender, RoutedEventArgs e)
        {
            string title = CloudPicTreeView.SelectedValue.ToString();
            int bias = title.IndexOf("标");

            string [] splitter = (title.Substring(bias + 3)).Split(' ');
            paras.BuildModel = null;
            int start_mark = 0;
            for (int i = 0; i < splitter.Count(); i++)
            {
                if (splitter[i].IndexOf("Item") == -1)
                {
                    if (start_mark == 0)
                        start_mark = 1;
                    else
                        paras.BuildModel += " ";
                    paras.BuildModel += splitter[i];
                }   
                else
                    break;
            }
            //MessageBox.Show(paras.BuildModel);
        }

        private void TreeViewSelectMethod(object sender, RoutedEventArgs e)
        {
            TreeViewItem SelectedOne = (TreeViewItem)CloudPicTreeView.SelectedItem;

            paras.ModelID = CloudPicTreeView.Items.IndexOf(SelectedOne.Parent);
            string title = CloudPicTreeView.SelectedValue.ToString();
            int bias = title.IndexOf(":");

            string [] splitter = (title.Substring(bias + 1)).Split(' ');
            if(paras.ModelID != 0)
                paras.SpecialID = int.Parse(splitter[0]) - 1;
            //string ValueString = CloudPicTreeView.SelectedValue;

            paras.BuildModel = null;
            int start_mark = 0;
            for (int i = 1; i < splitter.Count(); i++)
            {
                if (splitter[i].IndexOf("Item") == -1)
                {
                    if (start_mark == 0)
                        start_mark = 1;
                    else
                        paras.BuildModel += " ";
                    paras.BuildModel += splitter[i];
                }
                else
                    break;
            }
        }

        private void RunModel_Click(object sender, RoutedEventArgs e)
        {
            List<object> rtlist = new List<object>();
            rtlist.Add(new { id = "1", info = "开始运行" });
            //gridNoticeData.ItemsSource = rtlist;
            rtlist.Add(new { id = "2", info = "运行结束" });
            //System.Threading.Thread.Sleep(300);
            gridNoticeData.ItemsSource = rtlist;
            paras.StartRun = 1;
        }

        private void RunModelConfigure_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("运行参数设置保持默认");
            Views.RunTimeSettingWindow rtsw = new Views.RunTimeSettingWindow();
            rtsw.Show();
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
    }
}

