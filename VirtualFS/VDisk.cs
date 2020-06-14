using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VirtualFS
{
    public unsafe class VDisk
    {
        private byte* m_Data;
        public ulong Size;

        public RefUInt Head { get; set; }

        public VDisk(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            m_Data = (byte*)Marshal.AllocHGlobal(data.Length);
            Size = (ulong)data.Length;
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
            for (ulong i = 0; i < Size; i++)
            {
                fs.WriteByte(m_Data[i]);
            }
            fs.Flush();
            fs.Close();
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

        public void Defragment(FSRoot root, int size)
        {
            byte* newBuffer = (byte*)Marshal.AllocHGlobal(size);
            uint head = 0;

            void FileDefrag(VFile f)
            {
                memcpy(newBuffer + head, m_Data + f.Offset, f.Size);
                f.Offset = head;
                head += f.Size;
            }

            root.RootDir.FileForEach(FileDefrag);
            root.RootDir.DirForEach(dir => { dir.FileForEach(FileDefrag); });
            Size = (ulong)size;
            Marshal.FreeHGlobal((IntPtr)m_Data);
            m_Data = newBuffer;
        }

        // helpers
        [DllImport("msvcrt.dll")]
        private static extern void* memcpy(void* destination, void* source, ulong num);
    }
}