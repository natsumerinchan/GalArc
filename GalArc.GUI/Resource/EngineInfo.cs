﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GalArc.Resource
{
    internal class EngineInfo
    {
        internal string EngineName { get; set; }
        internal string UnpackFormat { get; set; }
        internal string PackFormat { get; set; }
        internal string PackVersion { get; set; }
        internal bool isUnpackEncodingEnabled { get; set; }
        internal bool isPackEncodingEnabled { get; set; }

        internal EngineInfo(string engineName, string unpackFormat, string packFormat)
        {
            EngineName = engineName;
            UnpackFormat = unpackFormat;
            PackFormat = packFormat;
            PackVersion = string.Empty;
            isUnpackEncodingEnabled = false;
            isPackEncodingEnabled = false;
        }
        internal EngineInfo(string engineName, string unpackFormat, string packFormat, string packVersion)
        {
            EngineName = engineName;
            UnpackFormat = unpackFormat;
            PackFormat = packFormat;
            PackVersion = packVersion;
            isUnpackEncodingEnabled = false;
            isPackEncodingEnabled = false;
        }
        internal EngineInfo(string engineName, string unpackFormat, string packFormat, bool isUnpackEncodingEnabled, bool isPackEncodingEnabled)
        {
            EngineName = engineName;
            UnpackFormat = unpackFormat;
            PackFormat = packFormat;
            PackVersion = string.Empty;
            this.isUnpackEncodingEnabled = isUnpackEncodingEnabled;
            this.isPackEncodingEnabled = isPackEncodingEnabled;
        }
        internal EngineInfo(string engineName, string unpackFormat, string packFormat, string packVersion, bool isUnpackEncodingEnabled, bool isPackEncodingEnabled)
        {
            EngineName = engineName;
            UnpackFormat = unpackFormat;
            PackFormat = packFormat;
            PackVersion = packVersion;
            this.isUnpackEncodingEnabled = isUnpackEncodingEnabled;
            this.isPackEncodingEnabled = isPackEncodingEnabled;
        }

    }
    internal class EngineInfos
    {
        public static List<EngineInfo> engineInfos = new List<EngineInfo>
        {
            new EngineInfo("AdvHD","ARC/PNA","ARC/PNA","1/2"),
            new EngineInfo("Ai5Win","VSD",string.Empty),
            new EngineInfo("Ai6Win","ARC",string.Empty),
            new EngineInfo("AmuseCraft","PAC","PAC"),
            new EngineInfo("Artemis","PFS","PFS","8/6/2",true,true),
            new EngineInfo("BiShop","BSA", "BSA","1/2"),
            new EngineInfo("EntisGLS","NOA","NOA",true,true),
            new EngineInfo("InnocentGrey","IGA/DAT","IGA/DAT"),
            new EngineInfo("KID","DAT","DAT"),
            new EngineInfo("Kirikiri","XP3","XP3","1/2"),
            new EngineInfo("Majiro","ARC","ARC","1/2"),
            new EngineInfo("NeXAS","PAC", string.Empty),
            new EngineInfo("NextonLikeC","LST",string.Empty),
            new EngineInfo("NitroPlus","PAK","PAK"),
            new EngineInfo("NScripter","NS2","NS2"),
            new EngineInfo("Palette","PAK","PAK"),
            new EngineInfo("RPGMaker","RGSSAD/RGSS2A/RGSS3A","RGSSAD/RGSS2A/RGSS3A","1"),
            new EngineInfo("Softpal","PAC","PAC"),
            new EngineInfo("SystemNNN","GPK/VPK","GPK/VPK","1/2"),
            new EngineInfo("Triangle","CG/SUD","CG/SUD")
        };
    }
}
