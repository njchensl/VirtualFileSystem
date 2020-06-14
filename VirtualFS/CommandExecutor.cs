using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VirtualFS
{
    public class CommandExecutor
    {
        public bool Running { get; set; }
        private VDisk m_Disk;
        private FSRoot m_Root;
        private Stack<VDirectory> m_DirStack;

        public CommandExecutor()
        {
            Running = true;
            m_DirStack = new Stack<VDirectory>();
        }

        public void Execute(string command)
        {
            string[] args = command.Trim().Split(" ");
            switch (args[0])
            {
                case "exit":
                {
                    Running = false;
                    break;
                }
                case "init":
                {
                    ulong size = ulong.Parse(args[1]);
                    FileStream s = File.Create("disk.vd");
                    for (ulong i = 0; i < size; i++)
                    {
                        s.WriteByte(0);
                    }

                    s.Flush();
                    s.Close();
                    break;
                }
                case "read":
                {
                    if (m_Disk != null || m_Root != null)
                    {
                        break;
                    }

                    m_Disk = new VDisk("disk.vd");
                    IFormatter formatter = new BinaryFormatter();
                    try
                    {
                        Stream stream = new FileStream("fs", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        m_Root = (FSRoot)formatter.Deserialize(stream);
                        stream.Flush();
                        stream.Close();
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("File system does not exist. Creating a new file system.");
                        Console.ForegroundColor = ConsoleColor.White;
                        m_Root = new FSRoot(m_Disk);
                    }

                    m_Disk.Head = m_Root.Head;
                    m_DirStack.Push(m_Root.RootDir);
                    break;
                }
                case "write":
                {
                    m_Disk.Write();
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream("fs", FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, m_Root);
                    stream.Flush();
                    stream.Close();
                    break;
                }
                case "workingDir":
                case "pwd":
                {
                    Console.WriteLine(GetWorkingDir());
                    break;
                }
                case "changeDir":
                case "cd":
                {
                    if (args[1].StartsWith("root/"))
                    {
                        args[1] = args[1].Substring(5);
                        m_DirStack.Clear();
                        m_DirStack.Push(m_Root.RootDir);
                    }

                    string[] dirs = args[1].Split("/");
                    foreach (var dir in dirs)
                    {
                        if (dir.Trim().Length == 0)
                        {
                            break;
                        }

                        if (dir == "..")
                        {
                            m_DirStack.Pop();
                        }
                        else
                        {
                            VDirectory target = m_DirStack.Peek().GetSubDirectory(dir);
                            if (target == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Not Found: " + dir);
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            }

                            m_DirStack.Push(target);
                        }
                    }

                    break;
                }
                case "moveFileIn":
                case "mfi":
                case "store":
                {
                    string name = args[1];
                    string fileName = args[2];
                    byte[] data = File.ReadAllBytes(fileName);
                    VFile file = m_Disk.NewFile(name, data);
                    m_DirStack.Peek().AddFile(file);
                    break;
                }
                case "takeFileOut":
                case "tfo":
                case "retrieve":
                {
                    string name = args[1];
                    VFile file = m_DirStack.Peek().GetFile(name);
                    if (file == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File : " + name + " does not exist");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }

                    byte[] fileData = m_Disk.ReadFile(file);
                    FileStream s = File.Create(name);
                    foreach (byte b in fileData)
                    {
                        s.WriteByte(b);
                    }

                    s.Flush();
                    s.Close();
                    break;
                }
                case "list":
                case "ls":
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    m_DirStack.Peek().DirForEach(dir =>
                    {
                        Console.WriteLine($"|{"Dir",15}   |   {dir.Name,40}    |    {dir.Size + " Bytes",20}|");
                    });
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    m_DirStack.Peek().FileForEach(f =>
                    {
                        Console.WriteLine($"|{"File",15}   |   {f.Name,40}    |    {f.Size + " Bytes",20}|");
                    });
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
                case "makeDir":
                case "mkdir":
                {
                    m_DirStack.Peek().AddSubDirectory(new VDirectory(args[1]));
                    break;
                }
                case "removeDir":
                case "rmdir":
                {
                    VDirectory target = m_DirStack.Peek().GetSubDirectory(args[1]);
                    if (target == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not Found: " + args[1]);
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }

                    m_DirStack.Peek().RemoveSubDirectory(target);
                    break;
                }
                case "removeFile":
                case "rm":
                {
                    string name = args[1];
                    VFile file = m_DirStack.Peek().GetFile(name);
                    if (file == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File : " + name + " does not exist");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }

                    m_DirStack.Peek().RemoveFile(file);
                    break;
                }
                case "defragment":
                case "defrag":
                {
                    int size;
                    if (args.Length >= 2)
                    {
                        size = int.Parse(args[1]);
                    }
                    else
                    {
                        size = (int)m_Disk.Size;
                    }

                    m_Disk.Defragment(m_Root, size);
                    break;
                }
                case "diskSize":
                case "dsize":
                {
                    Console.WriteLine("Size: " + m_Disk.Size);
                    break;
                }
            }
        }

        public string GetWorkingDir()
        {
            StringWriter sw = new StringWriter();
            m_DirStack.AsEnumerable()!.Reverse().Select(dir => dir.Name + "/").ForEach(sw.Write);
            return sw.ToString();
        }
    }
}