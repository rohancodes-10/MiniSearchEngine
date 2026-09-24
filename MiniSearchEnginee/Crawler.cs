using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;


namespace MiniSearchEnginee
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
       
        private async Task<(string Title,string Text,List<string> Links)> FetchPageAsync(string url)
        {
            //downloading the html as string
          var html=  await _http.GetStringAsync(url);

            //load it into an htmldocument
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            //extracting visible text 
            var contentNode = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");
            var text = contentNode != null ? contentNode.InnerText : doc.DocumentNode.InnerText;

            var TitleNode = doc.DocumentNode.SelectSingleNode("//title");
            string title;
            if (TitleNode != null)
            {
                title = TitleNode.InnerText.Trim();
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
        public async Task<List<(string Url,string Text,string Title)>> CrawlAsync(int maxPages)
        {
            var results = new List<(string Url, string Text,string Title)>();
            while(_frontier.Count>0 && _visited.Count < maxPages)
            {
                var url = _frontier.Dequeue();
                var uri = new Uri(url);
                var normalizedUrl = uri.GetLeftPart(UriPartial.Query);

                if (_visited.Contains(normalizedUrl))
                {
                    continue;
                }
                _visited.Add(normalizedUrl);
                try
                {
                    
                    var domain = uri.Host;
                    var path = uri.AbsolutePath;
                    var disallowedPaths = await GetDisallowedPathsAsync(domain);
                   
                    if (disallowedPaths.Any(p => path.StartsWith(p)))
                    {
                        continue;
                    }
                    var fetchdata = await FetchPageAsync(url);
                    results.Add((normalizedUrl, fetchdata.Text,fetchdata.Title));
                    await Task.Delay(1000);
                    foreach (var link in fetchdata.Links)
                    {
                        var linkUri = new Uri(link);
                        var normalizedLink = linkUri.GetLeftPart(UriPartial.Query);

                        if (!_visited.Contains(normalizedLink))
                        {
                            _frontier.Enqueue(normalizedLink);
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
                bool inWildcardSection = false;

                foreach (var line in Lines)
                {
                    var trimmed = line.Trim();

                    if (trimmed.StartsWith("User-agent:", StringComparison.OrdinalIgnoreCase))
                    {
                        var agent = trimmed.Substring("User-agent:".Length).Trim();
                        inWildcardSection = (agent == "*");
                        continue;
                    }

                    if (inWildcardSection && trimmed.StartsWith("Disallow:", StringComparison.OrdinalIgnoreCase))
                    {
                        var path = trimmed.Substring("Disallow:".Length).Trim();
                        if (path.Length > 0)
                        {
                            disallowed.Add(path);
                        }
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
