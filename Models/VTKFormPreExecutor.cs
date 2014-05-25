using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfRibbonApplication1.Models
{
    public class VTKFormPreExecutor
    {
        /*
         * In this class, we decide if the environment is suitable for
         * building VTK model
         * and choose the right model to use, if the wanted model is not 
         * in the list, then build one.
         */
        public VTKFormPreExecutor() { }

        /*
         * This function gives you the result that if the environment is 
         * suitable for generating models
         */
        public int FormEnvironmentChecker(FormParas paras)
        {   
            if (paras.StageID == -1) return -1; // The StageID is ok, there is no more setting
            if (paras.StartRun != 1) { 
                MessageBox.Show("请先运行模型运算"); 
                return 0; 
            } // The calculation hasnt started, need to quit
            if (paras.BuildModel == "") {
                MessageBox.Show("请先选择运算结果名称");
                return 0;
            } // The model type hasnt been choosen, need to quit
            return 1;
        }
        /*
         * This function returns the tower model name you will use
         */
        public string TowerModelNameGetter(LISFileReader EnvLisFileReader)
        {
            return EnvLisFileReader.TowerNameGetter();
        }
        /*
         * This function looks up the model list for the model name
         */
        public TowerModel TowerModelFinder(string TowerModelName, TowerListClass EnvTowerModelList)
        {
            int EnvFindResultIdx = EnvTowerModelList.FindTowerModelByName(TowerModelName);
            if (EnvFindResultIdx == -1)
                return null;
            else
                return EnvTowerModelList.getTowerModel(EnvFindResultIdx);
        }
        /*
         * This function will decide if you need to build a new model
         * And how to build the model
         * And where to put the model
         */
        public TowerModel TowerModelBuilder(TowerModel TowerModelOrigin, FormParas EnvFormParams, 
                                            LISFileReader EnvLisFileReader, TowerListClass TowerModelList)
        {
            // First: Transfer the basic datas
            TowerModel NewTowerModel = new TowerModel();
            NewTowerModel.TowerModelBasicDataCopier(TowerModelOrigin);

            Models.ModelPropertiesResolver mResolver = new ModelPropertiesResolver();
            
            // Second: Build the color table
            mResolver.ModelColorTableResolver(EnvLisFileReader.param.ModelTypeId, NewTowerModel, EnvLisFileReader);
            // Third: Revise the tower elements
            /*
            if (EnvFormParams.ifReviseTowerModelParameters == 1)
                mResolver.ModelParameterResolver(EnvLisFileReader.param.ModelTypeId, NewTowerModel, EnvLisFileReader);
            */
            // Last: Put the model in the list
            string TowerModelName = TowerModelNameGetter(EnvLisFileReader);
            TowerModelList.InsertTowerModel(NewTowerModel, TowerModelName);

            return NewTowerModel;
        }

        public TowerModel TowerModelGetter(LISFileReader EnvLisFileReader, TowerListClass EnvTowerModelList, TowerModel TowerModelOrigin,
                                            FormParas EnvFormParams)
        {
            /*
             * First we try to find the model you want
             */
            string TowerModelName = TowerModelNameGetter(EnvLisFileReader);
            TowerModel EnvRetTowerModel = TowerModelFinder(TowerModelName, EnvTowerModelList);
            if (EnvRetTowerModel != null) // Get it!
                return EnvRetTowerModel;
            else
            {
                //Here to build a new model and return
                return TowerModelBuilder(TowerModelOrigin, EnvFormParams, EnvLisFileReader, EnvTowerModelList);
            }
        }
    }
}
