using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffReporter.Logic {
    public class DiffHelper {

        public string RootPath { get; private set; }
        public List<string> ListFilesInBoth { get; private set; }
        //this list can be updated or changed
        public List<string> FileTypes { get; private set; } = new List<string>() { ".sum", "_report.txt" };


        public DiffHelper(string rootPath) {
            this.RootPath = rootPath;
        }

        public void Init() {
            string[] rootPathFolders = Directory.GetDirectories(RootPath);
            if (rootPathFolders.Length == 2) {
                ListFilesInBoth = CheckFolders.GetAllFilesInBoth(rootPathFolders, RootPath);
            } else {
                throw new ApplicationException("Please make sure you have two folders named \"folderOne\" and \"folderTwo\" in your working directory.");
            }
        }

        public void Compare() {
            //Use a Dictionary: key: relative path, values: folderOne path and folderTwo path
            Dictionary<string, Tuple<string, string>> dicAllFiles = new Dictionary<string, Tuple<string, string>>();

            foreach (var path in ListFilesInBoth) {
                var pairOfPaths = new Tuple<string, string>(RootPath + "\\folderOne" + path, RootPath + "\\folderTwo" + path);
                dicAllFiles.Add(path, pairOfPaths);
            }

            List<DiffFile> allDiffFiles = new List<DiffFile>();
            foreach (var filetype in FileTypes) {
                var keys = dicAllFiles.Where(x => x.Key.Contains(filetype)).Select(x => x.Key);

                foreach (var key in keys) {
                    var pairOfPaths = dicAllFiles[key];
                    DiffFile diffFile = new DiffFile(filetype, pairOfPaths);
                    diffFile.Init();
                    allDiffFiles.Add(diffFile);
                }
            }

            if (!allDiffFiles.Any()) {
                Console.WriteLine("No files to compare were found.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            ////OUTPUT
            CreateWord wordMethod = new CreateWord();
            List<string> allDiffXml = new List<string>();
            string header = wordMethod.CreateHeader(allDiffFiles);
            allDiffXml.Add(header);
            allDiffXml.AddRange(wordMethod.CreateMain(allDiffFiles));

            string filename = wordMethod.WriteDocx(allDiffXml);
        }
    }
}
