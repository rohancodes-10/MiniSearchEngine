using System;
using System.Collections.Generic;
using System.Text;

namespace minisearchengine
{
    public class InvertedIndex
    {
        private Dictionary<string, List<Posting>> Index { get; set; } = new();

        public Dictionary<int, DocumentMeta> Documents { get; set; } = new();
        public void AddDocument(int docId,string title,string sourcePath,string rawText)
        {
            var tokenizedtext = Tokenizer.Tokenize(rawText);
            var doc = new DocumentMeta
            {
                DocId = docId,
                DocumentName = title,
                SourcePath = sourcePath,
                RawText = rawText,
                TermCount = tokenizedtext.Count
            };
            Documents[docId] = doc;
            var termPositions = new Dictionary<string, List<int>>();
            for (int position = 0; position < tokenizedtext.Count; position++)
            {
                var term = tokenizedtext[position];
                if (termPositions.TryGetValue(term, out var positions))
                {
                    positions.Add(position);
                }
                else
                {
                    var newPositions = new List<int>();
                    newPositions.Add(position);
                    termPositions[term] = newPositions;
                }
            }
                foreach(var(term,positions) in termPositions)
            {
                if(Index.TryGetValue(term,out var postings))
                {
                    
                }
                else
                {
                    postings =new List<Posting>();
                    Index[term] = postings;
                }
                var newPosting = new Posting
                {
                    DocId = docId,
                    TermFrequency = positions.Count,
                    Positions = positions
                };
                postings.Add(newPosting);

            }
            
        }
        public void DebugPrintIndex()
        {
            foreach (var (term, postings) in Index)
            {
                Console.WriteLine($"{term}:");
                foreach (var p in postings)
                {
                    Console.WriteLine($"  doc {p.DocId} - freq {p.TermFrequency} - positions [{string.Join(", ", p.Positions)}]");
                }
            }
        }
        public List<Posting> GetPostings(string term1)
        {
            if(Index.TryGetValue(term1,out var postings))
            {
                return postings;
            }
            return new List<Posting>();
        }
    }
}
