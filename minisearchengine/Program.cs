using minisearchengine;


var crawler = new Crawler("https://en.wikipedia.org/wiki/Web_crawler");
var pages = await crawler.CrawlAsync(10);
foreach (var page in pages)
{
    Console.WriteLine($"{page.Url} - {page.Text.Length} chars");
}