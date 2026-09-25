

namespace MiniSearchEnginee
{
    public class SearchEngineService
    {
        public InvertedIndex? Index { get; private set; }
        public TfIdfScore? Scorer { get; private set; }
      
        public async Task BuildIndexAsync(string seedUrl,int maxPages)
        {
            Crawler crawler = new Crawler(seedUrl);
            var pages =await crawler.CrawlAsync(maxPages);

            var index = new InvertedIndex();
            int docid = 0;
            foreach(var page in pages)
            {
                index.AddDocument(docid, page.Title, page.Url, page.Text);
                docid++;
            }
            var scorer = new TfIdfScore(index);

            Index = index;
            Scorer = scorer;

        }
       
    }
}
