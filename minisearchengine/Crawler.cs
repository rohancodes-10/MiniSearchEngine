using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace minisearchengine
{
    public class Crawler
    {
        private readonly HashSet<string> _visited = new();
        private readonly Queue<string> _frontier = new();
        private readonly HttpClient _http = new();
        private readonly Dictionary<string, HashSet<string>> _robotscache = new();

       

        public Crawler(string seedUrl)
        {
            _http.DefaultRequestHeaders.Add("User-Agent", "MiniSearchEngineBot/1.0");
            _frontier.Enqueue(seedUrl);
        }
       
        private async Task<(string Title,string text,List<string> links)> FetchPageAsync(string url)
        {
            //downloading the html as string
          var html=  await _http.GetStringAsync(url);

            //load it into an htmldocument
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            //extracting visible text 
            var text = doc.DocumentNode.InnerText;

            var Title = doc.DocumentNode.SelectSingleNode("//title");
            string title;
            if (Title != null)
            {
                title = Title.InnerText.Trim();
            }
            else
            {
                title = url;
            }

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
            return (title,text, links);
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
                    var uri = new Uri(url);
                    var domain = uri.Host;
                    var path = uri.AbsolutePath;
                    var disallowedPaths = await GetDisallowedPathsAsync(domain);

                    if (disallowedPaths.Any(p => path.StartsWith(p)))
                    {
                        continue;
                    }
                    var fetchdata = await FetchPageAsync(url);
                    results.Add((url, fetchdata.text));
                    await Task.Delay(1000);
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

        private async Task<HashSet<string>> GetDisallowedPathsAsync(string domain)
        {
            if (_robotscache.ContainsKey(domain))
            {
                return _robotscache[domain];
            }
            var disallowed = new HashSet<string>();
            try
            {
               var robotsUrl= $"https://{domain}/robots.txt";
                var robotsText = await _http.GetStringAsync(robotsUrl);
                var Lines = robotsText.Split('\n');

                foreach(var line in Lines)
                {
                    if(line.StartsWith("Disallow:", StringComparison.OrdinalIgnoreCase))
                    {
                        var path = line.Substring(9).Trim();
                        disallowed.Add(path);
                    }

                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("no robots.txt or failed to fetch");
            }
            _robotscache[domain] = disallowed;
            return disallowed;
        }
        
    }
}
