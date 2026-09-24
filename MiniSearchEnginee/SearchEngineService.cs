using MiniSearchEnginee;

namespace MiniSearchEnginee
{
    public class SearchEngineService
    {
        public InvertedIndex Index { get; }
        public TfIdfScore Scorer { get; }
        public SearchEngineService(InvertedIndex index, TfIdfScore scorer)
        {
            Index = index;
            Scorer = scorer;
        }
       
    }
}
