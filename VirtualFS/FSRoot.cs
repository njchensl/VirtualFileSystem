using System;
using System.Collections.Generic;

namespace VirtualFS
{
    [Serializable]
    public class FSRoot
    {
        [NonSerialized]
        private VDisk m_Disk;
        public VDirectory RootDir { get; }
        public RefUInt Head;

        public FSRoot(VDisk disk)
        {
            m_Disk = disk;
            RootDir = new VDirectory("root");
            Head = new RefUInt();
        }
    }
}