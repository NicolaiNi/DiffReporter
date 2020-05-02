using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiffReporter {
    public class SingleDiff {
        public string Filetype { get; set; }
        public string DiffCategory { get; set; }
        public Diff.Item DiffItem { get; set; }
        public int DeletedA { get; set; }
        public int InsertedB { get; set; }
        public int StartA { get; set; }
        public int StartB { get; set; }
        public string[] FolderOneFileLines { get; set; }
        public string[] FolderTwoFileLines { get; set; }
        public string FolderOneDiffLine { get; set; }
        public string FolderTwoDiffLine { get; set; }
        public string SingleXml { get; set; }
        private string major = "major";
        private string minor = "minor";
        public bool MajorChange { get; set; }


        public SingleDiff(string filetype, Diff.Item diff, string FolderOneFile, string FolderTwoFile) {
            this.Filetype = filetype;
            this.DiffItem = diff;
            this.FolderOneFileLines = SplitLines(FolderOneFile);
            this.FolderTwoFileLines = SplitLines(FolderTwoFile);
        }

        public void Init() {
            SetDiffInfo();
            SetDiffLine(); //create the starting point for this single diff
            GetSingleDiffText(); //transform line to int and convert string to xml/html style text to highlight diffs on character basis
            this.DiffCategory = SetDiffCategory();
            SetMajorDiffDetected();
            ChangeSingleDiffDescription(); //if a major change is found, change color of "Line +X -Y" to red
        }

        private string[] SplitLines(string file) {
            string[] splitLines = file.Split('\n');
            return splitLines;
        }

        private void SetDiffInfo() {
            this.DeletedA = DiffItem.deletedA;
            this.InsertedB = DiffItem.insertedB;
            this.StartA = DiffItem.StartA;
            this.StartB = DiffItem.StartB;
        }

        private void SetDiffLine() {
            string lineFromA = "";
            for (int i = StartA; i < StartA + DeletedA; i++) {
                lineFromA = lineFromA + "\n" + FolderOneFileLines[i];
            }
            this.FolderOneDiffLine = lineFromA;

            string lineFromB = "";
            for (int j = StartB; j < StartB + InsertedB; j++) {
                lineFromB = lineFromB + "\n" + FolderTwoFileLines[j];
            }
            this.FolderTwoDiffLine = lineFromB;
        }


        private void GetSingleDiffText() {
            bool ingnorecase = false;
            //// transform string to int
            int[] aCodes = TransformationDiffCharCodes(FolderOneDiffLine, ingnorecase);
            int[] bCodes = TransformationDiffCharCodes(FolderTwoDiffLine, ingnorecase);

            Diff.Item[] diffsDetail = Diff.DiffInt(aCodes, bCodes);
            this.SingleXml = SingleDiffXml(diffsDetail, FolderOneDiffLine, FolderTwoDiffLine);
        }

        private string SetDiffCategory() {
            string category = nameof(major);
            string lineContent = String.Copy(SingleXml);

            switch (Filetype) {
                case "_report.txt":
                    // changes in working direcory/File Name Section
                    string searchPattern = "File Name";
                    if (lineContent.Contains(searchPattern)) {
                        category = CheckWorkingDir(lineContent);
                        if (category == nameof(major)) {
                            break;
                        }
                    }
                    //description
                    if (lineContent.Contains("Description")) {
                        //differences in Description part considered as minor change
                            category = nameof(minor);
                    }
                    //Substance Code
                    if (lineContent.Contains("Substance")) {
                        category = CheckSubstanceCode(lineContent);
                        if (category == nameof(major)) {
                            break;
                        }
                    }
                    //Creation Time
                    if (lineContent.Contains("Creation")) {
                        //differences in Creation part considered as minor change
                        category = nameof(minor);
                    }
                    break;
                case ".sum":
                    // changes in working direcory/File Name Section
                    string searchPattern2 = "Working directory";
                    if (lineContent.Contains(searchPattern2)) {
                        category = CheckWorkingDir(lineContent);
                        if (category == nameof(major)) {
                            break;
                        }
                    }
                    // changes in generation dates
                    if (lineContent.Contains("Input file generated")) {
                        //differences in generation date are considered as minor
                         category = nameof(minor);
                    }
                    // changes in run time
                    if (lineContent.Contains("run time")) {
                        //run time differences are considered as minor
                          category = nameof(minor);
                    }
                    break;
                default:
                    break;
            }

            return category;
        }
        private string CheckWorkingDir(string lineContent) {
            string category = "N/A";
            List<string> matchListString = new List<string>();

            if (lineContent.Contains($"<span itemprop=\"A\"")) {
                if (lineContent.Contains($"<span itemprop=\"B\"")) {
                    string[] splitLine = Regex.Split(lineContent, @" : ");
                    if (splitLine.Length != 0) {
                        string workingDirectory = splitLine[1].Trim();
                        MatchCollection matches = Regex.Matches(workingDirectory, $"<span itemprop=\"A\" style=\"background-color:Salmon\">(?<oldvalue>.*?)</span><span itemprop=\"B\" style=\"background-color:LightGreen\">(?<newvalue>.*?)</span>", RegexOptions.Multiline);
                        foreach (Match match in matches) {
                            var oldvalue = match.Groups["oldvalue"];
                            var newvalue = match.Groups["newvalue"];
                            matchListString.Add(oldvalue.ToString());
                            matchListString.Add(newvalue.ToString());

                            if (oldvalue.ToString().ToLower() == newvalue.ToString().ToLower()) {
                                if (category != nameof(major)) {
                                    category = nameof(minor);
                                }
                            } else {
                                category = nameof(major);
                            }
                        }
                        MatchCollection matches2 = Regex.Matches(workingDirectory, $"<span itemprop=.*?>(?<value>.*?)</span>", RegexOptions.Multiline);
                        foreach (Match match in matches2) {
                            string toTest = match.Groups["value"].ToString();
                            if (matchListString.Contains(toTest)) {
                                //the change is already detected and classified
                            } else {
                                // a new difference is detected, it is only present in one file --> deletion or insertion
                                category = nameof(major);
                            }
                        }
                    }
                } else {
                    //insertion in working directory in FolderOne File
                    category = nameof(major);
                }
            } else {
                if (lineContent.Contains($"<span  itemprop=\"B\"")) {
                    //insertion in working directory in FolderTwo File
                    category = nameof(major);
                }
            }

            return category;
        }

        private string CheckSubstanceCode(string lineContent) {
            //first 3 characters have to be equal in both
            string category = "N/A";

            string[] splitLine = Regex.Split(lineContent, @" : ");
            if (splitLine.Length != 0) {
                string substanceCode = splitLine[1].Trim();
                Match match = Regex.Match(substanceCode, @"([\w]{3})<span");
                if (match.Success) {
                    category = nameof(minor);
                } else {
                    //difference is somewhere in between first 3 characters
                    category = nameof(major);
                }
            }
            return category;
        }

        public static int[] TransformationDiffCharCodes(string aText, bool ignoreCase) {
            int[] Codes;

            if (ignoreCase)
                aText = aText.ToUpperInvariant();

            Codes = new int[aText.Length];

            for (int n = 0; n < aText.Length; n++)
                Codes[n] = (int)aText[n];

            return (Codes);
        }

        public string SingleDiffXml(Diff.Item[] diffs, string a, string b) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<singlediff>");
            sb.Append("<span style=\"font-weight:bold\">" + $"Line +{StartA} -{StartB}</span><br>");
            int pos = 0;

            // n number of changes
            for (int n = 0; n < diffs.Length; n++) {
                Diff.Item it = diffs[n];
                // write unchanged chars
                while (pos < it.StartB && pos < b.Length) {
                    //Console.Write(b[pos]);
                    sb.Append(b[pos]);
                    pos++;
                }
                //write deleted chars
                if (it.deletedA > 0) {
                    //Console.Write("#");
                    sb.Append("<span itemprop=\"A\" style=\"background-color:Salmon\">");
                    for (int m = 0; m < it.deletedA; m++) {
                        //Console.Write(a[it.StartA + m]);
                        sb.Append(a[it.StartA + m]);
                    }
                    //Console.Write("#");
                    sb.Append("</span>");
                }
                // write inserted chars
                if (pos < it.StartB + it.insertedB) {
                    //Console.Write("\\");
                    sb.Append("<span itemprop=\"B\" style=\"background-color:LightGreen\">");
                    while (pos < it.StartB + it.insertedB) {
                        //Console.Write(b[pos]);
                        sb.Append(b[pos]);
                        pos++;
                    }
                    //Console.Write("\\");
                    sb.Append("</span>");
                }
            }
            while (pos < b.Length) {
                //Console.Write(b[pos]);
                sb.Append(b[pos]);
                pos++;
            }

            sb.Append("</singlediff>");
            string singleXml = sb.ToString();
            return singleXml;
        }//end function

        public void SetMajorDiffDetected() {
            if (DiffCategory == nameof(major)) {
                this.MajorChange = true;
            }
            if (DiffCategory == nameof(minor)) {
                this.MajorChange = false;
            }

        }
        public void ChangeSingleDiffDescription() {
            if (MajorChange == true) {
                string singleXml = String.Copy(SingleXml);
                singleXml = singleXml.Replace("<span style=\"font-weight:bold\">", "<span style=\"color:red;font-weight:bold\">");
                SingleXml = singleXml;
            }
        }

    }
}
