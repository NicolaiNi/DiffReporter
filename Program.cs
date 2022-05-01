using DiffReporter.Logic;
using System;
using System.IO;

namespace DiffReporter
{
    class Program {
        static void Main(string[] args) {
            ////Get Path of two Folders:
            string rootPath = Directory.GetCurrentDirectory();

            var diffHelper = new DiffHelper(rootPath);
            diffHelper.Init();
            diffHelper.Compare();
            Console.WriteLine($"The application finished successfully. The file was created in {rootPath}.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
