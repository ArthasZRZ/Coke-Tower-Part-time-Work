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
using Kitware.VTK;

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
            CfExe.CraftsModeEnvSetter(450, 500);
            //MessageBox.Show(this.WinFormGrid.ActualWidth.ToString() + ' ' + this.WinFormGrid.ActualHeight.ToString());
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
        private void FrontViewBtn_Click_1(object sender, RoutedEventArgs e)
        {
            VTKFormRender form = (VTKFormRender)CraftsModeWinForm.Child;
            vtkRenderer ren = form.renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkRenderWindow renWin = form.renderWindowControl1.RenderWindow;

            vtkCamera camera = ren.GetActiveCamera();
            camera.SetRoll(form.StoredViewCamera[0].GetRoll());
            camera.SetPosition(form.StoredViewCamera[0].GetPosition()[0],
                               form.StoredViewCamera[0].GetPosition()[1],
                               form.StoredViewCamera[0].GetPosition()[2]);
            camera.SetFocalPoint(form.StoredViewCamera[0].GetFocalPoint()[0],
                                 form.StoredViewCamera[0].GetFocalPoint()[1],
                                 form.StoredViewCamera[0].GetFocalPoint()[2]);
            camera.SetViewUp(form.StoredViewCamera[0].GetViewUp()[0],
                             form.StoredViewCamera[0].GetViewUp()[1],
                             form.StoredViewCamera[0].GetViewUp()[2]);

            renWin.Render();
        }

        private void SideViewBtn_Click_1(object sender, RoutedEventArgs e)
        {
            VTKFormRender form = (VTKFormRender)CraftsModeWinForm.Child;
            vtkRenderer ren = form.renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkRenderWindow renWin = form.renderWindowControl1.RenderWindow;

            vtkCamera camera = ren.GetActiveCamera();
            camera.SetRoll(form.StoredViewCamera[1].GetRoll());
            camera.SetPosition(form.StoredViewCamera[1].GetPosition()[0],
                               form.StoredViewCamera[1].GetPosition()[1],
                               form.StoredViewCamera[1].GetPosition()[2]);
            camera.SetFocalPoint(form.StoredViewCamera[1].GetFocalPoint()[0],
                                 form.StoredViewCamera[1].GetFocalPoint()[1],
                                 form.StoredViewCamera[1].GetFocalPoint()[2]);
            camera.SetViewUp(form.StoredViewCamera[1].GetViewUp()[0],
                             form.StoredViewCamera[1].GetViewUp()[1],
                             form.StoredViewCamera[1].GetViewUp()[2]);

            renWin.Render();
        }

        private void VerticalViewBtn_Click_1(object sender, RoutedEventArgs e)
        {
            VTKFormRender form = (VTKFormRender)CraftsModeWinForm.Child;
            vtkRenderer ren = form.renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkRenderWindow renWin = form.renderWindowControl1.RenderWindow;

            vtkCamera camera = ren.GetActiveCamera();
            camera.SetRoll(form.StoredViewCamera[2].GetRoll());
            camera.SetPosition(form.StoredViewCamera[2].GetPosition()[0],
                               form.StoredViewCamera[2].GetPosition()[1],
                               form.StoredViewCamera[2].GetPosition()[2]);
            camera.SetFocalPoint(form.StoredViewCamera[2].GetFocalPoint()[0],
                                 form.StoredViewCamera[2].GetFocalPoint()[1],
                                 form.StoredViewCamera[2].GetFocalPoint()[2]);
            camera.SetViewUp(form.StoredViewCamera[2].GetViewUp()[0],
                             form.StoredViewCamera[2].GetViewUp()[1],
                             form.StoredViewCamera[2].GetViewUp()[2]);

            renWin.Render();
        }

        private void Menu_Click_1(object sender, RoutedEventArgs e)
        {
            Models.CraftsModeExecutor CfExe = new Models.CraftsModeExecutor();
            //Set the environment
            CfExe.CraftsModeEnvSetter((int)this.WinFormGrid.Width, (int)this.WinFormGrid.Height);

            MenuItem mi = e.Source as MenuItem;

            if (mi.Header.ToString() != "System.Windows.Controls.TextBlock")
            {
                string mi_header = (string)mi.Header;
                MenuItem mi_parent = (MenuItem)mi.Parent;

                string mi_p_header = (string)mi_parent.Header;

                int StageId = 0, ModelId = 0, SpecialId = 0;
                if (mi_header == "温度模型")
                {
                    StageId = NameToStageId(mi_p_header);
                    ModelId = 0;
                    SpecialId = 0;
                }
                else
                {
                    MenuItem mi_pparent = (MenuItem)mi_parent.Parent;
                    string mi_pp_header = (string)mi_pparent.Header;

                    StageId = NameToStageId(mi_pp_header);
                    ModelId = NameToModelId(mi_p_header);
                    SpecialId = int.Parse(mi_header.Split(' ')[0]);
                }


                CfExe.CraftsModeEnvModelSetter(StageId, ModelId, SpecialId);
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
        private int NameToStageId(string name)
        {
            if (name == "预热阶段")
                return 0;
            else if (name == "进油生焦阶段")
                return 1;
            else if (name == "吹气冷焦阶段")
                return 2;
            else if (name == "给水冷焦阶段")
                return 3;
            else
                return -1;
        }
        private int NameToModelId(string name)
        {
            if (name == "温度模型")
                return 0;
            else if (name == "变形模型")
                return 1;
            else if (name == "应力模型")
                return 2;
            else if (name == "弹性应变模型")
                return 3;
            else if (name == "塑性应变模型")
                return 4;
            else
                return -1;
        }
	}
}
