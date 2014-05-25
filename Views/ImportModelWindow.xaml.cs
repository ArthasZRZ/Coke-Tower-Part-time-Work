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

namespace WpfRibbonApplication1
{
    /// <summary>
    /// ImportModelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportModelWindow : Window
    {
        private static ImportModelWindow staticInstance = null;
        public ImportModelWindow()
        {
            this.InitializeComponent();
            this.Closed += WindowOnClosed; 
        }
        public static ImportModelWindow GetInstance()
        {
            if (staticInstance == null)
            {
                staticInstance = new ImportModelWindow();
            }

            return staticInstance;
        }
        private void WindowOnClosed(object sender, System.EventArgs e)
        {
            staticInstance = null;
        }

        private void LookUpFile_Node_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "lis files (*.lis)|*.lis";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Controls.TextBox TBNode = NodeFileTextBox;
                TBNode.Text = openFileDialog1.FileName;
            }
        }

        private void LookUpFile_Ele_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "lis files (*.lis)|*.lis";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Controls.TextBox TBEle = EleFileTextBox;
                TBEle.Text = openFileDialog1.FileName;
            }
        }

        private void ComfirmButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox TBNode = NodeFileTextBox;
            System.Windows.Controls.TextBox TBEle = EleFileTextBox;
            string SourcePath_Node = @TBNode.Text;
            string SourcePath_Ele = @TBEle.Text;
            string TargetPath_Node = MainWindow.WorkSpaceInstance.NLIST_FILENAME;
            string TargetPath_Ele = MainWindow.WorkSpaceInstance.ELIST_FILENAME;

            System.IO.File.Copy(SourcePath_Node, TargetPath_Node, true);
            System.IO.File.Copy(SourcePath_Ele, TargetPath_Ele, true);

            this.Close();
            //Here need to import data and draw the picture.

            MainWindow.WorkSpaceInstance.TowerModelInstance.ImportModel(TargetPath_Node, TargetPath_Ele, MainWindow.paras);
        }
    }
}
