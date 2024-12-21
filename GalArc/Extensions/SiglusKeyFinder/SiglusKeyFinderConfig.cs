﻿using System;

namespace GalArc.Extensions.SiglusKeyFinder
{
    [Extension]
    public class SiglusKeyFinderConfig : IExtension
    {
        public static string Path
        {
            get
            {
                if (string.IsNullOrEmpty(BaseSettings.Default.SiglusKeyFinderPath))
                {
                    BaseSettings.Default.SiglusKeyFinderPath = DefaultPath;
                    BaseSettings.Default.Save();
                    return DefaultPath;
                }
                return BaseSettings.Default.SiglusKeyFinderPath;
            }
            set
            {
                BaseSettings.Default.SiglusKeyFinderPath = value;
                BaseSettings.Default.Save();
            }
        }

        private static string DefaultPath => System.IO.Path.Combine(Environment.CurrentDirectory, "Extensions\\SiglusKeyFinder.exe");

        public string Description => "Extract xor key from SiglusEngine games.";
        public string OriginalAuthor => "yanhua0518";
        public string OriginalWebsite => "https://github.com/yanhua0518/GALgameScriptTools/tree/master/SiglusEngine";
        public string ExtensionWebsite => "https://github.com/detached64/SiglusKeyFinder";
    }
}
