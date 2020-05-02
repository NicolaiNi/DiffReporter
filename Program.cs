using DiffReporter.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiffReporter {
    class Program {
        static void Main(string[] args) {
            ////Get Path of two Folders: EFAM and Manually
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
