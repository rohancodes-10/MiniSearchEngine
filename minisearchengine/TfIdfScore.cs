using System;
using System.Collections.Generic;
using System.Text;

namespace minisearchengine
{
    public class TfIdfScore
    {
        private readonly InvertedIndex _index;
        public TfIdfScore(InvertedIndex index)
        {
            _index = index;
        }
        public List<SearchResult> Search(string query,int topN = 10)
        {
            var queryTerms = Tokenizer.Tokenize(query);
            int totalDcos = _index.Documents.Count;
            var scores = new Dictionary<int, Double>();
            foreach(var term in queryTerms)
            {
                var postings = _index.GetPostings(term);
                if (postings.Count==0)
                {
                    continue;
                }
                var idf = Math.Log((double)totalDcos / postings.Count);
                foreach(var posting in postings)
                {
                    var doc = _index.Documents[posting.DocId];
                    var tf = (double)posting.TermFrequency / doc.TermCount;
                    var termscore = tf * idf;
                    if(scores.TryGetValue(posting.DocId,out double score))
                    {
                        scores[posting.DocId] = score + termscore;
                    }
                    else
                    {
                        scores[posting.DocId] = termscore;
                    }
                }
            }
            return scores
                 .OrderByDescending(kv => kv.Value)
                 .Take(topN)
                 .Select(kv => new SearchResult
                 {
                     DocId = kv.Key,
                     SourcePath = _index.Documents[kv.Key].SourcePath,
                     Title = _index.Documents[kv.Key].DocumentName,
                     Score = kv.Value,
                     Snippet = BuildSnippet(_index.Documents[kv.Key].RawText)
                 }).ToList();
            
        }
        private string BuildSnippet(string rawText)
        {
            if (rawText.Length <= 150)
            {
                return rawText;
            }
            else
            {
                return rawText.Substring(0, 150)+"...";
            }
        }
    }
}
