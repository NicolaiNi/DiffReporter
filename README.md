# DiffReporter
automated comparison tool of text files coming from SWASH Models; compare output files of two different versions of SWASH Models

## Getting started 
To start this application the following packages are needed 
* DocumentFormat.OpenXml (v2.10.1)
* HtmlToOpenXml.dll (v2.0.3)

## Input
Text files genereated by [SWASH Models](https://www.pesticidemodels.eu/swash/home/) (Surface WAter Scenarios Help).
Text files for comparison have to be present in two folders (subfolders alloed) in working directory; file names has to be equal to be detected as files to compare

## Output
Word file with a summary of differences found between the files in the two folders


## Source 
C# implementation of the O(ND) Difference Algorithm (Myers, 1986) by [M. Hertel](https://www.mathertel.de/Diff/), [source code](https://www.mathertel.de/Diff/ViewSrc.aspx)
