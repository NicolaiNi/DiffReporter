using System;
using System.Collections.Generic;
using System.IO;

namespace DiffReporter {
    public class DiffFile {
        public string Filetype { get; set; }
        public Tuple<string, string> PairOfComparedFiles { get; set; }
        public Diff.Item[] Differences { get; set; }
        public List<SingleDiff> AllSingleDiffs = new List<SingleDiff>();
        public string FolderOneFile { get; set; }
        public string FolderTwoFile { get; set; }
        public bool MajorChangeInFile { get; set; } //set to true if Major Change is detected
        public bool NoChange { get; set; }

        public DiffFile(string filetype, Tuple<string, string> pairOfComparedFiles) {
            this.Filetype = filetype;
            this.PairOfComparedFiles = pairOfComparedFiles;
            this.FolderOneFile = GetContent(PairOfComparedFiles.Item1);
            this.FolderTwoFile = GetContent(PairOfComparedFiles.Item2);
            this.Differences = CreateDiff();
        }

        public void Init() {
            CreateSingleDiffs();
        }

        private string GetContent(string path) {
            string content = File.ReadAllText(path);
            content = content.Replace("\r", "");
            return content;
        }

        private Diff.Item[] CreateDiff() {
            Diff.Item[] differences = Diff.DiffText(FolderOneFile, FolderTwoFile, false, false, false);
            if(differences.Length == 0) {
                this.NoChange = true; 
            }
            return differences;
        }

        private void CreateSingleDiffs() {
            List<bool> changes = new List<bool>();
            if (Differences.Length != 0) {
                foreach (var diff in Differences) {
                    SingleDiff singleDiff = new SingleDiff(Filetype, diff, FolderOneFile, FolderTwoFile);
                    singleDiff.Init();
                    AllSingleDiffs.Add(singleDiff);
                    changes.Add(singleDiff.MajorChange);
                }
                // in case of one Major difference is detected in one of the differences, the Diff Object is marked as "major change"
                if (changes.Contains(true)) {
                    this.MajorChangeInFile = true;
                }
            }
        }

    }
}
