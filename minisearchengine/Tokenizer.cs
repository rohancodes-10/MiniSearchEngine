using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace minisearchengine
{
     public static class Tokenizer
    {
        public static List<string> Tokenize(string rawText)
        {
            var LowerText = rawText.ToLower();
            string[] pieces = Regex.Split(LowerText, @"[^a-z0-9]+");


            var filtered =new List<string>();
          //Storing token less than or equal to 1 length  
            foreach(var p in pieces)
           
            {
                if (p.Length > 1 && !StopWords.Contains(p))
                {
                    var result = Stemmer.Stem(p);
                    filtered.Add(result);
                }
                
            }

            return filtered;
            
        }
        
    }
    public static class StopWords
    {
        private static readonly HashSet<string> Words = new()
        {
             "a", "an", "the", "and", "or", "but", "is", "are", "was", "were",
        "in", "on", "at", "for", "with", "by", "from", "of", "to", "it"
        };
        public static bool Contains(string word) => Words.Contains(word);
    }
}
public static class Stemmer
{
    private static readonly string[] suffixes = { "ing", "s", "es", "ly", "ed" };

    public static string Stem(string word)
    {
        if (word.Length <= 3)
        {
            return word;
        }
        foreach(var suffix in suffixes)
        {
            if (word.EndsWith(suffix,StringComparison.Ordinal))
            {
                var stem = word.Substring(0, word.Length - suffix.Length);
                if (suffix == "ing"&& stem[^2] == stem[^1]&& !"aeiou".Contains(stem[^1]))
                {
                   var result= stem.Substring(0, stem.Length - 1);
                    return result;
                }
                return stem;
            }

        }
        return word;
    }
}
