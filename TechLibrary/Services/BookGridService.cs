using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TechLibrary.Contracts.Responses;
using TechLibrary.Data.Entities;
using TechLibrary.Models;
using TechLibrary.ViewModels;

namespace TechLibrary.Services
{
    public interface IBookGridService
    {
        GridResponse GetBooksGridAsync(GridRequest gridRequest, List<Book> allBooks);

    }

    public class BookGridService : IBookGridService
    {
        private readonly IMapper _mapper;
        public BookGridService( IMapper mapper)
        {
            _mapper = mapper;
        }



        /// <summary>
        /// Modify the allBooks books based on grid request
        /// </summary>
        /// <returns></returns>
        public  GridResponse GetBooksGridAsync(GridRequest gridRequest, List<Book> allBooks)
        {
            GridResponse response = new GridResponse();

            // If any of the the parameters is null, just return an empty object
            if (gridRequest is null || allBooks is null || !allBooks.Any() ||
                gridRequest.PerPage <= 0 || gridRequest.CurrentPage <=0)
            {
                return response;
            }
           

            // total books counts is required for rows property of b table
            response.TotalBooks = allBooks.Count();

            //  decrement the page index by 1, so it comes zero index.
            // Imagine if you are one page 1, without decrementing it will always skip the first 10 values
            gridRequest.CurrentPage -= 1;


            // We are doing pagination over here based on the gridrequest current page and per page property
            IEnumerable<Book> selectedBooks = allBooks
                .Skip(gridRequest.PerPage * gridRequest.CurrentPage)
                .Take(gridRequest.PerPage);

            // Some of the columns in grid can be sorted, so we are sorting based on that property
            List<Book> orderedBooks =  SortOrderedBooks(selectedBooks, gridRequest);

            // Auto mapper helps us mapping the properties from book to bookresponse
            List<BookResponse> bookResponse = _mapper.Map<List<BookResponse>>(orderedBooks);
            
            // equate the books 
            response.BookResponses = bookResponse;

            return response;
        }


        /// <summary>
        /// sort the allBooks books based on the grid request sort property 
        /// </summary>
        /// <returns></returns>
        public  List<Book> SortOrderedBooks(IEnumerable<Book> queryable, GridRequest gridRequest )
        {

            if (queryable is null || gridRequest is null)
            {
                throw new ArgumentNullException(nameof(gridRequest), nameof(queryable));
            }

            List<Book> orderedBooks = gridRequest.SortDesc
                ? queryable.OrderByDescending(m => m.ISBN).ToList()
                : queryable.OrderBy(m => m.ISBN).ToList();

            return orderedBooks;
        }
    }
}