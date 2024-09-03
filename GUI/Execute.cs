using GalArc;
namespace Interface
{
    class Execute
    {
        public static void ExecuteUnpack(string SelectedText)
        {
            if (!main.Main.showFormat.Items.Contains(Path.GetExtension(main.Main.filePath.Text)))
            {
                main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                return;
            }
            switch (SelectedText)
            {
                case "AdvHD":
                    if (Path.GetExtension(main.Main.filePath.Text) == ".arc")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.AdvHD.AdvHD_arc.arc_ver(main.Main.filePath.Text));
                        return;
                    }
                    else if (Path.GetExtension(main.Main.filePath.Text) == ".pna")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.AdvHD.AdvHD_pna.pna_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else
                    {
                        main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                        return;
                    }
                case "Ai5Win":
                    LogUnpackInfo.Unpack(ArcFormat.Ai5Win.Ai5Win_VSD.vsd_unpack(main.Main.filePath.Text));
                    return;
                case "Silky":
                    LogUnpackInfo.Unpack(ArcFormat.Silky.Silky_arc.arc_unpack(main.Main.filePath.Text));
                    return;
                case "AmuseCraft":
                    LogUnpackInfo.Unpack(ArcFormat.AmuseCraft.AmuseCraft_pac.pac_unpack(main.Main.filePath.Text));
                    return;
                case "Artemis":
                    LogUnpackInfo.Unpack(ArcFormat.Artemis.Artemis_pfs.pfs_unpack(main.Main.filePath.Text));
                    return;
                case "EntisGLS":
                    LogUnpackInfo.Unpack(ArcFormat.EntisGLS.EntisGLS_noa.noa_unpack(main.Main.filePath.Text));
                    return;
                case "InnocentGrey":
                    if (Path.GetExtension(main.Main.filePath.Text) == ".iga")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.InnocentGrey.InnocentGrey_iga.InnocentGrey_iga_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else if (Path.GetExtension(main.Main.filePath.Text) == ".dat")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.InnocentGrey.InnocentGrey_dat.dat_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else
                    {
                        main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                        return;
                    }
                case "NextonLikeC":
                    LogUnpackInfo.Unpack(ArcFormat.NextonLikeC.NextonLikeC_lst.NextonLikeC_lst_unpack(main.Main.filePath.Text), "lst", "no extension");
                    return;
                case "NScripter":
                    if (Path.GetExtension(main.Main.filePath.Text) == ".ns2")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.NScripter.NScripter_ns2.ns2_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else if(Path.GetExtension(main.Main.filePath.Text) == ".dat")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.NScripter.Nscripter_dat.dat_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else
                    {
                        main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                        return;
                    }
                case "SystemNNN":
                    if (Path.GetExtension(main.Main.filePath.Text) == ".gpk" || Path.GetExtension(main.Main.filePath.Text) == ".gtb")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.SystemNNN.SystemNNN_gpk.gpk_unpack(main.Main.filePath.Text), "gpk", "gtb");
                        return;
                    }
                    else if (Path.GetExtension(main.Main.filePath.Text) == ".vpk" || Path.GetExtension(main.Main.filePath.Text) == ".vtb")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.SystemNNN.SystemNNN_vpk.vpk_unpack(main.Main.filePath.Text), "vpk", "vtb");
                        return;
                    }
                    else
                    {
                        main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                        return;
                    }
                case "Triangle":
                    if (Path.GetExtension(main.Main.filePath.Text) == ".SUD" || Path.GetExtension(main.Main.filePath.Text) == ".sud")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.Triangle.Triangle_SUD.SUD_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else if (Path.GetExtension(main.Main.filePath.Text) == ".CG" || Path.GetExtension(main.Main.filePath.Text) == ".cg")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.Triangle.Triangle_CG.CG_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else
                    {
                        main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                        return;
                    }
                //case "KaGuYa":
                //    LogUnpackInfo.Unpack(ArcFormat.KaGuYa.KaGuYa_arc.arc_unpack(main.Main.filePath.Text));
                //    return;
                case "Nitro+":
                    LogUnpackInfo.Unpack(ArcFormat.NitroPlus.NitroPlus_pak.pak_unpack(main.Main.filePath.Text));
                    return;
                case "Softpal":
                    LogUnpackInfo.Unpack(ArcFormat.Softpal.Softpal_pac.pac_unpack(main.Main.filePath.Text));
                    return;
                case "KID":
                    if (Path.GetExtension(main.Main.filePath.Text) == ".dat")
                    {
                        LogUnpackInfo.Unpack(ArcFormat.KID.KID_dat.dat_unpack(main.Main.filePath.Text));
                        return;
                    }
                    else
                    {
                        main.Main.txtlog.AppendText("Unpack failed:Not a supported file." + Environment.NewLine + Environment.NewLine);
                        return;
                    }
                case "Kirikiri":
                    LogUnpackInfo.Unpack(ArcFormat.Kirikiri.KiriKiri_xp3.xp3_unpack(main.Main.filePath.Text));
                    return;
            }
        }
        //extension judgement:
        //use main.Main.showFormat.Items.Contains to judge extension 
        public static void ExecutePack(string SelectedText)
        {
            switch (SelectedText)
            {
                case "AdvHD":
                    if (main.Main.selPackFormat.SelectedIndex == 0)
                    {
                        int isAdvHDArcPack;
                        if (main.Main.selVersion.Text == "1")
                        {
                            isAdvHDArcPack = ArcFormat.AdvHD.AdvHD_arc.arc_v1_pack(main.Main.folderPath.Text);
                        }
                        else
                        {
                            isAdvHDArcPack = ArcFormat.AdvHD.AdvHD_arc.arc_v2_pack(main.Main.folderPath.Text);
                        }
                        LogPackInfo.Pack(isAdvHDArcPack);
                        return;
                    }
                    else
                    {
                        int isPnaPack = ArcFormat.AdvHD.AdvHD_pna.pna_pack(main.Main.folderPath.Text);
                        switch (isPnaPack)
                        {
                            case 0:
                                main.Main.txtlog.AppendText("Pack finished." + Environment.NewLine + Environment.NewLine);
                                return;
                            case 1:
                                main.Main.txtlog.AppendText("Pack failed:please make sure original pna file exists in the parent dir of " + main.Main.folderPath.Text + "." + Environment.NewLine + Environment.NewLine);
                                return;
                            default:
                                main.Main.txtlog.AppendText("Pack failed." + Environment.NewLine + Environment.NewLine);
                                return;
                        }
                    }
                case "Artemis":
                    LogPackInfo.Pack(ArcFormat.Artemis.Artemis_pfs.pfs_pack(main.Main.folderPath.Text));
                    return;
                case "EntisGLS":
                    int isNoaPack = ArcFormat.EntisGLS.EntisGLS_noa.noa_pack(main.Main.folderPath.Text);
                    switch (isNoaPack)
                    {
                        case 0:
                            main.Main.txtlog.AppendText("Pack finished." + Environment.NewLine + Environment.NewLine);
                            return;
                        case 1:
                            main.Main.txtlog.AppendText("Pack failed:No valid TimestampInfo.json found.You should use this tool to unpack first." + Environment.NewLine + Environment.NewLine);
                            return;
                        default:
                            main.Main.txtlog.AppendText("Pack failed." + Environment.NewLine + Environment.NewLine);
                            return;

                    }
                case "InnocentGrey":
                    if (main.Main.selPackFormat.Text == ".iga")
                    {
                        LogPackInfo.Pack(ArcFormat.InnocentGrey.InnocentGrey_iga.InnocentGrey_iga_pack(main.Main.folderPath.Text));
                        return;
                    }
                    else
                    {
                        LogPackInfo.Pack(ArcFormat.InnocentGrey.InnocentGrey_dat.dat_pack(main.Main.folderPath.Text));
                        return;
                    }
                case "NScripter":
                    if (main.Main.selPackFormat.SelectedIndex == 0)
                    {
                        LogPackInfo.Pack(ArcFormat.NScripter.NScripter_ns2.ns2_pack(main.Main.folderPath.Text));
                        return;
                    }
                    else
                    {
                        LogPackInfo.Pack(ArcFormat.NScripter.Nscripter_dat.dat_pack(main.Main.folderPath.Text));
                        return;
                    }
                case "SystemNNN":
                    if (main.Main.selPackFormat.Text == ".vpk")
                    {
                        int isVpkPack = ArcFormat.SystemNNN.SystemNNN_vpk.vpk_pack(main.Main.folderPath.Text);
                        switch (isVpkPack)
                        {
                            case 0:
                                main.Main.txtlog.AppendText("Pack finished." + Environment.NewLine + Environment.NewLine);
                                return;
                            case 1:
                                main.Main.txtlog.AppendText("Pack failed:please make sure all the files in " + main.Main.folderPath.Text + " have extension .vaw." + Environment.NewLine + Environment.NewLine);
                                return;
                            default:
                                main.Main.txtlog.AppendText("Pack failed." + Environment.NewLine + Environment.NewLine);
                                return;

                        }

                    }

                    else
                    {
                        int isGpkPack = ArcFormat.SystemNNN.SystemNNN_gpk.gpk_pack(main.Main.folderPath.Text);
                        switch (isGpkPack)
                        {
                            case 0:
                                main.Main.txtlog.AppendText("Pack finished." + Environment.NewLine + Environment.NewLine);
                                return;
                            case 1:
                                main.Main.txtlog.AppendText("Pack failed:please make sure all the files in " + main.Main.folderPath.Text + " have extension .dwq." + Environment.NewLine + Environment.NewLine);
                                return;
                            default:
                                return;

                        }

                    }
                case "Triangle":
                    if (main.Main.selPackFormat.Text == ".SUD")
                    {
                        LogPackInfo.Pack(ArcFormat.Triangle.Triangle_SUD.SUD_pack(main.Main.folderPath.Text));
                        return;
                    }
                    else
                    {
                        LogPackInfo.Pack(ArcFormat.Triangle.Triangle_CG.CG_pack(main.Main.folderPath.Text));
                        return;
                    }
                case "AmuseCraft":
                    LogPackInfo.Pack(ArcFormat.AmuseCraft.AmuseCraft_pac.pac_pack(main.Main.folderPath.Text));
                    return;
                case "Silky":
                    LogPackInfo.Pack(ArcFormat.Silky.Silky_arc.arc_pack(main.Main.folderPath.Text));
                    return;
                case "Nitro+":
                    LogPackInfo.Pack(ArcFormat.NitroPlus.NitroPlus_pak.pak_pack(main.Main.folderPath.Text));
                    return;
                case "Softpal":
                    LogPackInfo.Pack(ArcFormat.Softpal.Softpal_pac.pac_pack(main.Main.folderPath.Text));
                    return;
                case "KID":
                    LogPackInfo.Pack(ArcFormat.KID.KID_dat.dat_pack(main.Main.folderPath.Text));
                    return;
                case "Kirikiri":
                    LogPackInfo.Pack(ArcFormat.Kirikiri.KiriKiri_xp3.xp3_pack(main.Main.folderPath.Text));
                    return;
            }

        }

    }
}
