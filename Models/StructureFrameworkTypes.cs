using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WpfRibbonApplication1
{
    public class StructureFrameworkTypesBase
    {
        public string SFName { set; get; }
        public int SFNumber { set; get; }
    }
    public class StructureFrameWorkTypes : ObservableCollection<StructureFrameworkTypesBase>
    {
        public StructureFrameWorkTypes()
        {
            this.Add(new StructureFrameworkTypesBase{ SFName = "空模型", SFNumber = 0 });
        }
    }
}
