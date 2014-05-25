using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfRibbonApplication1.Models
{
    public class CraftsModeExecutor
    {
        /*
         * This class is used for assigning parameters to the environment
         * interact with the basic model classes
         */

        CraftsModeEnv CfEnv = null;

        public CraftsModeExecutor() { }

        public FormParas CraftsModeFormParasGetter() { return CfEnv.paras; }
        public WorkSpaceClass CraftsModeWorkSpaceInstance() { return CfEnv.WorkSpaceInstance; }
        public List<Models.CraftsModePreDefinedModelType> CraftsModeGetModelPreDefined() { return CfEnv.PreDefinedModelList; }

        public int CraftsModeEnvSetter(int Width, int Height)
        {
            CfEnv = new CraftsModeEnv();
            CfEnv.CraftsModeSetFormParas(Width, Height);
            CfEnv.CraftsModeSetWorkSpaceInstance();
            return 1;
        }

        public int CraftsModeEnvModelSetter(int pid, int mid, int sid) 
        {
            CfEnv.CraftsModeSetFormParasModel(pid, mid, sid);
            return 1;
        }
        public void CraftsModeEnvStartRunningSetter()
        {
            CfEnv.paras.StartRun = 1;
            CfEnv.paras.BuildModel = "1";
            return;
        }

        public TowerModel CraftsModePreExecutor()
        {
            Models.VTKFormPreExecutor preExecutor = new Models.VTKFormPreExecutor();
            int EnvCheck = preExecutor.FormEnvironmentChecker(CfEnv.paras);
            if (EnvCheck == 1)
            {
                // The Environment Check has been passed
                // First: Build the environment 
                if (CfEnv.WorkSpaceInstance.Env.BuildLisFileExecutor(CfEnv.paras) == -1)
                {
                    MessageBox.Show("文件不存在");
                    return null;
                }

                // Second: Get the model
                TowerModel newTowerModel = preExecutor.TowerModelGetter(CfEnv.WorkSpaceInstance.Env.LisFileExecutor,
                                                                        CfEnv.WorkSpaceInstance.Env.TowerModelList,
                                                                        CfEnv.WorkSpaceInstance.TowerModelInstance,
                                                                        CfEnv.paras);
                return newTowerModel;
                //BuildForm(newTowerModel);
            }
            else if (EnvCheck == -1)
            {
                return CfEnv.WorkSpaceInstance.TowerModelInstance;
                //Build Origin model
                //BuildForm(WorkSpaceInstance.TowerModelInstance);
            }
            return null;
        }
    }
}
