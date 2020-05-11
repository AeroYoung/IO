//根据不同编码的特点和标志,对一个文本文件判断编码方法如下
//开头字节   Charset/encoding   
// EF   BB   BF   UTF-8   
// FE   FF   UTF-16/UCS-2,    big   endian   
// FF   FE   UTF-16/UCS-2,   little   endian  
// FF   FE   00   00   UTF-32/UCS-4,   little   endian.   
// 00   00   FE   FF   UTF-32/UCS-4,   big-endian.
// UTF7  所有字节的内容不会大于127,也就是不大于&HFF
using System.IO;
using System.Text;

namespace ExpertLib.IO
{

    public static class TxtFileEncoding
    {
        /// <summary>
        /// 取文件编码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Encoding GetEncodingType(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Encoding r = GetEncodingType(fs);
                fs.Close();
                return r;
            }                
        }

        /// <summary>
        /// 取文件编码
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static Encoding GetEncodingType(FileStream fs)
        {  
            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            r.Close();

            if (ss[0] >= 0xEF)
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                
                // FF   FE   00   00   UTF-32/UCS-4,   little   endian.   
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x00 && ss[3] == 0x00)
                {
                    return System.Text.Encoding.UTF32;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                // 00   00   FE   FF   UTF-32/UCS-4,   big-endian.
                if (ss[0] == 0x00 && ss[1] == 0x00 && ss[2] == 0xFE && ss[3] == 0xFF)
                {
                    // 本应是UTF-32 big-endian 但CLR 不支持，故返回默认ASCII
                    return Encoding.Default;
                }
                else
                {
                    return Encoding.Default;
                }
            }
        }
        

    }
}
