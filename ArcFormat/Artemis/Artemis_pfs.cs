using GalArc;
using Interface;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Utility;

namespace ArcFormat.Artemis
{
    internal class Artemis_pfs
    {
        struct Artemis_pfs_Header
        {
            public string Magic { get; set; }
            public string Version { get; set; }
            public uint IndexSize { get; set; }
            public uint FileCount { get; set; }
            public uint pathLenSum { get; set; }
        }
        struct Artemis_pfs_Entry
        {
            public string filePath { get; set; } //绝对路径，拼接而成
            public uint Size { get; set; }
            public uint Offset { get; set; }
            public int pathLen { get; set; }
            public string path { get; set; }
        }

        public static int pfs_unpack(string filePath)
        {
            //init
            Artemis_pfs_Header header = new()
            {
                Magic = "pf",
                FileCount = 0
            };

            //judge
            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new(fs);
            if (Encoding.ASCII.GetString(br.ReadBytes(2)) != header.Magic)
                return 1;
            header.Version = Encoding.ASCII.GetString(br.ReadBytes(1));
            //version?
            switch (header.Version)
            {
                case "8":
                    header.IndexSize = br.ReadUInt32();
                    main.Main.txtlog.AppendText("Valid pfs v8 archive detected." + Environment.NewLine);

                    //compute shakey

                    byte[] xorKey;
                    byte[] headerBytes = br.ReadBytes((int)header.IndexSize);
                    SHA1 sha = SHA1.Create();
                    xorKey = sha.ComputeHash(headerBytes);

                    fs.Seek(-header.IndexSize, SeekOrigin.Current);

                    header.FileCount = br.ReadUInt32();

                    long presPos8 = 0;

                    //process
                    DisplayFace.displayUn((int)header.FileCount, filePath);
                    for (int i = 0; i < header.FileCount; i++)
                    {
                        Artemis_pfs_Entry entry = new();
                        entry.pathLen = br.ReadInt32();
                        entry.filePath = filePath.Replace(".", "_") + "\\" + ArcEncoding.Encodings().GetString(br.ReadBytes(entry.pathLen));
                        br.ReadUInt32(); // skip 4 unused bytes:0x00000000
                        entry.Offset = br.ReadUInt32();
                        entry.Size = br.ReadUInt32();

                        Directory.CreateDirectory(Path.GetDirectoryName(entry.filePath));

                        using (FileStream fo = new(entry.filePath, FileMode.Create, FileAccess.Write))
                        {
                            presPos8 = fs.Position;
                            fs.Seek(entry.Offset, SeekOrigin.Begin);

                            for (int j = 0; j < entry.Size + 1; j += xorKey.Length)
                            {
                                int toRead = (int)Math.Min(entry.Size - j, xorKey.Length);
                                byte[] buffer = br.ReadBytes(toRead);
                                fo.Write(Xor.BytesXorBytes(buffer, xorKey), 0, toRead);
                            }
                            fs.Seek(presPos8, SeekOrigin.Begin);
                        }
                        main.Main.bar.PerformStep();
                    }
                    fs.Close();
                    br.Close();
                    return 0;

                case "2":
                    header.IndexSize = br.ReadUInt32();
                    main.Main.txtlog.AppendText("Valid pfs v2 archive detected." + Environment.NewLine);
                    br.ReadUInt32();//reserve 0x00000000
                    header.FileCount = br.ReadUInt32();
                    long presPos2 = 0;
                    DisplayFace.displayUn((int)header.FileCount, filePath);

                    for (int i = 0; i < header.FileCount; i++)
                    {
                        Artemis_pfs_Entry entry = new();
                        entry.pathLen = (int)br.ReadUInt32();
                        entry.filePath = filePath.Replace(".", "_") + "/" + ArcEncoding.Encodings().GetString(br.ReadBytes(entry.pathLen)).Replace("\\", "/");
                        br.ReadUInt32();//0x10000000
                        br.ReadUInt32();//0x00000000
                        br.ReadUInt32();//0x00000000
                        entry.Offset = br.ReadUInt32();
                        entry.Size = br.ReadUInt32();

                        Directory.CreateDirectory(Path.GetDirectoryName(entry.filePath));

                        presPos2 = fs.Position;
                        fs.Seek(entry.Offset, SeekOrigin.Begin);
                        byte[] buffer = br.ReadBytes((int)entry.Size);
                        File.WriteAllBytes(entry.filePath, buffer);
                        fs.Seek(presPos2, SeekOrigin.Begin);

                        main.Main.bar.PerformStep();
                    }
                    fs.Close();
                    br.Close();
                    return 0;

                case "6":
                    header.IndexSize = br.ReadUInt32();
                    main.Main.txtlog.AppendText("Valid pfs v6 archive detected." + Environment.NewLine);
                    header.FileCount = br.ReadUInt32();
                    long presPos6 = 0;//position at present
                    DisplayFace.displayUn((int)header.FileCount, filePath);

                    //process
                    for (int i = 0; i < header.FileCount; i++)
                    {
                        Artemis_pfs_Entry entry = new();
                        entry.pathLen = br.ReadInt32();
                        entry.filePath = filePath.Replace(".", "_") + "/" + ArcEncoding.Encodings().GetString(br.ReadBytes(entry.pathLen)).Replace("\\", "/");
                        br.ReadUInt32(); // skip 4 unused bytes:0x00000000
                        entry.Offset = br.ReadUInt32();
                        entry.Size = br.ReadUInt32();

                        Directory.CreateDirectory(Path.GetDirectoryName(entry.filePath));

                        presPos6 = fs.Position;
                        fs.Seek(entry.Offset, SeekOrigin.Begin);
                        byte[] buffer = br.ReadBytes((int)entry.Size);
                        File.WriteAllBytes(entry.filePath, buffer);
                        fs.Seek(presPos6, SeekOrigin.Begin);

                        main.Main.bar.PerformStep();
                    }
                    fs.Close();
                    br.Close();
                    return 0;

                default:
                    main.Main.bar.Value = 0;
                    main.Main.txtlog.AppendText("pfs v" + header.Version + " archive temporarily not supported." + Environment.NewLine);
                    return 2;
            }
        }
        public static int pfs_pack(string folderPath)
        {
            //init
            Artemis_pfs_Header header = new()
            {
                Magic = "pf",
                Version = main.Main.selVersion.Text,
                pathLenSum = 0
            };
            List<Artemis_pfs_Entry> index = new();

            //get the amount of files
            header.FileCount = (uint)Util.GetFileCount_All(folderPath);
            DisplayFace.displayPack((int)header.FileCount);

            //new pathString array
            string[] pathString = new string[header.FileCount];

            //get path len and restore file info to pathString
            DirectoryInfo d = new(folderPath);
            int i = 0;
            foreach (FileInfo file in d.GetFiles("*.*", SearchOption.AllDirectories))
            {
                pathString[i] = file.FullName.Replace(folderPath + "\\", string.Empty);
                i++;
            }
            Util.InsertSort(pathString);

            //add entry
            for (int j = 0; j < header.FileCount; j++)
            {
                Artemis_pfs_Entry artemisEntry = new()
                {
                    Size = (uint)new FileInfo(folderPath + "\\" + pathString[j]).Length,
                    path = pathString[j],
                    filePath = folderPath + "\\" + pathString[j],
                    pathLen = ArcEncoding.Encodings().GetByteCount(pathString[j])
                };

                index.Add(artemisEntry);
                header.pathLenSum += (uint)artemisEntry.pathLen;
            }

            switch (main.Main.selVersion.Text)
            {
                case "8":
                    //compute indexsize
                    header.IndexSize = 4 + 16 * header.FileCount + header.pathLenSum + 4 + 8 * header.FileCount + 12;
                    //indexsize=(filecount)4byte+(pathlen+0x00000000+offset to begin+file size)16byte*filecount+pathlensum+(file count+1)4byte+8*filecount+(0x00000000)4byte*2+(offsettablebegin-0x7)4byte

                    //write header
                    MemoryStream ms8 = new((int)(header.IndexSize + Marshal.SizeOf<Artemis_pfs_Header>()));
                    BinaryWriter writer8 = new(ms8);
                    writer8.Write(Encoding.ASCII.GetBytes(header.Magic));
                    writer8.Write(Encoding.ASCII.GetBytes(header.Version));
                    writer8.Write(header.IndexSize);
                    writer8.Write(header.FileCount);

                    //write entry
                    long posIndexStart = ms8.Position - sizeof(uint);//0x7
                    uint offset8 = header.IndexSize + 7;

                    foreach (var file in index)
                    {
                        writer8.Write(file.pathLen);
                        writer8.Write(ArcEncoding.Encodings().GetBytes(file.path));
                        writer8.Write(0); // reserved
                        writer8.Write(offset8);
                        writer8.Write(file.Size);
                        offset8 += file.Size;
                    }

                    long posOffsetTable = ms8.Position;
                    uint offsetCount = header.FileCount + 1;
                    writer8.Write(offsetCount);//filecount + 1
                    uint total = 4;

                    //write table
                    foreach (var file in index)
                    {
                        total = total + 4 + (uint)file.pathLen;
                        uint posOffset = total;
                        writer8.Write(posOffset);
                        writer8.Write(0); // reserved
                        total += 12;
                    }
                    writer8.Write(0); // EOF of offset table
                    writer8.Write(0); // EOF of offset table
                    uint tablePos = (uint)(posOffsetTable - 7);
                    writer8.Write(tablePos);

                    //write data
                    byte[] xorKey = new byte[20];
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        byte[] xorBuf = ms8.ToArray().Skip((int)posIndexStart).Take((int)header.IndexSize).ToArray();
                        xorKey = sha1.ComputeHash(xorBuf);
                    }
                    if (Path.GetFileName(folderPath).Contains("_pfs"))
                    {
                        FileStream arc = new(Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath).Replace("_", ".") + ".new", FileMode.Create, FileAccess.Write);
                        ms8.WriteTo(arc);
                        foreach (var file in index)
                        {
                            byte[] fileData = File.ReadAllBytes(file.filePath);
                            arc.Write(Xor.BytesXorBytes(fileData, xorKey), 0, fileData.Length);
                            main.Main.bar.PerformStep();
                        }
                        arc.Close();
                    }
                    else
                    {
                        FileStream arc = new(Path.Combine(Path.GetDirectoryName(folderPath) + "\\root.pfs"), FileMode.Create, FileAccess.Write);
                        ms8.WriteTo(arc);
                        foreach (var file in index)
                        {
                            byte[] fileData = File.ReadAllBytes(file.filePath);

                            arc.Write(Xor.BytesXorBytes(fileData, xorKey), 0, fileData.Length);

                            main.Main.bar.PerformStep();
                        }
                        arc.Close();
                    }
                    ms8.Close();
                    writer8.Close();


                    return 0;

                case "2":
                    header.IndexSize = 8 + 24 * header.FileCount + header.pathLenSum;

                    //write header
                    main.Main.txtlog.AppendText("Packing header……" + Environment.NewLine);
                    MemoryStream ms2 = new();
                    BinaryWriter writer2 = new(ms2);
                    writer2.Write(Encoding.ASCII.GetBytes(header.Magic));
                    writer2.Write(Encoding.ASCII.GetBytes(header.Version));
                    writer2.Write(header.IndexSize);
                    writer2.Write((uint)0);
                    writer2.Write(header.FileCount);
                    uint offset2 = header.IndexSize + 7;

                    //write entry
                    foreach (var file in index)
                    {
                        writer2.Write((uint)file.pathLen);
                        writer2.Write(ArcEncoding.Encodings().GetBytes(file.path));
                        writer2.Write((uint)16);
                        writer2.Write((uint)0);
                        writer2.Write((uint)0);
                        writer2.Write(offset2);
                        writer2.Write(file.Size);
                        offset2 += file.Size;
                    }

                    //write data
                    if (Path.GetFileName(folderPath).Contains("_pfs"))
                    {
                        FileStream arc = new(Path.Combine(Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath).Replace("_", ".") + ".new"), FileMode.Create, FileAccess.Write);
                        ms2.WriteTo(arc);

                        foreach (var file in index)
                        {
                            byte[] fileData = File.ReadAllBytes(file.filePath);
                            arc.Write(fileData, 0, fileData.Length);

                            main.Main.bar.PerformStep();
                        }
                        arc.Close();
                    }
                    else
                    {
                        FileStream arc = new(Path.Combine(Path.GetDirectoryName(folderPath) + "\\root.pfs"), FileMode.Create, FileAccess.Write);
                        ms2.WriteTo(arc);
                        foreach (var file in index)
                        {
                            byte[] fileData = File.ReadAllBytes(file.filePath);
                            arc.Write(fileData, 0, fileData.Length);

                            main.Main.bar.PerformStep();
                        }
                        arc.Close();
                    }

                    return 0;

                case "6":
                    //compute indexsize
                    header.IndexSize = 4 + 16 * header.FileCount + header.pathLenSum + 4 + 8 * header.FileCount + 12;
                    //indexsize=(filecount)4byte+(pathlen+0x00000000+offset to begin+file size)16byte*filecount+pathlensum+(file count+1)4byte+8*filecount+(0x00000000)4byte*2+(offsettablebegin-0x7)4byte

                    //write header
                    //main.Main.txtlog.AppendText("Packing header……" + Environment.NewLine);

                    MemoryStream ms6 = new((int)(header.IndexSize + Marshal.SizeOf<Artemis_pfs_Header>()));
                    BinaryWriter writer6 = new(ms6);
                    writer6.Write(Encoding.ASCII.GetBytes(header.Magic));
                    writer6.Write(Encoding.ASCII.GetBytes(header.Version));
                    writer6.Write(header.IndexSize);
                    writer6.Write(header.FileCount);

                    //write entry
                    //main.Main.txtlog.AppendText("Packing entry……" + Environment.NewLine);

                    uint offset6 = header.IndexSize + 7;

                    foreach (var file in index)
                    {
                        uint filenameSize = (uint)file.pathLen;//use utf-8 for japanese character in file name
                        writer6.Write(filenameSize);
                        writer6.Write(ArcEncoding.Encodings().GetBytes(file.path));
                        writer6.Write(0); // reserved
                        writer6.Write(offset6);
                        writer6.Write(file.Size);
                        offset6 += file.Size;
                    }

                    long posOffsetTable6 = ms6.Position;
                    uint offsetCount6 = header.FileCount + 1;
                    writer6.Write(offsetCount6);//filecount + 1
                    uint total6 = 4;

                    //write table
                    //main.Main.txtlog.AppendText("Packing table……" + Environment.NewLine);

                    foreach (var file in index)
                    {
                        total6 = total6 + 4 + (uint)file.pathLen;//use utf-8 for japanese character in file name
                        uint posOffset = total6;
                        writer6.Write(posOffset);
                        writer6.Write(0); // reserved
                        total6 += 12;
                    }
                    writer6.Write(0); // EOF of offset table
                    writer6.Write(0); // EOF of offset table
                    uint tablePos6 = (uint)(posOffsetTable6 - 7);
                    writer6.Write(tablePos6);

                    //write data
                    //main.Main.txtlog.AppendText("Packing data……" + Environment.NewLine);

                    if (Path.GetFileName(folderPath).Contains("_pfs"))
                    {
                        FileStream arc = new(Path.Combine(Path.GetDirectoryName(folderPath) + "\\" + Path.GetFileName(folderPath).Replace("_", ".") + ".new"), FileMode.Create, FileAccess.Write);
                        ms6.WriteTo(arc);
                        foreach (var file in index)
                        {
                            byte[] fileData = File.ReadAllBytes(file.filePath);
                            arc.Write(fileData, 0, fileData.Length);

                            main.Main.bar.PerformStep();
                        }
                        arc.Close();
                    }
                    else
                    {
                        FileStream arc = new(Path.Combine(Path.GetDirectoryName(folderPath) + "\\root.pfs"), FileMode.Create, FileAccess.Write);
                        ms6.WriteTo(arc);
                        foreach (var file in index)
                        {
                            byte[] fileData = File.ReadAllBytes(file.filePath);
                            arc.Write(fileData, 0, fileData.Length);

                            main.Main.bar.PerformStep();
                        }
                        arc.Close();
                    }
                    ms6.Close();
                    writer6.Close();


                    return 0;
            }
            return 1;
        }

    }

}


