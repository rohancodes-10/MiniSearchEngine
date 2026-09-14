using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace minisearchengine
{
    public class DocumentMeta
    {
        public int DocId { get; set; }
        public string DocumentName { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string RawText { get; set; } = "";
        public int TermCount { get; set; }

    }
}
