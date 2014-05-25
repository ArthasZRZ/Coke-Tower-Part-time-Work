using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfRibbonApplication1.Models
{
    public class ModelPropertiesResolver
    {
        public Dictionary<int, int> ElemToColor = new Dictionary<int, int>();

        ModelColorGenerator ColorGen;

        public ModelPropertiesResolver() {
            ColorGen = new ModelColorGenerator();
        }
        public void ModelColorTableResolver(int type, TowerModel tower, Models.LISFileReader lisFile)
        {
            Dictionary<int, LISFileItem> NODE_ID_TO_ITEM = lisFile.ITEM_DICTIONARY;
            int TYPE_ID = lisFile.param.SpecialTypeId;

            switch (type)
            {
                case 0:
                    GetTemperatureColorTable(tower, NODE_ID_TO_ITEM);
                    break;
                case 1:
                    GetDisplacementColorTable(tower, NODE_ID_TO_ITEM, TYPE_ID);
                    break;
                case 2:
                    GetStressColorTable(tower, NODE_ID_TO_ITEM, TYPE_ID);
                    break;
                case 3:
                    GetStrainColorTable(tower, NODE_ID_TO_ITEM, TYPE_ID);
                    break;
                case 4:
                    GetPlasticStrainColorTable(tower, NODE_ID_TO_ITEM, TYPE_ID);
                    break;
            }

            // assign new elem-color table 
            tower.SetColorGen(ColorGen);
        }
        public void ModelParameterResolver(int type, TowerModel tower, Models.LISFileReader lisFile)
        {
            Dictionary<int, LISFileItem> NODE_ID_TO_ITEM = lisFile.ITEM_DICTIONARY;
            int TYPE_ID = lisFile.param.SpecialTypeId;

            switch (type)
            {
                case 0:
                    break;
                case 1:
                    SetDisplacementModelParamters(tower, NODE_ID_TO_ITEM, TYPE_ID);
                    break;
                case 2:
                    SetStrainModelParamters(tower, NODE_ID_TO_ITEM, TYPE_ID);
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

        }
        public void GetTemperatureColorTable(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM)
        {
            foreach (ListElemBase elem in tower.ElemList)
            {
                //Calculate how many nodes are available?
                int NumberOfAvailableNodes = 0;
                for (int i = 0; i < 8; i++)
                    if (NODE_ID_TO_ITEM.ContainsKey(elem.GetElemCorrespondingNodesByIdx(i)))
                        NumberOfAvailableNodes++;
                
                //MessageBox.Show(NumberOfAvailableNodes.ToString());

                if(NumberOfAvailableNodes >= 4){
                    double AVE_TEMP = 0;
                    for (int i = 0; i < 8; i++)
                        if (NODE_ID_TO_ITEM.ContainsKey(elem.Elem_Nodes[i]))
                            AVE_TEMP += NODE_ID_TO_ITEM[elem.Elem_Nodes[i]].VAL[0];

                    // Remember the cnt could be zero!
                    // Here must be a handle function
                    
                    AVE_TEMP /= NumberOfAvailableNodes;
                    ColorGen.AddElemPropertyValue(AVE_TEMP, elem.Elem_Number);
                }
            }
            
            //ColorGen.SetNodePropertyMinMaxValue();
            ColorGen.SetElemPropertyMinMaxValue();
            ColorGen.SetGivenColorTable();
            ColorGen.SetDivisionValues();
        }
        public void GetStrainColorTable(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM, int TYPE_ID)
        {
            foreach (ListElemBase elem in tower.ElemList)
            {
                double AVE_TEMP = 0.0;
                int cnt = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (NODE_ID_TO_ITEM.ContainsKey(elem.Elem_Nodes[i]))
                    {
                        ListNodeBase node = tower.NodeList[tower.NodeElemTable[elem.Elem_Nodes[i]]];
                        LISFileItem lisItem = NODE_ID_TO_ITEM[elem.Elem_Nodes[i]];

                        AVE_TEMP += lisItem.VAL[TYPE_ID];
                        cnt++;
                    }
                }
                if (cnt != 0)
                    AVE_TEMP /= cnt;
                ColorGen.AddElemPropertyValue(AVE_TEMP, elem.Elem_Number);
            }
            ColorGen.SetElemPropertyMinMaxValue();
            ColorGen.SetGivenColorTable();
            ColorGen.SetDivisionValues();
        }
        public void SetStrainModelParamters(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM, int TYPE_ID)
        {
            foreach (ListNodeBase node in tower.NodeList)
            {
                if (NODE_ID_TO_ITEM.ContainsKey(node.Node_id))
                {
                    node.Node_Coord[0] += NODE_ID_TO_ITEM[node.Node_id].VAL[1];
                    node.Node_Coord[1] += NODE_ID_TO_ITEM[node.Node_id].VAL[2];
                    node.Node_Coord[2] += NODE_ID_TO_ITEM[node.Node_id].VAL[3];
                }
            }
        }
        public void GetDisplacementColorTable(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM, int TYPE_ID)
        {
            /*
             * The displacement part has 4 options:
             * 1,2,3: X Y Z
             * 4: Displacement Sum
             */
            const int node_use_number = 8;
            
            foreach (ListElemBase elem in tower.ElemList)
            {
                double value_getter = 0.0;
                for (int i = 0; i < 8; i++)
                {
                    if (NODE_ID_TO_ITEM.ContainsKey(elem.Elem_Nodes[i]))
                    {
                        LISFileItem NodeItemGetter = NODE_ID_TO_ITEM[elem.Elem_Nodes[i]];
                        switch (TYPE_ID)
                        {
                            case 0:
                                value_getter += NodeItemGetter.GetValueListValueByIdx(0);
                                break;
                            case 1:
                                value_getter += NodeItemGetter.GetValueListValueByIdx(1);
                                break;
                            case 2:
                                value_getter += NodeItemGetter.GetValueListValueByIdx(2);
                                break;
                            case 3:
                                value_getter += NodeItemGetter.GetValueListValueByIdx(3);
                                break;
                        }
                        //MessageBox.Show(value_getter.ToString());
                    }
                }

                value_getter /= node_use_number;
                
                ColorGen.AddElemPropertyValue(value_getter, elem.Elem_Number);
            }
            ColorGen.SetElemPropertyMinMaxValue();
            ColorGen.SetGivenColorTable();
            ColorGen.SetDivisionValues();
        }
        public void SetDisplacementModelParamters(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM, int TYPE_ID)
        {
            foreach (ListNodeBase node in tower.NodeList)
            {
                if (NODE_ID_TO_ITEM.ContainsKey(node.Node_id))
                {
                    node.Node_Coord[0] = NODE_ID_TO_ITEM[node.Node_id].VAL[1];
                    node.Node_Coord[1] = NODE_ID_TO_ITEM[node.Node_id].VAL[2];
                    node.Node_Coord[2] = NODE_ID_TO_ITEM[node.Node_id].VAL[3];
                }
            }
        }
        public void GetPlasticStrainColorTable(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM, int TYPE_ID)
        {
            foreach (ListElemBase elem in tower.ElemList)
            {
                double AVE_TEMP = 0.0;
                int cnt = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (NODE_ID_TO_ITEM.ContainsKey(elem.Elem_Nodes[i]))
                    {
                        ListNodeBase node = tower.NodeList[tower.NodeElemTable[elem.Elem_Nodes[i]]];
                        LISFileItem lisItem = NODE_ID_TO_ITEM[elem.Elem_Nodes[i]];

                        AVE_TEMP += lisItem.VAL[TYPE_ID];
                        cnt++;
                    }
                }
                if (cnt != 0)
                    AVE_TEMP /= cnt;
                ColorGen.AddElemPropertyValue(AVE_TEMP, elem.Elem_Number);
            }
            ColorGen.SetElemPropertyMinMaxValue();
            ColorGen.SetGivenColorTable();
            ColorGen.SetDivisionValues();
        }
        public void GetStressColorTable(TowerModel tower, Dictionary<int, LISFileItem> NODE_ID_TO_ITEM, int TYPE_ID)
        {
            foreach (ListElemBase elem in tower.ElemList)
            {
                double AVE_TEMP = 0.0;
                int cnt = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (NODE_ID_TO_ITEM.ContainsKey(elem.Elem_Nodes[i]))
                    {
                        ListNodeBase node = tower.NodeList[tower.NodeElemTable[elem.Elem_Nodes[i]]];
                        LISFileItem lisItem = NODE_ID_TO_ITEM[elem.Elem_Nodes[i]];
                        
                        AVE_TEMP += lisItem.VAL[TYPE_ID];
                        cnt++;
                    }
                }
                if(cnt != 0)
                    AVE_TEMP /= cnt;
                ColorGen.AddElemPropertyValue(AVE_TEMP, elem.Elem_Number);
            }
            ColorGen.SetElemPropertyMinMaxValue();
            ColorGen.SetGivenColorTable();
            ColorGen.SetDivisionValues();
        }
    }
}
