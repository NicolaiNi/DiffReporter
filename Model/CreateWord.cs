using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml; //for hmtl converter

namespace DiffReporter {
    public class CreateWord {
        public string WriteDocx(List<string> allDiffXml) {
            string rootPath = Directory.GetCurrentDirectory();
            string user = Environment.UserName;
            string date = DateTime.Today.ToShortDateString().Replace(".", "");


            string wordFilename = Path.Combine(rootPath, $"report.diff.{date}_{user}.docx");
            if (File.Exists(wordFilename)) {
                File.Delete(wordFilename);
            }
            using (MemoryStream generatedDocument = new MemoryStream()) {

                using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document)) {
                    MainDocumentPart mainPart = package.AddMainDocumentPart();

                    new Document(new Body()).Save(mainPart); 

                    HtmlConverter converter = new HtmlConverter(mainPart);

                    foreach (var item in allDiffXml) {
                        converter.ParseHtml(item);
                    }
                    mainPart.Document.Save();
                }
                

                File.WriteAllBytes(wordFilename, generatedDocument.ToArray());
                //Process.Start(wordFilename);
            }
            return $"report.diff.{date}_{user}.docx";
        }

        public string CreateHeader(List<DiffFile> allDiffFiles) {
            string header = "";
            header = "<p style=\"font-weight:bold\">Report Diff Files</p>";

            int numberDiffFiles = allDiffFiles.Count();
            header += $"<p>In total, {numberDiffFiles} file(s) were analyzed.</p>";
            List<string> filetypes = allDiffFiles.Select(d => d.Filetype).Distinct().ToList();
            header += "<p><ul>";
            foreach (var filetype in filetypes) {
                int numberFiletype = allDiffFiles.Where(x => x.Filetype == filetype).Count();
                header += $"<li>{numberFiletype} <i>*{filetype}</i> file(s)</li>";
            }
            header += "</ul></p>";
            int NumberMajorChanges = allDiffFiles.Where(d => d.MajorChangeInFile == true).Count();
            if (NumberMajorChanges > 0) {
                header += $"Major changes were identified in {NumberMajorChanges} file(s).<br>";
            } else {
                header += $"No major changes were identified.<br>";
            }
            int NumberNoChanges = allDiffFiles.Where(d => d.NoChange == true).Count();
            if (NumberNoChanges > 0) {
                header += $"No changes were identified for {NumberNoChanges} file(s).";
            } 
            header += "<br><br>";

            return header;
        }

        public List<string> CreateMain(List<DiffFile> allDiffFiles) {
            List <string> main = new List<string>();

            foreach (var diff in allDiffFiles.OrderByDescending(d => d.MajorChangeInFile).ThenBy(d => d.NoChange)) {
                StringBuilder sb = new StringBuilder();

                if (diff == allDiffFiles.OrderByDescending(d => d.MajorChangeInFile).ThenBy(d => d.NoChange).First())
                {
                    sb.Append("<ol>");
                }
                var t = Path.GetFileName(diff.PairOfComparedFiles.Item1);
                sb.Append("<li><diff>");
                sb.Append("<span>Filetype: <i>" + diff.Filetype + "</i></span><br>");
                sb.Append("<span>FolderOne: " + $"<a href=\"{diff.PairOfComparedFiles.Item1}\">" + Path.GetFileName(diff.PairOfComparedFiles.Item1) + "</a></span><br>");
                sb.Append("<span>FolderTwo: " + $"<a href=\"{diff.PairOfComparedFiles.Item2}\">" + Path.GetFileName(diff.PairOfComparedFiles.Item2) + "</a></span><br>");

                foreach (var singleDiff in diff.AllSingleDiffs) {
                    sb.Append(singleDiff.SingleXml);
                    sb.Append("<br>");
                }
                sb.Append("</diff></li>");
                if (diff == allDiffFiles.OrderByDescending(d => d.MajorChangeInFile).ThenBy(d => d.NoChange).Last())
                {
                    sb.Append("</ol>");
                }
                string diffXml = sb.ToString();
                main.Add(diffXml);
            }
            
            return main;
        }

    }
}
