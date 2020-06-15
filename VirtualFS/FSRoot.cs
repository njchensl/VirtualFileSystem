using System;

namespace VirtualFS
{
    [Serializable]
    public class FSRoot
    {
        public VDirectory RootDir { get; }
        public RefUInt Head;

        public FSRoot()
        {
            RootDir = new VDirectory("root", null);
            Head = new RefUInt();
        }
    }
}