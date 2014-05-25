using System;
using System.Collections.Generic;
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
	/// CraftsMode.xaml 的交互逻辑
	/// </summary>
	public partial class CraftsMode : Window
	{
		public CraftsMode()
		{
            this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
            //First: Chart -- Working Status
            Models.WorkStatusDataSeriesGenerator DataSeriesGenerator = new Models.WorkStatusDataSeriesGenerator();

            DataSeriesGenerator.CSVImporter();
            StatusChart.Series.Add(DataSeriesGenerator.GetTowerTopSeries());
            StatusChart.Series.Add(DataSeriesGenerator.GetTowerBottomSeries());
            StatusChart.Series[0].Name = "塔顶";
            StatusChart.Series[1].Name = "塔底";
            StatusChart.AnimationEnabled = false;

            //Second: VTK
            Models.CraftsModeExecutor CfExe = new Models.CraftsModeExecutor();
            //Set the environment
            CfExe.CraftsModeEnvSetter((int)this.WinFormGrid.Width, (int)this.WinFormGrid.Height);
            //Get the Model
            TowerModel CraftsModeTowerModel = CfExe.CraftsModePreExecutor();

            FormParas CraftsModeFormParas = CfExe.CraftsModeFormParasGetter();
            WorkSpaceClass CraftsModeWorkSpaceInstance = CfExe.CraftsModeWorkSpaceInstance();

            VTKFormRender CraftsModeForm = new VTKFormRender(CraftsModeFormParas,
                                                             CraftsModeTowerModel,
                                                             CraftsModeWorkSpaceInstance);
            CraftsModeForm.TopLevel = false;
            CraftsModeWinForm.Child = CraftsModeForm;

            // Third: Append Child of CheckBox
            foreach (Models.HeatDoubler hd in CraftsModeWorkSpaceInstance.HeatDoublerInstances.list)
            {
                CheckBox cb = new CheckBox();
                cb.Content = hd.Name;
                
                KeyPointsHolder.Children.Add(cb);
            }

            //Fourth: Append Child to the ComboBox
            foreach (Models.CraftsModePreDefinedModelType pd in CfExe.CraftsModeGetModelPreDefined())
            {
                ModelName.Items.Add(pd.GetModelNameString());
            }
		}
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
               
        }
        private void RunModelButtonClick(object sender, RoutedEventArgs e)
        {
            Models.CraftsModeExecutor CfExe = new Models.CraftsModeExecutor();
            //Set the environment
            CfExe.CraftsModeEnvSetter((int)this.WinFormGrid.Width, (int)this.WinFormGrid.Height);
            //Get the Model
            int ModelIdx = ModelName.SelectedIndex - 1;
           
            List<Models.CraftsModePreDefinedModelType> pdList = CfExe.CraftsModeGetModelPreDefined();
            Models.CraftsModePreDefinedModelType pd = pdList[ModelIdx];

            CfExe.CraftsModeEnvModelSetter(pd.GetModelStageId(), pd.GetModelModelId(), pd.GetModelSpecialId());
            CfExe.CraftsModeEnvStartRunningSetter();

            TowerModel CraftsModeTowerModel = CfExe.CraftsModePreExecutor();
            
            FormParas CraftsModeFormParas = CfExe.CraftsModeFormParasGetter();
            WorkSpaceClass CraftsModeWorkSpaceInstance = CfExe.CraftsModeWorkSpaceInstance();

            VTKFormRender CraftsModeForm = new VTKFormRender(CraftsModeFormParas,
                                                             CraftsModeTowerModel,
                                                             CraftsModeWorkSpaceInstance);
            CraftsModeForm.TopLevel = false;
            CraftsModeWinForm.Child = CraftsModeForm;
        }
	}
}