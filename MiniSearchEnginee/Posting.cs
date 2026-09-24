using System;
using System.Collections.Generic;
using System.Text;

namespace MiniSearchEnginee
{
    public class Posting
    {
        public int DocId { get; set; }
        public int TermFrequency { get; set; }
        public List<int> Positions { get; set; } = new();
    }
}
