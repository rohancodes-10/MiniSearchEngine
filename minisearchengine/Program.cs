using minisearchengine;
using System.ComponentModel.DataAnnotations;

var index = new InvertedIndex();
index.AddDocument(0, "Doc A", "path/a", "the cat sat on the mat running run");
index.AddDocument(1, "Doc B", "path/b", "the cat ran fast runs run");
index.AddDocument(2, "Doc C", "path/c", "football is a popular sport with two teams cat");

var scorer = new TfIdfScore(index);
var results = scorer.Search("the cat is running");
foreach (var r in results)
{
    Console.WriteLine($"[{r.Score:F4}] {r.Title} ({r.SourcePath})");
}