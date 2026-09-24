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
        public IActionResult Index()
        {
            return View();
        }
    }
}
