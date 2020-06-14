using System;

namespace VirtualFS
{
    [Serializable]
    public class VFile
    {
        public string Name { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }

        public VFile(string name, uint offset, uint size)
        {
            Name = name;
            Offset = offset;
            Size = size;
        }
    }

}