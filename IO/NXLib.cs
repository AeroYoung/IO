using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExpertLib.IO
{
    public static class NXStorage
    {
        public static void SetPreviewImage(this PictureBox pic, string filePath)
        {
            Graphics g = pic.CreateGraphics();
            Font font = new Font("宋体", 17);

            if (!File.Exists(filePath))
            {
                g.Clear(Color.FromArgb(192, 192, 255));
                g.DrawString(filePath + "文件不存在", font, new SolidBrush(Color.White), 80, 80);
                g.Dispose();
                return;
            }

            Storage root = StorageFile.OpenStorageFile(filePath);

            if (root == null)
            {
                g.Clear(Color.FromArgb(192, 192, 255));
                g.DrawString(filePath + "该文件暂无预览图 \n打开文件后重新保存一下即可", font, new SolidBrush(Color.White), 80, 80);
                g.Dispose();
                return;
            }

            int count = 0;

            StgElementInfo info = root.StorageInfo;
            
            List<StgElementInfo> co = root.GetChildElementsInfo();

            foreach (StgElementInfo stg in co)
            {
                if (stg.StgType != StgElementType.Storage) continue;
                if (!stg.Name.Equals("images")) continue;

                Storage images = root.OpenStorage(stg.Name);

                if (images == null) continue;

                List<StgElementInfo> subco = images.GetChildElementsInfo();
                foreach (StgElementInfo substg in subco)
                {
                    if (!substg.Name.Equals("preview")) continue;

                    StorageStream stream = images.OpenStream(substg.Name);

                    if (stream == null) continue;

                    pic.Image = stream.BytToImg(stream.StreamToBytes());

                    stream.Close();
                    images.Dispose();

                    count++;
                }
            }
            //root.Commit();
            root.Dispose();

            if (count == 0)
            {
                g.Clear(Color.FromArgb(192, 192, 255));
                g.DrawString("无预览图", font, new SolidBrush(Color.White), 80, 80);
                g.Dispose();
                return;
            }

            return;
        }

        public static void ExportJTFile(string filePath)
        {
            
            if (!File.Exists(filePath))
            {
                return;
            }

            Storage root = StorageFile.OpenStorageFile(filePath);

            if (root == null)
            {
                return;
            }

            int count = 0;

            StgElementInfo info = root.StorageInfo;

            List<StgElementInfo> co = root.GetChildElementsInfo();

            foreach (StgElementInfo stg in co)
            {
                if (stg.StgType != StgElementType.Storage) continue;
                if (!stg.Name.Equals("UG_PART")) continue;

                Storage ugPart = root.OpenStorage(stg.Name);

                if (ugPart == null) continue;

                List<StgElementInfo> subco = ugPart.GetChildElementsInfo();
                foreach (StgElementInfo substg in subco)
                {
                    if (!substg.Name.Equals("JT")) continue;

                    StorageStream stream = ugPart.OpenStream(substg.Name);

                    if (stream == null) continue;

                    FileStream fs = new FileStream(@"E:\Users\AeroYoung\Desktop\1.jt",FileMode.Create);
                    byte[] bs = stream.StreamToBytes();
                    fs.Write(bs, 0, bs.Length);
                    fs.Flush();
                    fs.Close();

                    stream.Close();
                    ugPart.Dispose();

                    count++;
                }
            }
            //root.Commit();
            root.Dispose();
            
            return;
        }

        public static void trans(string filePath)
        {            
            if (!File.Exists(filePath))
            {
                Console.Write("no file\n");
                return;
            }

            Storage root = StorageFile.OpenStorageFile(filePath);

            if (root == null)
            {
                Console.Write("null storage\n");
                return;
            }
            
            StgElementInfo info = root.StorageInfo;
            List<StgElementInfo> co = root.GetChildElementsInfo();
            string tab = "";
            foreach (StgElementInfo stg in co)
            {

                if (stg.StgType == StgElementType.Storage)
                {
                    Console.Write(tab + "Storage:" + stg.Name + "\n");
                    Storage stgg = root.OpenStorage(stg.Name);
                    if (stgg != null)
                    {
                        subTrans(stgg, tab + "--");
                        stgg.Dispose();
                    }
                }
                else if (stg.StgType == StgElementType.Stream)
                {
                    Console.Write(tab + "Stream:" + stg.Name + "\n");
                    StorageStream stream = root.OpenStream(stg.Name);
                    if(stream!=null)
                    {
                        Console.Write(tab + "——Stream:" + stream.StreamToBytes() + "\n");
                        stream.Close();
                    }                    
                }
                else
                {
                    Console.Write(tab + "else "+ stg.StgType.ToString()+":" + stg.Name + "\n");
                }
            }
            
            root.Dispose();            
            return;
        }

        public static void subTrans(Storage root,string tab)
        {
            if (root == null) return;
            StgElementInfo info = root.StorageInfo;
            List<StgElementInfo> co = root.GetChildElementsInfo();
            foreach (StgElementInfo stg in co)
            {

                if (stg.StgType == StgElementType.Storage)
                {
                    Console.Write(tab + "Storage:" + stg.Name + "\n");
                    Storage stgg = root.OpenStorage(stg.Name);
                    if(stgg!=null)
                    {
                        subTrans(stgg, tab + "--");
                        stgg.Dispose();
                    }
                    
                }
                else if (stg.StgType == StgElementType.Stream)
                {
                    Console.Write(tab + "Stream:" + stg.Name + "\n");
                    StorageStream stream = root.OpenStream(stg.Name);
                    if (stream != null)
                    {
                        Console.Write(tab + "——Stream:" + stream.StreamToBytes().ToString() + "\n");
                        stream.Close();
                    }
                }
                else
                {
                    Console.Write(tab + "else " + stg.StgType.ToString() + ":" + stg.Name + "\n");
                }
            }
        }
    }
}
