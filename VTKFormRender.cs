using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Kitware.VTK;

namespace WpfRibbonApplication1
{
    public class DataFileType
    {
        public int id;
        public int UpperClass;

    }
    public class FormParas
    {
        public int RotateAngle;
        public int UsingEdges;
        public int UsingVirtualHeater;
        public int Using3DTower;
        public int StageID; //-1 origin stage
        public int ModelID;
        public int SpecialID;
        public string BuildModel; //-1 origin build schema
        public int StartRun;
        public string tempFile;
        public double Width;
        public double Height;
        public int ifReviseTowerModelParameters;
        public int globalEnv;

        public FormParas()
        {
            RotateAngle = 0;
            UsingEdges = 1;
            UsingVirtualHeater = 0;
            Using3DTower = 1;
            StageID = -1;
            BuildModel = "";
            StartRun = 0;
            ifReviseTowerModelParameters = 1;
            Width = 300.0;
            Height = 300.0;
            globalEnv = 1;
        }
    }
    public partial class VTKFormRender : Form
    {
        FormParas paras = null;
        TowerModel TowerModelInstance = null;
        WorkSpaceClass WorkSpaceInstance = null;

        public List<vtkCamera> StoredViewCamera;

        public VTKFormRender(FormParas paras, TowerModel tmpModel, WorkSpaceClass WorkSpaceInstance)
        {
            this.paras = new FormParas();
            this.paras = paras;
            this.TowerModelInstance = tmpModel;
            this.WorkSpaceInstance = WorkSpaceInstance;

            InitializeComponent();
        }

        //Parameters for building a model

        private void BasicVTKBuilder(ref vtkActor actor, ref vtkPoints points, ref vtkCellArray polys,
                                      ref vtkFloatArray scalars, ref vtkLookupTable Luk, ref vtkActor2D actor2D)
        {            
            int pointsNum = 0;

            TowerModelInstance.VTKDrawModel(ref points, ref polys, ref scalars, ref pointsNum, paras);
         
            vtkPolyData profile = vtkPolyData.New();
            
            profile.SetPoints(points);
            profile.SetPolys(polys);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();

            if (paras.RotateAngle == 0)
            {
                profile.GetCellData().SetScalars(scalars);
                mapper.SetInput(profile);
            }
            else
            {
                vtkRotationalExtrusionFilter refilter = vtkRotationalExtrusionFilter.New();
                profile.Update();
                profile.GetCellData().SetScalars(scalars);
                //profile.GetPointData().SetScalars(scalars);
                refilter.SetInput(profile);
                refilter.SetResolution(50);
                refilter.SetAngle(paras.RotateAngle);
                refilter.SetTranslation(0);
                refilter.SetDeltaRadius(0);

                mapper.SetInputConnection(refilter.GetOutputPort());
            }
            
            mapper.SetScalarRange(TowerModelInstance.GetColorGenColorTableMinvalue(),
                                  TowerModelInstance.GetColorGenColorTableMaxValue());
            actor.SetMapper(mapper);

            // This text property is for scalarBar
            vtkTextProperty textProperty = vtkTextProperty.New();
            //textProperty.SetFontFamilyToCourier();
            //textProperty.SetColor(1.0, 1.0, 0.5);
            textProperty.SetFontSize(10);

            vtkScalarBarActor scalarBar = vtkScalarBarActor.New();
            scalarBar.SetLookupTable(mapper.GetLookupTable());
            scalarBar.SetTitle("Color Table");
            scalarBar.SetNumberOfLabels(TowerModelInstance.GetColorGenColorTableSize());
            scalarBar.SetTitleTextProperty(textProperty);
            scalarBar.SetLabelTextProperty(textProperty);
            scalarBar.SetWidth(0.07);
            scalarBar.SetHeight(0.6);
            //scalarBar.SetDrawFrame(1);

            vtkLookupTable hueLut = vtkLookupTable.New();
            hueLut.SetTableRange(TowerModelInstance.GetColorGenColorTableMinvalue(),
                                 TowerModelInstance.GetColorGenColorTableMaxValue());
            hueLut.SetHueRange(0.667, 0);
            hueLut.SetSaturationRange(1, 1);
            hueLut.SetValueRange(1, 1);
            hueLut.SetNumberOfTableValues(TowerModelInstance.GetColorGenColorTableSize());
            hueLut.Build();

            mapper.SetLookupTable(hueLut);
            scalarBar.SetLookupTable(hueLut);
            
            // The actor links the data pipeline to the rendering subsystem
            actor2D = scalarBar;
            //actor.GetProperty().SetColor(0.388, 0.388, 0.388);
        }

        public void BasicVTKBuilderWithoutRunning(ref vtkActor actor, ref vtkPoints points, ref vtkCellArray polys,
                                                  ref vtkFloatArray scalars, ref vtkLookupTable Luk)
        {
            int pointsNum = 0;

            TowerModelInstance.VTKDrawModel(ref points, ref polys, ref scalars, ref pointsNum, paras);

            vtkPolyData profile = vtkPolyData.New();

            profile.SetPoints(points);
            profile.SetPolys(polys);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();

            if (paras.RotateAngle == 0)
            {
                profile.GetCellData().SetScalars(scalars);
                mapper.SetInput(profile);
            }
            else
            {
                vtkRotationalExtrusionFilter refilter = vtkRotationalExtrusionFilter.New();
                profile.Update();
                profile.GetCellData().SetScalars(scalars);
                //profile.GetPointData().SetScalars(scalars);
                refilter.SetInput(profile);
                refilter.SetResolution(50);
                refilter.SetAngle(paras.RotateAngle);
                refilter.SetTranslation(0);
                refilter.SetDeltaRadius(0);

                mapper.SetInputConnection(refilter.GetOutputPort());
            }
            mapper.SetScalarRange(0, 5);
            mapper.SetLookupTable(Luk);
            actor.SetMapper(mapper);
            
            Luk.SetNumberOfTableValues(7);
            Luk.SetTableValue(0, 0, 1, 0, 1); //
            Luk.SetTableValue(1, 0, 0, 0.8, 1); //inner surface
            Luk.SetTableValue(2, 0, 1, 0, 1); //
            Luk.SetTableValue(3, 0, 0, 1, 1); 
            Luk.SetTableValue(4, 0, 0, 0.8, 1); //insider
            Luk.SetTableValue(5, 0, 0.8, 0, 1); //outer surface
            Luk.Build();
        }

        public void ExtractEdgesVTKBuilderWithoutRunning(ref vtkActor actor, ref vtkPoints points, ref vtkCellArray polys,
                                                            ref vtkFloatArray scalars, ref vtkLookupTable Luk)
        {
            int pointsNum = 0;

            TowerModelInstance.VTKDrawModel(ref points, ref polys, ref scalars, ref pointsNum, paras);

            vtkPolyData profile = vtkPolyData.New();

            profile.SetPoints(points);
            profile.SetPolys(polys);

            vtkExtractEdges ExtProfile = new vtkExtractEdges();
            

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();

            if (paras.RotateAngle == 0)
            {
                profile.GetCellData().SetScalars(scalars);
                ExtProfile.SetInput(profile);
                mapper.SetInputConnection(ExtProfile.GetOutputPort());
            }
            else
            {
                vtkRotationalExtrusionFilter refilter = vtkRotationalExtrusionFilter.New();
                profile.Update();
                profile.GetCellData().SetScalars(scalars);
                //profile.GetPointData().SetScalars(scalars);
                ExtProfile.SetInput(profile);
                
                refilter.SetInputConnection(ExtProfile.GetOutputPort());
                refilter.SetResolution(50);
                refilter.SetAngle(paras.RotateAngle);
                refilter.SetTranslation(0);
                refilter.SetDeltaRadius(0);

                mapper.SetInputConnection(refilter.GetOutputPort());
            }
            mapper.SetScalarRange(0, 5);
            mapper.SetLookupTable(Luk);
            actor.SetMapper(mapper);

            Luk.SetNumberOfTableValues(7);
            Luk.SetTableValue(0, 0, 1, 0, 1); //
            Luk.SetTableValue(1, 0, 0, 0.8, 1); //inner surface
            Luk.SetTableValue(2, 0, 1, 0, 1); //
            Luk.SetTableValue(3, 0, 0, 1, 1);
            Luk.SetTableValue(4, 0, 0, 0.8, 1); //insider
            Luk.SetTableValue(5, 0, 0.8, 0, 1); //outer surface
            Luk.Build();
        }

        private vtkActor2D VirtualHeaterVTKBuilder()
        {
            vtkActor2D actor = vtkActor2D.New();

            vtkPoints pointSource = vtkPoints.New();
            vtkStringArray labels = vtkStringArray.New();
            vtkCellArray verts = vtkCellArray.New();

            TowerModelInstance.VTKLabelGetter(ref pointSource, ref labels, ref verts, paras, WorkSpaceInstance);

            //MessageBox.Show(labels.ToString());
            vtkPolyData polyData = vtkPolyData.New();
            polyData.SetPoints(pointSource);
            polyData.SetVerts(verts);
            polyData.GetPointData().AddArray(labels);

            vtkTextProperty textProp = vtkTextProperty.New();
            textProp.SetFontSize(12);
            //textProp.SetColor(1.0, 1.0, 0.5);
            textProp.SetFontFamilyToArial();

            vtkPointSetToLabelHierarchy hie = vtkPointSetToLabelHierarchy.New();
            hie.SetInput(polyData);
            hie.SetMaximumDepth(15);
            hie.SetLabelArrayName("111");
            hie.SetTargetLabelCount(100);
            hie.SetTextProperty(textProp);

            vtkLabelPlacementMapper labelMapper = new vtkLabelPlacementMapper();
            labelMapper.SetInputConnection(hie.GetOutputPort());
            
            vtkFreeTypeLabelRenderStrategy strategy = new vtkFreeTypeLabelRenderStrategy();
            labelMapper.SetRenderStrategy(strategy);
            labelMapper.UseDepthBufferOn();
            labelMapper.SetShapeToNone();
            labelMapper.SetStyleToOutline();
            
            //labelMapper.UseUnicodeStringsOff();

            actor.SetMapper(labelMapper);  
            return actor;
        }

        /*
         * In this function, first need to build the color table,
         * and then add the text
         */

        public void VTKInfoBuilder(ref vtkActor2D actor)
        {
            vtkActor2D colorTextActor = new vtkActor2D();
            vtkTextMapper colorText = new vtkTextMapper();
            colorTextActor.SetMapper(colorText);

            //Generate the string:
            string outPutChartString = "";
            outPutChartString += "[Model Details]\n";
            outPutChartString += "Phase: " + WorkSpaceInstance.Env.GetModelGeneratingPhase() + "\n";
            outPutChartString += "ModelType: " + WorkSpaceInstance.Env.GetModelTypeString() + "\n";
            outPutChartString += "Model Name: " + WorkSpaceInstance.Env.GetCurrentTowerModelName() + "\n";
            outPutChartString += "\n";
            outPutChartString += "Min: " + TowerModelInstance.GetColorGenColorTableMinvalue().ToString() +
                                 " Max: " + TowerModelInstance.GetColorGenColorTableMaxValue().ToString() +
                                 "\n";


            colorText.SetInput(outPutChartString);
            colorText.GetTextProperty().SetFontSize(12);
            colorText.GetTextProperty().SetFontFamilyToArial();
            //colorText.GetTextProperty().SetColor(225, 39, 39);

            colorTextActor.SetDisplayPosition(20, 50);
            actor = colorTextActor;
        }

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            
            // Create components of the rendering subsystem
            //
            vtkRenderer ren1 = renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkRenderWindow renWin = renderWindowControl1.RenderWindow;
            renWin.SetSize((int)paras.Height, (int)paras.Width);

            // Add the actors to the renderer, set the window size
            //
            vtkPoints points = vtkPoints.New();
            vtkCellArray polys = vtkCellArray.New();
            vtkFloatArray scalars = vtkFloatArray.New();
            vtkLookupTable Luk = vtkLookupTable.New();
            if (paras.Using3DTower == 1)
            {
                vtkActor actor1 = new vtkActor();
                vtkActor2D actor2D = new vtkActor2D();
                if (paras.StageID != -1)
                {
                    if (paras.UsingEdges == 1)
                    {
                        MessageBox.Show("单元显示无法使用");
                    }
                    else
                    {
                        BasicVTKBuilder(ref actor1, ref points, ref polys, ref scalars, ref Luk, ref actor2D);
                        ren1.AddActor(actor1);
                        ren1.AddActor2D(actor2D);

                        vtkActor2D textActor = new vtkActor2D();
                        VTKInfoBuilder(ref textActor);
                        ren1.AddActor2D(textActor);
                    }
                }
                else
                {
                    if (paras.UsingEdges == 1)
                        ExtractEdgesVTKBuilderWithoutRunning(ref actor1, ref points, ref polys, ref scalars, ref Luk);
                    else
                        BasicVTKBuilderWithoutRunning(ref actor1, ref points, ref polys, ref scalars, ref Luk);
                    ren1.AddActor(actor1);
                }
            }

            if (paras.UsingVirtualHeater == 1)
            {
                //MessageBox.Show("!");
                vtkActor2D actor2 = VirtualHeaterVTKBuilder();
                ren1.AddActor(actor2);
            }

            
            renWin.Render();
            vtkCamera camera = ren1.GetActiveCamera();

            //camera.ParallelProjectionOn();
            //camera.Elevation(20);
            int[] camera_pos = new int[3];

            if (paras.globalEnv == 1)
                camera_pos[1] = -70;
            else
                camera_pos[1] = -80;

            if (paras.RotateAngle == 0)
            {
                camera_pos[1] = -camera_pos[1];
                camera_pos[0] = camera_pos[2] = 0;
                camera.SetRoll(-2);
            }
            else if (paras.RotateAngle == 180)
            {
                camera_pos[0] = camera_pos[2] = 0;
                camera.SetRoll(180);
            }
            else if (paras.RotateAngle == 90)
            {
                double r = (double)Math.Abs(camera_pos[1]);
                camera_pos[0] = (int)(-r * 0.707);
                camera_pos[1] = (int)(-r * 0.707);
                camera_pos[2] = 0;
                camera.SetRoll(225);
            }
            else if (paras.RotateAngle == 270)
            {
                double r = (double)Math.Abs(camera_pos[1]);
                camera_pos[0] = (int)(r * 0.707);
                camera_pos[1] = (int)(-r * 0.707);
                camera_pos[2] = 0;
                camera.SetRoll(135);
            }

            camera.SetPosition((double)camera_pos[0], (double)camera_pos[1], (double)camera_pos[2]);
            //camera.Yaw(10);
            camera.Elevation(1);
            camera.ParallelProjectionOn();
            camera.Zoom(0.9);
            StoredViewCamera = new List<vtkCamera>();
            for (int i = 0; i < 3; i++)
            {
                vtkCamera ViewCamera = new vtkCamera();
                

                if (i == 0)
                {
                    ViewCamera.SetPosition(camera.GetPosition()[0],
                                           camera.GetPosition()[1],
                                           camera.GetPosition()[2]);
                    ViewCamera.SetRoll(camera.GetRoll());
                }
                else if (i == 1)
                {
                    ViewCamera.SetPosition(80, 0, 0);
                    ViewCamera.SetRoll(180);
                }
                else if (i == 2)
                {
                    ViewCamera.SetPosition(0, 0, -80);
                    ViewCamera.SetRoll(camera.GetRoll());
                }
                ViewCamera.SetFocalPoint(camera.GetFocalPoint()[0],
                                              camera.GetFocalPoint()[1],
                                              camera.GetFocalPoint()[2]);
                ViewCamera.SetViewUp(camera.GetViewUp()[0],
                                    camera.GetViewUp()[1],
                                    camera.GetViewUp()[2]);
                StoredViewCamera.Add(ViewCamera);
            }

            //camera.Zoom(1.5);
        }
    }
}
