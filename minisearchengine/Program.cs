using minisearchengine;

var result = Tokenizer.Tokenize("The cats are running and shyam runs with them well i do agreeing");
Console.WriteLine(string.Join(", ", result));
