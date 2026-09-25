# Mini Search Engine

A small, real search engine built from scratch in C# / ASP.NET Core — crawler, inverted index, TF-IDF ranking, and a web UI, all hand-written with no external search libraries.

Type in a URL, it crawls the site (respecting `robots.txt` and rate limits), builds a searchable index, and lets you run ranked keyword searches against it — through a browser.

## Features

- **Crawler** — starts from a user-supplied seed URL, follows links within a page limit, respects `robots.txt` per domain (cached), waits between requests, and survives individual page failures without crashing the crawl. URLs are normalized (fragments stripped) to avoid indexing the same page twice.
- **Indexer** — a hand-built inverted index (`term → documents containing it`), with a tokenizer (lowercase, stopword removal, suffix-stripping stemmer) shared between documents and queries.
- **Ranking** — TF-IDF (term frequency × inverse document frequency), so results are ranked by relevance, not just presence of a keyword.
- **Web UI** — ASP.NET Core MVC. Submit a URL to crawl, then search the resulting index, see ranked results with scores and snippets.

## Architecture

```
User submits seed URL
        │
        ▼
    Crawler ──► fetches pages, extracts text + links (HtmlAgilityPack)
        │
        ▼
  InvertedIndex ──► tokenizes each page, builds term → postings
        │
        ▼
   TfIdfScorer ──► scores and ranks documents for a query
        │
        ▼
    Web UI ──► search box, ranked results
```

## Running it

Requires .NET 10 SDK.

```
cd MiniSearchEnginee
dotnet run
```

Or open the `.sln` in Visual Studio and press Run.

Navigate to the app in your browser — it opens on the crawl form. Submit a seed URL (crawling takes roughly 1 second per page, due to the deliberate rate-limiting delay), then search the resulting index.

**Example seed URLs to try:**
- `https://en.wikipedia.org/wiki/Web_crawler`
- `https://en.wikipedia.org/wiki/Search_engine`
- `https://en.wikipedia.org/wiki/Machine_learning`

## Known limitations

This was built as a learning project with a deliberately scoped-down MVP, not a production search engine. Known gaps:

- **Snippets** show the first ~150 characters of a document, not text centered on the matched query term.
- **Content extraction** has a Wikipedia-specific fallback (`div#mw-content-text`) for cleaner article text; other sites fall back to the full page's visible text, which can be noisy (nav menus, footers, etc.).
- **No persistence** — the index is rebuilt from a live crawl every time; nothing is saved to disk or a database between runs.
- **No phrase search, fuzzy/typo-tolerant matching, or BM25** — ranking is plain TF-IDF only.
- **Stemming** is a simplified suffix-stripper, not the full Porter Stemmer algorithm, and can't handle irregular word forms (e.g. "ran" doesn't stem to "run").

## Tech stack

C# · ASP.NET Core MVC · HtmlAgilityPack · no external search/indexing libraries — the tokenizer, inverted index, and ranking are all hand-written.
