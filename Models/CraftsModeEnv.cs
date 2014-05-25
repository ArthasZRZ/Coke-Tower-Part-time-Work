using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfRibbonApplication1.Models
{
    public class CraftsModePreDefinedModelType
    {
        string Name;
        int StageId;
        int ModelId;
        int SpecialId;
        LISFileReader RefLisFileReader;

        public CraftsModePreDefinedModelType() { RefLisFileReader = new LISFileReader(); }

        public string GetModelNameString() { return Name; }
        public int GetModelStageId() { return StageId; }
        public int GetModelModelId() { return ModelId; }
        public int GetModelSpecialId() { return SpecialId; }

        public CraftsModePreDefinedModelType(string Name, int StageId, int ModelId, int SpecialId) 
        {
            this.Name = Name;
            this.StageId = StageId;
            this.ModelId = ModelId;
            this.SpecialId = SpecialId;
        }
        public void SetAsTemperatureModel(int StageId)
        {
            
            this.StageId = StageId;
            this.ModelId = 0;
            this.SpecialId = 0;

            RefLisFileReader = new LISFileReader();
            RefLisFileReader.param = new LISFileParams(this.StageId, this.ModelId, this.SpecialId);
            this.Name = "Model_" + RefLisFileReader.TowerNameGetter();
            //MessageBox.Show(this.Name);
        }
        public void SetAsStrainModel(int StageId, int SpecialId)
        {
            this.StageId = StageId;
            this.ModelId = 1;
            this.SpecialId = SpecialId;
           
            RefLisFileReader = new LISFileReader();
            RefLisFileReader.param = new LISFileParams(this.StageId, this.ModelId, this.SpecialId);
            this.Name = "Model_" + RefLisFileReader.TowerNameGetter();
        }
        public void SetAsPlasticStrainModel(int StageId, int SpecialId)
        {
            this.StageId = StageId;
            this.ModelId = 2;
            this.SpecialId = SpecialId;

            RefLisFileReader = new LISFileReader();
            RefLisFileReader.param = new LISFileParams(this.StageId, this.ModelId, this.SpecialId);
            this.Name = "Model_" + RefLisFileReader.TowerNameGetter();
        }
        public void SetAsStressModel(int StageId, int SpecialId)
        {
            this.StageId = StageId;
            this.ModelId = 3;
            this.SpecialId = SpecialId;

            RefLisFileReader = new LISFileReader();
            RefLisFileReader.param = new LISFileParams(this.StageId, this.ModelId, this.SpecialId);
            this.Name = "Model_" + RefLisFileReader.TowerNameGetter();
        }
    }

    public class CraftsModeEnv
    {
        public FormParas paras = null;
        public WorkSpaceClass WorkSpaceInstance = null;

        public List<CraftsModePreDefinedModelType> PreDefinedModelList;

        public CraftsModeEnv() 
        { 
            //Here we set the Models:
            PreDefinedModelList = new List<CraftsModePreDefinedModelType>();
            
            for (int StageId = 0; StageId < 4; StageId++)
            {
                CraftsModePreDefinedModelType pd = new CraftsModePreDefinedModelType();
                pd.SetAsTemperatureModel(StageId);
                PreDefinedModelList.Add(pd);
            }
        }
        public int CraftsModeSetFormParas(int Width, int Height)
        {
            paras = new FormParas();

            //Set the model show parameters:
            paras.RotateAngle = 180;
            paras.UsingEdges = 0;
            paras.StageID = -1;
            paras.UsingVirtualHeater = 1;
            paras.Width = Width;
            paras.Height = Height;
            paras.globalEnv = 2; // This is the CraftsMode

            return 1;
        }
        public int CraftsModeSetFormParasModel( int pid, int mid, int sid ) 
        {
            paras.StageID = pid;
            paras.ModelID = mid;
            paras.SpecialID = sid;

            return 1;
        }
        public int CraftsModeSetWorkSpaceInstance()
        {
            WorkSpaceInstance = new WorkSpaceClass();

            //Import new model
            string TargetPath_Node = System.AppDomain.CurrentDomain.BaseDirectory + @"\CalDatas\NLIST_new.lis";
            string TargetPath_Elem = System.AppDomain.CurrentDomain.BaseDirectory + @"\CalDatas\ELIST_new.lis";
            WorkSpaceInstance.TowerModelInstance.ImportModel(TargetPath_Node, 
                                                             TargetPath_Elem,
                                                             paras);

            //Import HeatDoubler
            string HeatDoublerFilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"\CalDatas\CokeThermocouple_new.csv";
            WorkSpaceInstance.HeatDoublerInstances.HeatDoublerBuilder(HeatDoublerFilePath);

            return 1;
        }
    }
}
