
using System.Collections.Generic;
using TechLibrary.Models;

namespace TechLibrary.Contracts.Responses
{
    public class GridResponse
    {
        public int TotalBooks { get; set; }
        public List<BookResponse> BookResponses { get; set; }

        public GridResponse()
        {
            BookResponses = new List<BookResponse>();
        }
    }
}