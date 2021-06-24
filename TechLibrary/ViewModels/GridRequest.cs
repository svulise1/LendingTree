namespace TechLibrary.ViewModels
{
    public class GridRequest
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public string SortBy { get; set; }
        public bool SortDesc { get; set; }
        public string Filter { get; set; }
    }
}


//currentPage: 2
// filter: undefined
// perPage: 10
// sortBy: "isbn"
// sortDesc: true