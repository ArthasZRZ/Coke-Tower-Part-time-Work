using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.ComponentModel;
using Kitware.VTK;

namespace WpfRibbonApplication1
{
    public class ListElemBase
    {
        private const int NumberOfNodes = 8;
        private const int NumberOfModels = 5;

        public int Elem_Number;
        public int[] Elem_Modeltype;
        public int[] Elem_Nodes;
        public ListElemBase(int ElemNumber, int[] M, int[] N)
        {
            Elem_Modeltype = new int[5];
            Elem_Nodes = new int[8];

            Elem_Number = ElemNumber;
            for(int i = 0; i < NumberOfModels; i++){
                Elem_Modeltype[i] = M[i];
            }
            for(int i = 0; i < NumberOfNodes; i++){
                Elem_Nodes[i] = N[i];
            }
        }
        public int GetNumberOfNodes() { return NumberOfNodes; }
        public int GetNumberOfModels() { return NumberOfModels; }
        public int GetElemCorrespondingNodesByIdx(int NodeId) { return Elem_Nodes[NodeId]; }
    }
    public class ListNodeBase
    {
        public int Node_id;
        public int Node_Number;
        public double[] Node_Coord;
        public ListNodeBase(int Nodeid, int NodeNumber, double X, double Y, double Z)
        {
            Node_Coord = new double[3];
            Node_Coord[0] = X;
            Node_Coord[1] = Y;
            Node_Coord[2] = Z;
            Node_Number = NodeNumber;
            Node_id = Nodeid;
        }
    }
    public class NodeListCompare : IComparer<ListNodeBase>
    {
        public NodeListCompare()
        {

        }
        public int Compare(ListNodeBase x, ListNodeBase y)
        {
            if (x.Node_Coord[1] == y.Node_Coord[1])
                return x.Node_Coord[0].CompareTo(y.Node_Coord[0]);
            else
                return x.Node_Coord[1].CompareTo(y.Node_Coord[1]);
        }
    }
    public class BoundaryLine
    {
        public int x, y;
        public BoundaryLine(int x, int y) { 
            this.x = x; 
            this.y = y; 
        }
    }
    public class TowerModel
    {
        public List<ListElemBase> ElemList;
        public List<ListNodeBase> NodeList;
        public Dictionary<int, int> NodeElemTable;

        Models.ModelColorGenerator ColorGen;
        int ElemListSize, NodeListSize;

        public TowerModel()
        {
        }
        public TowerModel(TowerModel prevTowerModel)
        {
            ElemList = new List<ListElemBase>(prevTowerModel.ElemList.ToArray());
            NodeList = new List<ListNodeBase>(prevTowerModel.NodeList.ToArray());
            NodeElemTable = new Dictionary<int, int>(prevTowerModel.NodeElemTable);
            ElemListSize = prevTowerModel.ElemListSize;
            NodeListSize = prevTowerModel.NodeListSize;
            ColorGen = new Models.ModelColorGenerator();
        }
        

        //Related to Elem n Node
        public bool IfNodeIdInNodeElemTable(int NodeId) { return NodeElemTable.ContainsKey(NodeId); }

        public int GetNodeId(ListNodeBase node) { return node.Node_Number; }
        public int GetElemId(ListElemBase elem) { return elem.Elem_Number; }
        public int GetNodePositionInNodeListFromNodeId(int NodeId) { return IfNodeIdInNodeElemTable(NodeId) ? NodeElemTable[NodeId] : -1; }

        //Related To ColorGenerator
        public void SetColorGen(Models.ModelColorGenerator ColorGen) { this.ColorGen = ColorGen; }
        
        public int GetColorGenColorTableSize() { return ColorGen.GetColorTableSize(); }
        public double GetColorGenColorTableValue(int id) { return ColorGen.GetDivisionValueBaseOnPosition(id); }
        public double GetColorGenColorTableMaxValue() { return ColorGen.GetMaxValue(); }
        public double GetColorGenColorTableMinvalue() { return ColorGen.GetMinValue(); }
        private double GetElemToColorValue(int Id) { return ColorGen.GetElemToColorValue(Id); }
        private double GetElemToProperty(int ElemId) { return ColorGen.GetElemToPropertyValue(ElemId); }
        private double GetNodeToProperty(int NodeId) { return ColorGen.GetNodeToPropertyValue(NodeId); }
        private double GetNodeToScalarValue(int NodeId) { return GetNodeToProperty(NodeId); }

        private bool IfElemToColorKeyExists(int Id) { return ColorGen.IfElemToColorKeyExist(Id); }
        private bool IfNodeToColorKeyExists(int Id) { return ColorGen.IfNodeToColorKeyExist(Id); }
        private bool IfElemToPropertyKeyExists(int Id) { return ColorGen.IfElemToPropertyKeyExist(Id); }
        

        /*
         * This function is used for building a new TowerModel by 
         * using the former TowerModel's data
         */
        public void TowerModelBasicDataCopier( TowerModel TowerModelOrigin )
        {
            /*
             * These 3 elements are the most important ones
             */
            ElemList = new List<ListElemBase>(TowerModelOrigin.ElemList.ToArray());
            NodeList = new List<ListNodeBase>(TowerModelOrigin.NodeList.ToArray());
            NodeElemTable = new Dictionary<int, int>(TowerModelOrigin.NodeElemTable);

        }

        public void ImportModel(string fnNode, string fnEle, FormParas paras)
        {
            ElemList = new List<ListElemBase>();
            NodeList = new List<ListNodeBase>();
            NodeElemTable = new Dictionary<int, int>();
            //ElemColorMatcher = new Dictionary<int, int>();

            FileStream fsNode = new FileStream(fnNode, FileMode.Open, FileAccess.Read);
            FileStream fsElem = new FileStream(fnEle, FileMode.Open, FileAccess.Read);
            StreamReader srNode = new StreamReader(fsNode);
            StreamReader srElem = new StreamReader(fsElem);
            srNode.BaseStream.Seek(0, SeekOrigin.Begin);
            srElem.BaseStream.Seek(0, SeekOrigin.Begin);

            NodeListSize = ElemListSize = 0;

            // iterate NLIST.lis
            string tmpNode = srNode.ReadLine();
            int NodeBegin = 0;
            while (tmpNode != null)
            {
                string[] tmpNodeSplit = Regex.Split(tmpNode.Trim(), " ", RegexOptions.IgnoreCase);
                if (tmpNodeSplit[0] == "NODE")
                {
                    NodeBegin = 1;
                }
                else if (NodeBegin == 1 && tmpNodeSplit[0] != "")
                {
                    int NodeNumber = int.Parse(tmpNodeSplit[0]);

                    double[] Coord = new double[3];
                    int cnt = 0;
                    int start = 0;
                    foreach (string str in tmpNodeSplit)
                    {
                        if (start == 0)
                            start = 1;
                        else if (str != "")
                        {
                            Coord[cnt] = System.Convert.ToDouble(str);
                            cnt++;
                        }
                    }
                    NodeList.Add(new ListNodeBase(NodeListSize, NodeNumber, Coord[0], Coord[1], Coord[2]));
                    NodeListSize++;
                }
                tmpNode = srNode.ReadLine();
            }
            /*
             * NodeElemTabel is a map from the node list id to real node id
             */
            int cntn = 0;
            foreach (ListNodeBase node in NodeList){
                NodeElemTable.Add(node.Node_Number, cntn);
                cntn++;
            }


            // iterate ELIST.lis
            string tmpElem = srElem.ReadLine();
            int ElemBegin = 0;
            while (tmpElem != null)
            {
                string[] tmpElemSplit = Regex.Split(tmpElem.Trim(), " ", RegexOptions.IgnoreCase);
                if (tmpElemSplit[0] == "ELEM")
                {
                    ElemBegin = 1;
                }
                else if (ElemBegin == 1 && tmpElemSplit[0] != "")
                {
                    int ElemNumber = int.Parse(tmpElemSplit[0]);
                    int[] M = new int[5];
                    int[] N = new int[8];
                    int cntM = 0, cntN = 0;
                    int start = 0;
                    foreach (string str in tmpElemSplit)
                    {
                        if (start == 0)
                            start++;
                        else if (str != "")
                        {
                            if (cntM == 5)
                            {
                                N[cntN] = int.Parse(str);
                                cntN++;
                            }
                            else
                            {
                                M[cntM] = int.Parse(str);
                                cntM++;
                            }
                        }
                    }
                    //Elemlist reorder
                    int[] new_N = new int[8];
                    new_N[0] = N[0];
                    new_N[2] = N[1];
                    new_N[4] = N[2];
                    new_N[6] = N[3];
                    new_N[1] = N[4];
                    new_N[3] = N[5];
                    new_N[5] = N[6];
                    new_N[7] = N[7];

                    ElemList.Add(new ListElemBase(ElemNumber, M, new_N));
                    ElemListSize++;
                }
                tmpElem = srElem.ReadLine();
            }

            paras.Using3DTower = 1;

        }
        
        /*********************************************
         * VTK functions:
         * DrawModel -- Basic function to draw model
         * DrawEdgesModel -- Add grids to the model
         * LabelGetter -- Add label to the model
         *********************************************/
        public int VTKDrawModel(ref vtkPoints points, 
                                 ref vtkCellArray strips, 
                                 ref vtkFloatArray scalars, 
                                 ref int pointsNum,
                                 FormParas paras)
        {
            // The following routine is built for general purpose

            for (int i = 0; i < NodeList.Count(); i++)
            {
                points.InsertPoint(i,
                    NodeList[i].Node_Coord[0],
                    0,
                    -NodeList[i].Node_Coord[1]);
            }
            pointsNum = NodeList.Count();

            if(paras.StageID != -1)
                scalars.SetNumberOfValues(ElemList.Count());
            int cnt = 0;
            double MAX_R = 0;
            double MAX_R_VALUE = 0;
            int MAX_ELEM_NUM = 0;
            foreach (ListElemBase elem in ElemList)
            {
                strips.InsertNextCell(8);
                if (paras.StageID == -1)
                    scalars.InsertNextValue(elem.Elem_Modeltype[0]);
                else
                {
                    if (IfElemToPropertyKeyExists(elem.Elem_Number))
                        scalars.SetValue(cnt, (float)GetElemToProperty(elem.Elem_Number));
                    else
                        scalars.SetValue(cnt, 0);
                    //MessageBox.Show(((float)GetElemToProperty(elem.Elem_Number)).ToString());
                }
                
                for (int i = 0; i < 8; i++)
                {
                    int NodeId = elem.Elem_Nodes[i];
                    int NodePosInPointsArray = GetNodePositionInNodeListFromNodeId(NodeId);
                    strips.InsertCellPoint(NodePosInPointsArray);
                    if (paras.StageID != -1)
                    {
                        if (NodeList[NodePosInPointsArray].Node_Coord[0] > MAX_R)
                        {
                            MAX_R = NodeList[NodePosInPointsArray].Node_Coord[0];
                            if (IfElemToPropertyKeyExists(elem.Elem_Number))
                                MAX_R_VALUE = GetElemToProperty(elem.Elem_Number);
                            else
                                MAX_R_VALUE = 0;
                            MAX_ELEM_NUM = elem.Elem_Number;
                        }
                    }
                }
                
                cnt++;
            }
            //MessageBox.Show(MAX_R.ToString() + ' ' + MAX_R_VALUE.ToString() + ' ' + MAX_ELEM_NUM.ToString());

            return 1;
        }
        public void VTKDrawEdgesModel(ref vtkPoints points,
                                    ref vtkCellArray strips,
                                    FormParas paras)
        {
            
            int cnt = 0;
            for (int i = 0; i < NodeListSize; i++)
            {
                points.InsertPoint(cnt,
                    NodeList[i].Node_Coord[0],
                    0,
                    -NodeList[i].Node_Coord[1]);
                cnt++;

            }

            foreach (ListElemBase elem in ElemList)
            {
                for (int i = 0; i < 8; i++)
                {
                    strips.InsertNextCell(2);
                    if (i != 7)
                    {
                        strips.InsertCellPoint((int)NodeElemTable[elem.Elem_Nodes[i]] - 1);
                        strips.InsertCellPoint((int)NodeElemTable[elem.Elem_Nodes[i + 1]] - 1);
                    }
                    else
                    {
                        strips.InsertCellPoint((int)NodeElemTable[elem.Elem_Nodes[i]] - 1);
                        strips.InsertCellPoint((int)NodeElemTable[elem.Elem_Nodes[0]] - 1);
                    }
                }
            }

        }

        public void VTKLabelGetter(ref vtkPoints pointsrc, ref vtkStringArray strArr, ref vtkCellArray cellArr, FormParas paras, 
                                    WorkSpaceClass WorkSpaceInstance)
        {
            Models.HeatDoublers hdlist = WorkSpaceInstance.HeatDoublerInstances;

            //MessageBox.Show(hdlist.listSize.ToString());

            strArr.SetNumberOfValues(hdlist.listSize);
       
            strArr.SetName("111");
            for (int i = 0; i < hdlist.listSize; i++)
            {
                pointsrc.InsertNextPoint(hdlist.list[i].X, 0, -hdlist.list[i].Y);
                strArr.SetValue(i, hdlist.list[i].Name);
                //MessageBox.Show(hdlist.list[i].Name);
                cellArr.InsertNextCell(1);
                cellArr.InsertCellPoint(i);
            }
        }
    }
}
