using System;
using System.Collections.Generic;
using System.Text;

namespace minisearchengine
{
    public class SearchResult
    {
            public int DocId { get; set; }
        public string Title { get; set; } = "";
        public string SourcePath{ get; set; } = "";
        public double Score { get; set; }
        public string Snippet { get; set; } = "";
        
    }
}
