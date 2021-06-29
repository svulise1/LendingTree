using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechLibrary.Data;
using TechLibrary.Data.Entities;
using TechLibrary.Models;
using TechLibrary.ViewModels;
using static System.String;

namespace TechLibrary.Services
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(int bookid);

        Task<List<Book>> GetBooksAsync();

        Task<bool> UpdateBook(BookResponse updatedBook);

        Task<Tuple<int, List<Book>>> GetFilteredBooks(string filterContent, GridRequest gridRequest);
        Task<NewBookResponse> AddBook(BookResponse bookResponse);
        Task<int> GetBookCount();

        Task<List<Book>> GetBooksGridRequest(GridRequest gridRequest);

    }

    public class BookService : IBookService
    {
        private readonly DataContext _dataContext;

        public BookService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// return books based on filtered content, we check filtered content on title and short desc
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<int, List<Book>>> GetFilteredBooks(string filterContent, GridRequest gridRequest)
        {
            if (IsNullOrEmpty(filterContent) || gridRequest is null)
            {
                throw  new ArgumentNullException(nameof(gridRequest), nameof(filterContent));
            }

            //  decrement the page index by 1, so it comes zero index.
            // Imagine if you are one page 1, without decrementing it will always skip the first 10 values

            int selectedPage = gridRequest.CurrentPage;
            selectedPage -= 1;
            List<Book> filterBooks = await _dataContext.Books.Where(m =>
                    m.ShortDescr.ToLower().Contains(filterContent.ToLower()) ||
                    m.Title.ToLower().Contains(filterContent.ToLower()))
                .ToListAsync();

            if (filterBooks.Count == 0)
            {
                return  new Tuple<int, List<Book>>(0,filterBooks);
            }

            List<Book> filteredBookGrid = filterBooks.Skip(gridRequest.PerPage * selectedPage)
                .Take(gridRequest.PerPage).ToList();

            filteredBookGrid = SortBooks(filteredBookGrid, gridRequest);
            return  new Tuple<int, List<Book>>(filterBooks.Count, filteredBookGrid);
        }

        public async Task<int> GetBookCount()
        {
          return  await _dataContext.Books.CountAsync();
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            IQueryable<Book> queryable = _dataContext.Books.AsQueryable();

            return await queryable.ToListAsync();
        }


        public async Task<List<Book>> GetBooksGridRequest(GridRequest gridRequest)
        { 
            List<Book> selectedBooks;

            if (gridRequest.CurrentPage == 1)
            {
              selectedBooks =  await _dataContext.Books.Take(gridRequest.PerPage).ToListAsync();
            }
            else
            {
                int selectedPage = gridRequest.CurrentPage - 1;
                int offset = (selectedPage * gridRequest.PerPage) + 1; 
                selectedBooks = await _dataContext.Books.Where(m => m.BookId >= offset).Take(gridRequest.PerPage)
                    .ToListAsync();
            }
            
            if(selectedBooks is null || selectedBooks.Count <= 0)
                return  new List<Book>();

            return SortBooks(selectedBooks, gridRequest);
        }

        public  List<Book> SortBooks (List<Book> selectedBooks, GridRequest gridRequest)
        {
            if (selectedBooks != null && selectedBooks.Count > 0)
            {
                selectedBooks = gridRequest.SortDesc
                    ? selectedBooks.OrderByDescending(m => m.ISBN).ToList()
                    : selectedBooks.OrderBy(m => m.ISBN).ToList();
            }
            return selectedBooks;
        }

        /// <summary>
        /// returns a book with book id as primary key
        /// </summary>
        /// <returns></returns>
        public async Task<Book> GetBookByIdAsync(int bookid)
        {
            return await _dataContext.Books.SingleOrDefaultAsync(x => x.BookId == bookid);
        }

        /// <summary>
        /// returns a book with ISBN as primary key
        /// </summary>
        /// <returns></returns>
        public async Task<Book> GetBookByIsbnAsync(string isbn)
        {
            if (IsNullOrEmpty(isbn))
                throw new ArgumentNullException(nameof(isbn));
            return await _dataContext.Books.SingleOrDefaultAsync(x =>
                string.Equals(x.ISBN.ToLower(), isbn.ToLower()));
        }

        /// <summary>
        /// Add a new book to db based on the Book Response
        /// </summary>
        /// <returns></returns>

        public async Task<NewBookResponse> AddBook(BookResponse bookResponse)
        {
            // Check if book response is null or not
            if (bookResponse is null)
                throw new ArgumentNullException(nameof(bookResponse));

            // Create the return response
            NewBookResponse response = new NewBookResponse();

            // Check if ISBN is duplicated or not
            Book isIsbnValid = await GetBookByIsbnAsync(bookResponse.ISBN);

            // If ISBN is duplicated, return the response saying that it is not valid
            if (isIsbnValid != null)
            {
                response.IsValid = false;
                return response;
            }

            // Create a new entity based on the bookresponse values
            Book newBook = new Book
            {
                PublishedDate = bookResponse.PublishedDate,
                ISBN = bookResponse.ISBN,
                ShortDescr = bookResponse.Descr,
                Title = bookResponse.Title
            };

            // Add the book
            _dataContext.Add(newBook);

            // Call the commit to database
            await _dataContext.SaveChangesAsync();

            response.IsSuccessful = true;
            response.IsValid = true;
            return response;
        }

        /// <summary>
        /// Update the book, based on the id and book response
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateBook(BookResponse updatedBook)
        {
            //Checking if arguments is null or not
            if (updatedBook is null)
                throw new ArgumentNullException(nameof(updatedBook));

            // Get the book based on the book id
            Book selectedBook = await GetBookByIdAsync(updatedBook.BookId);

            // If Book is null, that means it is a invalid book id
            if (selectedBook is null)
                throw new InvalidOperationException($"Invalid Book Id - {updatedBook.BookId}");

            // Now update the values
            selectedBook.ShortDescr = updatedBook.Descr;
            selectedBook.Title = updatedBook.Title;
            selectedBook.PublishedDate = updatedBook.PublishedDate;

            // call a commit on the database, because the entity is still tracking
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}