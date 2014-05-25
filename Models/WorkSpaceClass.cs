using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfRibbonApplication1
{
    public class TowerListClass
    {
        public List<TowerModel> TowerList;
        public Dictionary<string, int> TowerNameToTowerIndexInList;
        public TowerListClass()
        {
            TowerList = new List<TowerModel>();
            TowerNameToTowerIndexInList = new Dictionary<string, int>();
        }
        public int FindTowerModelByName(string TowerModelName)
        {
            return (TowerNameToTowerIndexInList.ContainsKey(TowerModelName)) ? 
                    TowerNameToTowerIndexInList[TowerModelName] : -1;
        }
        public TowerModel getTowerModel(int ListIndex)
        {
            if (ListIndex < TowerList.Count() && ListIndex >= 0)
                return TowerList[ListIndex];
            else
                return null;
        }
        public void InsertTowerModel(TowerModel TowerModelInstance, string TowerModelName)
        {
            
            if (FindTowerModelByName(TowerModelName) == -1)
            {
                TowerList.Add(TowerModelInstance);
                TowerNameToTowerIndexInList[TowerModelName] = TowerList.Count() - 1;
            }
            // If this model's name is already in the dictionary, just update it
            else
            {
                int ListIndex = FindTowerModelByName(TowerModelName);
                TowerList[ListIndex] = TowerModelInstance;
            }
        }   
    }

    public class WorkSpaceRunModelEnvironment
    {
        public TowerListClass TowerModelList = null;
        public Models.LISFileReader LisFileExecutor = null;
       
        public WorkSpaceRunModelEnvironment() {
            TowerModelList = new TowerListClass();
            LisFileExecutor = new Models.LISFileReader();
        }
        public void AssignLisFileExecutor(Models.LISFileReader lisfilereader)
        {
            LisFileExecutor = lisfilereader;
        }
        public int BuildLisFileExecutor(FormParas paras)
        {
            LisFileExecutor = new Models.LISFileReader();

           
            return LisFileExecutor.WrapperInstaller(paras.StageID, paras.ModelID, paras.SpecialID, paras.BuildModel);
        }

        public string GetModelGeneratingPhase()
        {
            int phaseId = LisFileExecutor.param.PhaseId;
            string result = "";
            switch (phaseId)
            {
                case 0:
                    result = "preheating";
                    break;
                case 1:
                    result = "oilfilling";
                    break;
                case 2:
                    result = "streamcooling";
                    break;
                case 3:
                    result = "watercooling";
                    break;
                default:
                    result = "WRONG: No such Phase ID";
                    break;
            }
            return result;
        }
        public string GetCurrentTowerModelName()
        {
            if (LisFileExecutor.TowerModelName == null)
                return "WRONG: TowerModelName hasn't been set yet";
            else
                return LisFileExecutor.TowerModelName;
        }
        public string GetModelTypeString()
        {
            string result = "";
            switch (LisFileExecutor.param.ModelTypeId)
            {
                case 0:
                    result = "temperature";
                    break;
                case 1:
                    result = "dof";
                    break;
                case 2:
                    result = "stress";
                    break;
                case 3:
                    result = "strain";
                    break;
                case 4:
                    result = "plastic";
                    break;
                default:
                    result = "WRONG: ModelTypeId can't be recognized";
                    break;
            }
            return result;
        }
    }

    public class WorkSpaceClass
    {
        public string NAME { set; get; }
        public string NLIST_FILENAME { set; get; }
        public string ELIST_FILENAME { set; get; }
        public string ROOT_DIR { set; get; }
        public string DBNAME { set; get; }
        public TowerModel TowerModelInstance = null; // This is the base TowerModel
        public Models.HeatDoublers HeatDoublerInstances = null;
        public WorkSpaceCategory Category = null;

        public WorkSpaceRunModelEnvironment Env = null;

        public WorkSpaceClass()
        {
            NLIST_FILENAME = "";
            ELIST_FILENAME = "";
            TowerModelInstance = new TowerModel();
            HeatDoublerInstances = new Models.HeatDoublers();
            ROOT_DIR = "";
            Category = new WorkSpaceCategory();

            Env = new WorkSpaceRunModelEnvironment();
        }
        public WorkSpaceClass(string nfilename, string efilename, string rootdir,
            string dbname, TowerModel twmodel, Models.HeatDoublers hdinst,
            WorkSpaceCategory Category)
        {
            NLIST_FILENAME = nfilename;
            ELIST_FILENAME = efilename;
            ROOT_DIR = rootdir;
            DBNAME  = dbname;
            TowerModelInstance = twmodel;
            HeatDoublerInstances = hdinst;

            this.Category = Category;
        }
    }
}
