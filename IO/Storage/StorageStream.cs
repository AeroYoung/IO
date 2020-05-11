using ExpertLib.Dialogs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using ExpertLib;
using System.Drawing;

namespace ExpertLib.IO
{
    /// <summary>
    /// 存储流类
    /// </summary>
    public sealed class StorageStream : System.IO.Stream,System.IDisposable
    {
        #region Instace Field
        private bool disposed;
        private IStream stream;
        #endregion

        #region Construcor
        internal StorageStream(IStream stream)
        {
            this.stream = stream;
        }

        ~StorageStream()
        {
            this.Dispose();
        }

        public new void Dispose()
        {
            if (!this.disposed)
            {
                Marshal.ReleaseComObject(this.stream);
                this.stream = null;

                this.disposed = true;
            }

            GC.SuppressFinalize(this);
        }
        #endregion

        #region 转换

        public Image ToImage()
        {
            try
            {
                MemoryStream ms = new MemoryStream(StreamToBytes());
                Image img = Image.FromStream(ms);
                return img;
            }
            catch (Exception ex)
            {
                Log.e($"ToImage {ex.Message}");
                return null;
            }
        }

        public byte[] StreamToBytes()
        {
            byte[] bytes = new byte[this.Length];
            this.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            this.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public Image BytToImg(byte[] byt)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byt);
                Image img = Image.FromStream(ms);
                return img;
            }
            catch (Exception ex)
            {
                Log.e($"BytToImg {ex.Message}");
                return null;
            }
        }

        #endregion
        
        /// <summary>
        /// Convenience method for writing Strings to the stream
        /// </summary>
        /// <param name="s"></param>
        public void Write(string s)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] pv = encoding.GetBytes(s);

            Write(pv, 0, pv.GetLength(0));
        }

        #region orverride

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            this.stream.Commit(0);
        }

        public override long Length
        {
            get
            {
                if (this.stream == null)
                    throw new ObjectDisposedException("Invalid stream object.");

                ComTypes.STATSTG statstg;

                this.stream.Stat(out statstg, 1 /**//* STATSFLAG_NONAME*/ );

                return statstg.cbSize;
            }
        }

        public override long Position
        {
            get { return Seek(0, SeekOrigin.Current); }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (stream == null)
                throw new ObjectDisposedException("Invalid stream object.");

            if (offset != 0)
            {
                throw new NotSupportedException("Only 0 offset is supported");
            }

            int bytesRead;

            unsafe
            {
                IntPtr address = new IntPtr(&bytesRead);

                stream.Read(buffer, count, address);
            }

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (stream == null)
                throw new ObjectDisposedException("Invalid stream object.");

            long position = 0;

            unsafe
            {
            IntPtr address = new IntPtr(&position);
            stream.Seek(offset, (int)origin, address);
        }

            return position;
        }

        public override void SetLength(long value)
        {
            if (stream == null)
                throw new ObjectDisposedException("Invalid stream object.");

            stream.SetSize(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (stream == null)
                throw new ObjectDisposedException("Invalid stream object.");

            if (offset != 0)
            {
                throw new NotSupportedException("Only 0 offset is supported");
            }

            stream.Write(buffer, count, IntPtr.Zero);
            stream.Commit(0);
        }
        
        public override void Close()
        {
            if (this.stream != null)
            {
                stream.Commit(0);

                this.Dispose();
            }
        }

        #endregion
    }
}
