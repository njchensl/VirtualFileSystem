using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace VirtualFS
{
    public unsafe class VDisk
    {
        private readonly byte* m_Data;
        private readonly ulong m_Size;

        public RefUInt Head { get; set; }

        public VDisk(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            m_Data = (byte*)Marshal.AllocHGlobal(data.Length);
            m_Size = (ulong)data.Length;
            fixed (byte* src = data)
            {
                memcpy(m_Data, src, (ulong)data.Length);
            }

            data = null;
            GC.Collect();
        }

        ~VDisk()
        {
            Marshal.FreeHGlobal((IntPtr)m_Data);
        }

        public void Write()
        {
            FileStream fs = File.OpenWrite("disk.vd");
            for (ulong i = 0; i < m_Size; i++)
            {
                fs.WriteByte(m_Data[i]);
            }
        }

        public VFile NewFile(string name, byte[] data)
        {
            VFile file = new VFile(name, Head, (uint)data.Length);
            fixed (byte* dataStart = data)
            {
                memcpy(m_Data + Head, dataStart, (ulong)data.Length);
            }

            Head.Value += (uint)data.Length;
            return file;
        }

        public byte[] ReadFile(VFile file)
        {
            byte* dataStart = m_Data + file.Offset;
            byte[] dataArray = new byte[file.Size];
            fixed (byte* dataArrayStart = dataArray)
            {
                memcpy(dataArrayStart, dataStart, file.Size);
            }

            return dataArray;
        }

        // helpers
        [DllImport("msvcrt.dll")]
        private static extern void* memcpy(void* destination, void* source, ulong num);
    }
}