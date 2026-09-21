using minisearchengine;


var crawler = new Crawler("https://en.wikipedia.org/wiki/Web_crawler");
var pages = await crawler.CrawlAsync(10);

var index = new InvertedIndex();
int docid = 0;
foreach(var page in pages)
{
    index.AddDocument(docid, page.Title, page.Url, page.Text);
    docid++;
}

var scorer = new TfIdfScore(index);
while (true)
{
    Console.Write("search> ");
    var query = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(query) || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var results = scorer.Search(query);
    if (results.Count == 0)
    {
        Console.WriteLine("  No results.\n");
        continue;
    }

    foreach (var r in results)
    {
        Console.WriteLine($"[{r.Score:F4}] {r.Title} - {r.Snippet}");
    }
}