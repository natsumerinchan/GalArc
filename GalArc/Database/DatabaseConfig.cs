﻿using System;

namespace GalArc.Database
{
    public static class DatabaseConfig
    {
        public static bool IsEnabled { get; set; } = true;

        public static string Path
        {
            get
            {
                if (string.IsNullOrEmpty(BaseSettings.Default.DatabasePath))
                {
                    BaseSettings.Default.DatabasePath = DefaultPath;
                    BaseSettings.Default.Save();
                    return DefaultPath;
                }
                return BaseSettings.Default.DatabasePath;
            }
            set
            {
                BaseSettings.Default.DatabasePath = value;
                BaseSettings.Default.Save();
            }
        }

        private static string DefaultPath => System.IO.Path.Combine(Environment.CurrentDirectory, "Database\\");
    }

    public interface Scheme
    {
        int Version { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DatabaseSchemeAttribute : Attribute
    {
    }
}
