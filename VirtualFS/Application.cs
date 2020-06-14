using System;

namespace VirtualFS
{
    internal static class Application
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            CommandExecutor executor = new CommandExecutor();
            while (executor.Running)
            {
                string workingDir = executor.GetWorkingDir();
                if (workingDir.Trim() != "")
                {
                    workingDir = workingDir.Substring(0, workingDir.Length - 1);
                }
                Console.Write(workingDir + "$ ");
                executor.Execute(Console.ReadLine());
            }
        }
    }
}