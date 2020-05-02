# DiffReporter
automated comparison tool of text files coming from SWASH Models

## Input
Text Files genereated by [SWASH Models](https://www.pesticidemodels.eu/swash/home/) (Surface WAter Scenarios Help)
Text Files has to be present in two folders (subfolders alloed) in working directory; file names has to be equal to be detected as files to compare

## Output
Word file with a summary of differences found between the files in the two folders

## Source 
C# implementation of the O(ND) Difference Algorithm (Myers, 1986) by [M. Hertel](https://www.mathertel.de/Diff/), [source code](https://www.mathertel.de/Diff/ViewSrc.aspx)
