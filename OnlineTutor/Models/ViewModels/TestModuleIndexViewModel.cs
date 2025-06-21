namespace OnlineTutor.Models.ViewModels
{
    public class TestModuleIndexViewModel
    {
        public int SpellingTestsCount { get; set; }
        public int RegularTestsCount { get; set; }
        public int TotalTestsCount => SpellingTestsCount + RegularTestsCount;
    }
}
