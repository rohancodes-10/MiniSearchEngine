using minisearchengine;

var corpusPath = Path.Combine(AppContext.BaseDirectory, "corpus");
var index = new InvertedIndex();

var files = Directory.GetFiles(corpusPath, "*.txt");
var docId = 0;
foreach (var file in files)
{
    var text = File.ReadAllText(file);
    var title = Path.GetFileNameWithoutExtension(file);
    index.AddDocument(docId, title, file, text);
    docId++;
}

var scorer = new TfIdfScore(index);

while (true)
{
    Console.Write("search> ");
    var query = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(query) || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var results = scorer.Search(query);
    foreach (var r in results)
    {
        Console.WriteLine($"[{r.Score:F4}] {r.Title} - {r.Snippet}");
    }
}