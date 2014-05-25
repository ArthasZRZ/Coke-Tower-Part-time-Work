using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfRibbonApplication1.Models
{
    public class ModelColorGenerator
    {
        const int COLOR_TABLE_SIZE = 10;
        double MIN_VALUE, MAX_VALUE;
        Dictionary<int, double> ElemToProperty;
        Dictionary<int, double> NodeToProperty;
        Dictionary<int, int> ElemToColor;
        Dictionary<int, int> NodeToColor;
        List<double> DivisionValues;

        public ModelColorGenerator() {
            MIN_VALUE = 1000.0;
            MAX_VALUE = -1000.0;
            ElemToProperty = new Dictionary<int,double>();
            ElemToColor = new Dictionary<int, int>();
            NodeToProperty = new Dictionary<int, double>();
            NodeToColor = new Dictionary<int, int>();
            DivisionValues = new List<double>();
        }

        public bool IfElemToColorKeyExist(int ElemId) { return ElemToColor.ContainsKey(ElemId); }
        public bool IfNodeToColorKeyExist(int NodeId) { return NodeToColor.ContainsKey(NodeId); }
        public bool IfNodeToPropertyKeyExist(int NodeId) { return NodeToProperty.ContainsKey(NodeId); }
        public bool IfElemToPropertyKeyExist(int ElemId) { return ElemToProperty.ContainsKey(ElemId); }

        public double GetMinValue() { return MIN_VALUE; }
        public double GetMaxValue() { return MAX_VALUE; }
        public Dictionary<int, int> GetElemToColor() { return ElemToColor; }
        public double GetElemToColorValue(int ElemId) { return ElemToColor[ElemId]; }
        public int GetColorTableSize() { return COLOR_TABLE_SIZE; }
        public int GetColorValue(int id){ return ElemToColor[id]; }
        public double GetElemToPropertyValue(int ElemId) { return (ElemToProperty.ContainsKey(ElemId) ? ElemToProperty[ElemId] : -1); }
        public double GetNodeToPropertyValue(int NodeId) { return NodeToProperty[NodeId]; }

        public int GetPropertyCorrespondingPosition(double property)
        {
            return (int)(((double)(property - MIN_VALUE) / (MAX_VALUE - MIN_VALUE)) * (COLOR_TABLE_SIZE));
        }
        public double GetDivisionValueBaseOnPosition(int id) { return DivisionValues[id]; }

        public void AddElemPropertyValue(double property, int ElemId)
        {
            ElemToProperty.Add(ElemId, property);
        }
        public void AddNodePropertyValue(double property, int NodeId)
        {
            NodeToProperty.Add(NodeId, property);
        }

        private void ResetMinMaxValue()
        {
            MAX_VALUE = -1000.0;
            MIN_VALUE = 1000.0;
        }
        public void SetElemPropertyMinMaxValue()
        {
            ResetMinMaxValue();
            foreach (KeyValuePair<int, double> Pair in ElemToProperty)
            {
                double property = Pair.Value;
                if (MAX_VALUE < property) MAX_VALUE = property;
                if (MIN_VALUE > property) MIN_VALUE = property;
            }
        }
        public void SetNodePropertyMinMaxValue()
        {
            foreach (KeyValuePair<int, double> Pair in NodeToProperty)
            {
                double property = Pair.Value;
                if (MAX_VALUE < property) MAX_VALUE = property;
                if (MIN_VALUE > property) MIN_VALUE = property;
            }
        }

        
        public void SetGivenColorTable()
        {
            foreach (KeyValuePair<int, double> Pair in ElemToProperty )
            {
                ElemToColor.Add(Pair.Key, GetPropertyCorrespondingPosition(Pair.Value));
            }
        }
        
        public void SetDivisionValues()
        { 
            double d = (MAX_VALUE - MIN_VALUE) / ( COLOR_TABLE_SIZE - 1 );
            for (int i = 0; i < COLOR_TABLE_SIZE; i++)
            {
                DivisionValues.Add(d * i + MIN_VALUE);
            } 
        }
    }
}
