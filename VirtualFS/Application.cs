using System;

namespace VirtualFS
{
    internal static class Application
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            CommandExecutor executor = new CommandExecutor();
            while (executor.Running)
            {
                executor.Execute(Console.ReadLine());
            }
        }
    }
}