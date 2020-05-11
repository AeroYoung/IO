using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ExpertLib.IO
{
    /// <summary>
    /// 文件读写操作
    /// </summary>
    public static class FileCtr
    {
        public static byte[] ImageToBytes(this Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public static byte[] StreamToBytes(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static Stream BytesToStream(this byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public static List<T> ReadSerializable<T>(string filePath)
        {
            List<T> result = new List<T>();

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            if (fs.Length > 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                result = bf.Deserialize(fs) as List<T>;
            }
            fs.Close();

            return result;
        }

        public static Dictionary<T, V> ReadSerializable<T, V>(string filePath)
        {
            Dictionary<T, V> result = new Dictionary<T, V>();

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    if (fs.Length > 0)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        result = bf.Deserialize(fs) as Dictionary<T, V>;
                    }
                    fs.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return result;
            }        
        }

        public static void WriteSerializable<T, V>(this Dictionary<T, V> obj, string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public static void WriteSerializable<T>(this List<T> obj, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }

        /// <summary>
        /// 获得一个唯一的文件名(有重复则在文件名前面加上序号)
        /// </summary>
        /// <param name="dir">所在目录</param>
        /// <param name="name">文件名</param>
        public static FileInfo UniqueNewFile(this FileInfo info)
        {
            string dir = info.DirectoryName;
            string name = info.Name;
            FileInfo result = new FileInfo(dir + "\\" + name);
            int i = 1;
            while (File.Exists(result.FullName))
            {
                result = new FileInfo(dir + "\\(" + (i++).ToString() + ")" + name);
            }
            return result;
        }

        public static string LegalFileName(this string value)
        {
            string result = value.Replace("/", "").Replace("\\", "")
                .Replace("\"", "").Replace("<", "").Replace(">", "")
                .Replace("?", "").Replace("*", "").Replace(":","")
                .Replace(".", "").Replace("|", "");

            if (result.Trim().Length == 0)
                result += "新建文件夹";

            return result;
        }

    }
}
