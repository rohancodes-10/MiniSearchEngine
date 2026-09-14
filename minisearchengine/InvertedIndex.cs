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
        }
    }
}
