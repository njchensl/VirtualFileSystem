using System.IO;
using static stdio;

namespace VirtualFS
{
    public unsafe class VDisk
    {
        private byte* m_Data;
        public ulong Size;
        public string Path { get; }

        public RefUInt Head { get; set; }

        public VDisk(string path)
        {
            Path = path;
            FileInfo f = new FileInfo(path);

            m_Data = (byte*)malloc((ulong)f.Length);
            Size = (ulong)f.Length;
            void* file = fopen(path, "rb");
            fread(m_Data, (ulong)f.Length, 1, file);
            fclose(file);
        }

        ~VDisk()
        {
            free(m_Data);
        }

        public void Write()
        {
            void* file = fopen(Path, "wb");
            fwrite(m_Data, Size, 1, file);
            fclose(file);
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

        public byte* ReadFileNative(VFile file)
        {
            return m_Data + file.Offset;
        }
        
        public void Defragment(FSRoot root, int size)
        {
            byte* newBuffer = (byte*)malloc((ulong)size);
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
            free(m_Data);
            m_Data = newBuffer;
        }
    }
}