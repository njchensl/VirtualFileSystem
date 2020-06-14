using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VirtualFS
{
    [Serializable]
    public class VDirectory
    {
        public string Name { get; set; }
        private IList<VDirectory> m_SubDirectories;
        private IList<VFile> m_Files;

        public VDirectory(string name)
        {
            Name = name;
            m_SubDirectories = new List<VDirectory>();
            m_Files = new List<VFile>();
        }

        public VDirectory GetSubDirectory(string name)
        {
            return m_SubDirectories.FirstOrDefault(dir => dir.Name == name);
        }

        public void AddSubDirectory(VDirectory dir)
        {
            m_SubDirectories.Add(dir);
        }

        public VFile GetFile(string name)
        {
            return m_Files.FirstOrDefault(file => file.Name == name);
        }

        public void DirForEach(Action<VDirectory> action)
        {
            m_SubDirectories.ForEach(action);
        }
        
        public void FileForEach(Action<VFile> action)
        {
            m_Files.ForEach(action);
        }

        public void AddFile(VFile file)
        {
            m_Files.Add(file);
        }
    }
}