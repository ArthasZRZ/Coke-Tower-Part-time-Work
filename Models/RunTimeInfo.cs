using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfRibbonApplication1.Models
{
    public class RunTimeInfo
    {
        public string id { set; get; }
        public string info { set; get; }
        private int p1;
        private string p2;

        public RunTimeInfo() {}
       
        public RunTimeInfo(int p1, string p2)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}
