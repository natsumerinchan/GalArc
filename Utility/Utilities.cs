﻿// File: Utility/Utilities.cs
// Date: 2024/08/26
// Description: 一些常用的工具函数
//
// Copyright (C) 2024 detached64
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utility
{
    public class Utilities
    {
        /// <summary>
        /// Sort the file paths.Use string.CompareOrdinal() to avoid culture influence.
        /// </summary>
        /// <param name="pathString"></param>
        public static void InsertSort(string[] pathString)
        {
            for (int i = 1; i < pathString.Length; i++)
            {
                string insrtVal = pathString[i];
                int insertIndex = i - 1;

                while (insertIndex >= 0 && string.CompareOrdinal(insrtVal, pathString[insertIndex]) < 0)
                {
                    string temp;
                    temp = pathString[insertIndex + 1];
                    pathString[insertIndex + 1] = pathString[insertIndex];
                    pathString[insertIndex] = temp;
                    insertIndex--;
                }
                pathString[insertIndex + 1] = insrtVal;
            }
        }

        /// <summary>
        /// Only used for continuous file names with a separator in between like '0x00'.
        /// If file name length is fixed,use GetString.TrimEnd() instead.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="toThis"></param>
        /// <returns></returns>
        public static byte[] ReadUntil_Ansi(BinaryReader br, byte toThis)
        {
            List<byte> name = new List<byte>();
            byte aName;
            while ((aName = br.ReadByte()) != toThis)
            {
                name.Add(aName);
            }
            return name.ToArray();
        }

        /// <summary>
        /// Used for continuous file name with separators 0x0000 in between.
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static string ReadUntil_Unicode(BinaryReader br)
        {
            List<byte> name = new List<byte>();
            byte[] aname = new byte[2];
            byte[] fix = new byte[2];
            while ((aname = br.ReadBytes(2))[0] != 0x00 || aname[1] != 0x00)
            {
                name.Add(aname[0]);
                name.Add(aname[1]);
            }
            return Encoding.Unicode.GetString(name.ToArray());
        }

        public static string ReadCString(BinaryReader br, Encoding encoding)
        {
            return encoding.GetString(ReadUntil_Ansi(br, 0x00));
        }

        /// <summary>
        /// Get file count in specified folder and all subfolders.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static int GetFileCount_All(string folderPath)
        {
            string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            return allFiles.Length;
        }

        /// <summary>
        /// Get file count in specified folder only.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static int GetFileCount_TopOnly(string folderPath)
        {
            string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            return allFiles.Length;
        }

        /// <summary>
        /// Get all extensions among all files in specified folder and all subfolders.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string[] GetFileExtensions(string folderPath)
        {
            HashSet<string> uniqueExtension = new HashSet<string>();
            DirectoryInfo d = new DirectoryInfo(folderPath);
            foreach (FileInfo file in d.GetFiles())
            {
                uniqueExtension.Add(file.Extension.Replace(".", string.Empty));
            }
            string[] ext = new string[uniqueExtension.Count];
            uniqueExtension.CopyTo(ext);
            return ext;
        }

        /// <summary>
        /// Get file name length sum among all files in specified folder and all subfolders.
        /// </summary>
        /// <param name="fileSet"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static int GetNameLenSum(IEnumerable<string> fileSet, Encoding encoding)
        {
            int sum = 0;
            foreach (string s in fileSet)
            {
                sum += encoding.GetBytes(Path.GetFileName(s)).Length;
            }
            return sum;
        }

    }
}
