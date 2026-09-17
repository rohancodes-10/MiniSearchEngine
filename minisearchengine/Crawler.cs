using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace minisearchengine
{
    public class Crawler
    {
        private readonly HashSet<string> _visited = new();
        private readonly Queue<string> _frontier = new();
        private readonly HttpClient _http = new();

       

        public Crawler(string seedUrl)
        {
            _http.DefaultRequestHeaders.Add("User-Agent", "MiniSearchEngineBot/1.0");
            _frontier.Enqueue(seedUrl);
        }
       
        private async Task<(string text,List<string> links)> FetchPageAsync(string url)
        {
            //downloading the html as string
          var html=  await _http.GetStringAsync(url);

            //load it into an htmldocument
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            //extracting visible text 
            var text = doc.DocumentNode.InnerText;

            var baseUri = new Uri(url);
            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
            var links = new List<string>();

            if (linkNodes != null)
            {
                foreach(var node in linkNodes)
                {
                    var result=node.GetAttributeValue("href", "");
                    try
                    {
                        var resolvedlink = new Uri(baseUri, result);
                        links.Add(resolvedlink.ToString());
                    }
                    catch (UriFormatException)
                    {
                        
                    }
                    
                }
            }
            return (text, links);
        }
        public async Task<List<(string Url,string Text)>> CrawlAsync(int maxPages)
        {
            var results = new List<(string Url, string Text)>();
            while(_frontier.Count>0 && _visited.Count < maxPages)
            {
                var url = _frontier.Dequeue();

                if (_visited.Contains(url))
                {
                    continue;
                }
                _visited.Add(url);
                try
                {
                    var fetchdata = await FetchPageAsync(url);
                    results.Add((url, fetchdata.text));

                    foreach (var link in fetchdata.links)
                    {
                        if (!_visited.Contains(link))
                        {
                            _frontier.Enqueue(link);
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    // this page failed to fetch (404, redirect issue, timeout, etc.) - skip it, keep crawling
                    Console.WriteLine($"  Skipped (fetch failed): {url}");
                }
            }
            return results;
        }
        
    }
}
