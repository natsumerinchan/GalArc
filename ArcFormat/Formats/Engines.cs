using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcFormat.Formats
{
    public class Engines
    {
        public class EngineInfo
        {
            public string EngineName { get; set; }
            public BindingList<string> Format { get; set; }
            public BindingList<string> PackFormat { get; set; }

            public EngineInfo(string engineName, BindingList<string> format, BindingList<string> packFormat)
            {
                EngineName = engineName;
                Format = format;
                PackFormat = packFormat;
            }
        }

        public static void EngineInit()
        {
            List<EngineInfo> engines = new List<EngineInfo>();
            engines.Add(new EngineInfo("AdvHD",new List<string> { ".arc", ".pna" }, new List<string> { ".arc",".pna"}));
        }
    }
}
