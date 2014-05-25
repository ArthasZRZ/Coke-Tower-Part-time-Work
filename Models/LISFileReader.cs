/*
 *  This class is for the modeling data processing; 
 *  LISFileReader is going to deal with all the file operations
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Windows;


namespace WpfRibbonApplication1.Models
{
    public class LISFileParams
    {
        public int PhaseId;
        public int ModelTypeId;
        public int SpecialTypeId;

        public LISFileParams() { }
        public LISFileParams(int pid, int mtid, int stid)
        {
            PhaseId = pid;
            ModelTypeId = mtid;
            SpecialTypeId = stid;
        }
        public LISFileParams(int pid, string info_str)
        {
            if (info_str == "Temperature")
            {
                PhaseId = pid;
                ModelTypeId = 0;
                SpecialTypeId = 0;
            }
            else
            {
                string[] tmpFileNameSplit = Regex.Split(info_str.Trim(), " ", RegexOptions.IgnoreCase);
                int Size = tmpFileNameSplit.Count();
                //MessageBox.Show(tmpFileNameSplit[Size - 1]);
                switch (tmpFileNameSplit[Size - 1])
                {
                    case "Displacement":
                        ModelTypeId = 1;
                        break;
                    case "Stress":
                        ModelTypeId = 2;
                        break;
                    case "Strain":
                        if (tmpFileNameSplit[Size - 2] == "Plastic")
                            ModelTypeId = 4;
                        else
                            ModelTypeId = 3;
                        break;
                }

                switch (tmpFileNameSplit[0])
                {
                    case "X-Component":
                        SpecialTypeId = 0;
                        break;
                    case "Y-Component":
                        SpecialTypeId = 1;
                        break;
                    case "Z-Component":
                        SpecialTypeId = 2;
                        break;
                }
            }
        }
    }
    public class LISFileItem
    {
        public int ID;
        public List<double> VAL;
        public int VAL_LIST_NUMBER;

        public LISFileItem(int id, List<double> val)
        {
            ID = id;
            VAL = val;
        }
        public double GetValueListValueByIdx(int Id) { return VAL[Id]; }
    }
    public class LISFileReader
    {

        public string DataDir = @"\CalDatas";
        public string PreHeatDir = @"\CalDatas\0PreHeat";
        public string OilFillingDir = @"\CalDatas\1OilFilling";
        public string StreamCoolingDir = @"\CalDatas\2StreamCooling\";
        public string WaterCoolingDir = @"\CalDatas\3WaterCooling\";

        public Dictionary<int, LISFileItem> ITEM_DICTIONARY = null;
        public int VAL_LIST_NUMBER = 1;
        public LISFileParams param = null;

        public string TowerModelName = null;

        public LISFileReader()
        {
            ITEM_DICTIONARY = new Dictionary<int, LISFileItem>();
            param = new LISFileParams();
            VAL_LIST_NUMBER = 1;
        }

        /*
         * This function connects the choices you choose in the treeviewlist
         * to "real file name address"
         */
        public string FileLocator(int fileIdx)
        {
            string UpperDir = null;
            string LowerDir = null;

            switch (param.PhaseId)
            {
                case 0:
                    UpperDir = PreHeatDir;
                    break;
                case 1:
                    UpperDir = OilFillingDir;
                    break;
                case 2:
                    UpperDir = StreamCoolingDir;
                    break;
                case 3:
                    UpperDir = WaterCoolingDir;
                    break;
            }

            switch(param.ModelTypeId) {
                case 0:
                    LowerDir = "PRNSOL_Temperature.lis";
                    break;
                case 1:
                    LowerDir = "PRNSOL_DOF_ALL.lis";
                    break;
                case 2:
                    if (fileIdx == 0)
                        LowerDir = "PRNSOL_Stress_ALL.lis";
                    else if (fileIdx == 1)
                        LowerDir = "PRNSOL_Stress_VonMises.lis";
                    else
                    {
                        MessageBox.Show("WRONG: fileIdx number is out of range");
                        return null;
                    }
                    break;
                case 3:
                    if (fileIdx == 0)
                        LowerDir = "PRNSOL_Strain_ALL.lis";
                    else
                        LowerDir = "PRNSOL_Strain_VonMises.lis";

                    break;
                case 4:
                    if (fileIdx == 0)
                        LowerDir = "PRNSOL_PLASTIC_ALL.lis";
                    else
                        LowerDir = "PRNSOL_PLASTIC_VonMises.lis";
                    break;
            }
            return UpperDir + @"\" + LowerDir;
        }

        /*
         * According to user's choice, here we generate the model's name
         */
        public string TowerNameGetter()
        {

            string hi_name = "", lo_name = "";
            switch (param.PhaseId)
            {
                case 0:
                    hi_name = "preheat";
                    break;
                case 1:
                    hi_name = "oilfilling";
                    break;
                case 2:
                    hi_name = "streamcooling";
                    break;
                case 3:
                    hi_name = "watercooling";
                    break;
            }
            switch (param.ModelTypeId)
            {
                case 0:
                    lo_name = "temperature";
                    break;
                case 1:
                    lo_name = "dof";
                    break;
                case 2:
                    lo_name = "stress";
                    break;
                case 3:
                    lo_name = "strain";
                    break;
                case 4:
                    lo_name = "plastic";
                    break;
            }
            
            TowerModelName = hi_name + " " + lo_name + " " + param.SpecialTypeId.ToString();
            
            return TowerModelName;
        }

        public void CleanTmpLists()
        {
            ITEM_DICTIONARY = new Dictionary<int, LISFileItem>();
            VAL_LIST_NUMBER = 1;
        }

        public int ImportFile(string fname)
        {
            try
            {
                FileStream fstream = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + fname, FileMode.Open, FileAccess.Read);
                StreamReader sfstream = new StreamReader(fstream);
                sfstream.BaseStream.Seek(0, SeekOrigin.Begin);
                string tmpNode = sfstream.ReadLine();

                int NodeBegin = 0;
                while (tmpNode != null)
                {
                    // divide the line it read into parts
                    string[] tmpNodeSplit = Regex.Split(tmpNode.Trim(), " ", RegexOptions.IgnoreCase);
                    if (tmpNodeSplit[0] == "NODE")
                        NodeBegin = 1;
                    else if (NodeBegin == 1 && tmpNodeSplit[0] != "")
                    {
                        /*
                         * if this tmpString belongs to the "data line" and 
                         * "valid dataline"
                         */

                        // Why this line exsit? NodeNumber is useless, because the id contains in the val_list
                        // WRONG!!!!!!
                        // NodeNumber is the parse result
                        int NodeNumber = -1;
                        Boolean tryParse = Int32.TryParse(tmpNodeSplit[0], out NodeNumber);
                        if (!tryParse) NodeBegin = 0;
                        else
                        {
                            /*
                             * Here we've finished the node id parsing step
                             * Then we could add the values that this data line contains
                             */
                            List<double> val_list = new List<double>();
                            for(int t = 1; t < tmpNodeSplit.Count(); t++)
                            {
                                string str = tmpNodeSplit[t];
                                if (str != "")
                                {
                                    // Need to deal with the minus
                                    List<int> mid_mark = new List<int>();
                                    for (int idx = 0; idx < str.Length; idx++)
                                    {
                                        if (str[idx] == '-' && idx != 0 && str[idx - 1] != 'E')
                                            mid_mark.Add(idx);
                                    }
                                    mid_mark.Add(str.Length);
                                    int tmp_start = 0;
                                    foreach (int mark in mid_mark)
                                    {
                                        string tmp_str = str.Substring(tmp_start, mark - tmp_start);
                                        tmp_start = mark;
                                        val_list.Add(Double.Parse(tmp_str));
                
                                    }
                                }
                            }

                            LISFileItem NewFileItem = new LISFileItem(NodeNumber, val_list);
                            if (!ITEM_DICTIONARY.ContainsKey(NodeNumber))
                            {
                                ITEM_DICTIONARY.Add(NodeNumber, NewFileItem);
                            }
                            else
                            {
                                foreach(double val in val_list){
                                    ITEM_DICTIONARY[NodeNumber].VAL.Add(val);
                                }
                            }
                        }
                    }
                    tmpNode = sfstream.ReadLine();
                }
                return 1;
            }
            catch (FileNotFoundException e)
            {
                return -1;
            }
        }
        /*
        public int WrapperInstaller(int pid, string infostr)
        {
            param = new LISFileParams(pid, infostr);

            // Get filename by using function -- FileLocator
            string fname = FileLocator();
            // Import the file by using function -- ImportFile
            int fileret = ImportFile(fname);
            return fileret;
        }
        */
        public int FileIdxNumberGetter(int mid)
        {
            if (mid >= 2)
                return 2;
            else
                return 1;
        }
        public int FileIdxDataWidthGetter(int mid, int Idx)
        {
            int retWidth = 0;
            switch (mid)
            {
                case 2:
                    if (Idx == 0)
                        retWidth = 0;
                    else if (Idx == 1)
                        retWidth = 6;
                    break;
                default:
                    break;
            }
            return retWidth;
        }
        public int WrapperInstaller(int pid, int mid, int sid, string infostr)
        {
            param = new LISFileParams(pid, mid, sid);
            // Get filename by using function -- FileLocator
            int fileret = 1;
            for (int i = 0; i < FileIdxNumberGetter(mid); i++)
            {
                int fileIdx = i;
                string fname = FileLocator(fileIdx);
                // Import the file by using function -- ImportFile
                fileret = ImportFile(fname);
                if (fileret == -1)
                    return fileret;
            }
            return fileret;
        }
    }
}
