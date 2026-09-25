using Microsoft.AspNetCore.Mvc;

namespace MiniSearchEnginee.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchEngineService _searchEngineService;
        public SearchController(SearchEngineService searchEngineService)
        {
            _searchEngineService = searchEngineService;
        }

        [HttpGet]
        public IActionResult Results(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View();
            }
            var results = _searchEngineService.Scorer.Search(q);
            return View(results);
            
        }
        [HttpGet]
        public IActionResult Crawl()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crawl(string seedUrl)
        {
            if (string.IsNullOrWhiteSpace(seedUrl))
            {
                TempData["Error"] = "Please enter a url";
                RedirectToAction("Crawl");
            }
            await _searchEngineService.BuildIndexAsync(seedUrl, 15);
            return RedirectToAction("Results");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
