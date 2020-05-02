using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffReporter {
    public class CheckFolders {
        public static List<string> GetAllFilesInBoth(string[] rootPathFolders, string rootPath) {
            string searchPattern = "*";
            List<string> pathAllFilesOne = new List<string>();
            List<string> pathAllFilesTwo = new List<string>();


            if (rootPathFolders[0].Contains("folderOne")) {
                pathAllFilesOne = Directory.GetFiles(rootPathFolders[0], searchPattern, SearchOption.AllDirectories).ToList();
            }else {
                Console.WriteLine("Folder with name \"folderOne\" was not found.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            if (rootPathFolders[1].Contains("folderTwo")) {
                pathAllFilesTwo = Directory.GetFiles(rootPathFolders[1], searchPattern, SearchOption.AllDirectories).ToList();
            } else {
                Console.WriteLine("Folder with name \"folderTwo\" was not found.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            List<string> pathAllFilesOneReplace = new List<string>();

            foreach (var path in pathAllFilesOne) {
                string pathReplace = path.Replace(rootPath + "\\folderOne", "");
                pathAllFilesOneReplace.Add(pathReplace);
            }

            List<string> pathAllFilesTwoReplace = new List<string>();

            foreach (var path in pathAllFilesTwo) {
                string pathReplace = path.Replace(rootPath + "\\folderTwo", "");
                pathAllFilesTwoReplace.Add(pathReplace);
            }

            var listPathInBothFolders = pathAllFilesOneReplace.Where(x => pathAllFilesTwoReplace.Contains(x)).ToList();

            return listPathInBothFolders;
        }

    }
}
