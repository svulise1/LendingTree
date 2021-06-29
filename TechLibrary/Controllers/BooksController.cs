using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using TechLibrary.Data.Entities;
using TechLibrary.Models;
using TechLibrary.Services;
using Newtonsoft.Json;
using TechLibrary.Contracts.Responses;
using TechLibrary.ViewModels;

namespace TechLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly IErrorStoreService _errorStoreService;
        private readonly IMemoryCache _memoryCache;
        public BooksController(ILogger<BooksController> logger,
            IBookService bookService,
            IMapper mapper, IErrorStoreService errorStoreService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
            _errorStoreService = errorStoreService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Gets the books by Grid Request.
        /// </summary>
        /// <returns></returns>




        
        [HttpPost]
        public async Task<IActionResult> GetBooksByGridRequest([FromBody] GridRequest gridRequest)
        {
            try
            {
                if(gridRequest is null)
                    throw  new ArgumentNullException(nameof(gridRequest));

                _logger.LogInformation("Get all books with grid conditions");

                // If the Filter Value is String.Empty or Null Value
                if (string.IsNullOrEmpty(gridRequest.Filter))
                {
                    // Check if we have the value in Cache
                    List<BookResponse> bookResponse = _memoryCache.Get<List<BookResponse>>(gridRequest.CurrentPage - 1);

                    if (bookResponse == null)
                    {
                        // If the Value is not in Cache, Get From DB
                        List<Book> books = await _bookService.GetBooksGridRequest(gridRequest);

                        // Using the mapper, Get the Book Response
                        bookResponse = _mapper.Map<List<BookResponse>>(books);

                        // Save the book response to Cache, if the count is same
                        if (bookResponse.Count == gridRequest.PerPage)
                        {
                            _memoryCache.Set(gridRequest.CurrentPage - 1, bookResponse);
                        }
                    }
                    
                    
                    // equate the value and return the result.
                    GridResponse gridResult = new GridResponse
                    {
                        BookResponses = bookResponse,
                        TotalBooks = await _bookService.GetBookCount()
                    };
                    return Ok(gridResult);
                }

                (int totalCount, List<Book> filteredBooks) = await  _bookService.GetFilteredBooks(gridRequest.Filter, gridRequest);
                // Using the mapper, Get the Book Response
                List<BookResponse> filteredBookResponses = _mapper.Map<List<BookResponse>>(filteredBooks);

                return Ok(new GridResponse
                {
                    BookResponses = filteredBookResponses,
                    TotalBooks =  totalCount
                });
            }
            catch (Exception e)
            {
                await _errorStoreService.RecordException(e, JsonConvert.SerializeObject(gridRequest));
                return BadRequest();
            }
        }



        /// <summary>
        /// Add the books to db
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddBook")]
        public async Task<IActionResult> GetBooksByGridRequest([FromBody] BookResponse bookResponse)
        {
            try
            {
                _logger.LogInformation("Add a new book");
                return Ok(await _bookService.AddBook(bookResponse));
            }
            catch (Exception e)
            {
                await _errorStoreService.RecordException(e, JsonConvert.SerializeObject(bookResponse));
                return BadRequest();
            }

        }



        /// <summary>
        /// Update the book based on client response
        /// </summary>
        /// <returns></returns>

        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromBody] BookResponse bookResponse)
        {
            try
            {
                _logger.LogInformation("Updating Book from client");

                bool isSuccess =  await _bookService.UpdateBook(bookResponse);

                if (isSuccess)
                {
                    List<BookResponse> cachedResult = _memoryCache.Get<List<BookResponse>>(bookResponse.PageNum - 1);

                    if (cachedResult!=null)
                    {
                        cachedResult.RemoveAll(m => m.BookId == bookResponse.BookId);
                        cachedResult.Add(bookResponse);
                    }
                }

                return Ok(isSuccess);
            }
            catch (Exception e)
            {
                await _errorStoreService.RecordException(e, JsonConvert.SerializeObject(bookResponse));
                return BadRequest();
            }
        }



        /// <summary>
        /// Get the book based on book id
        /// </summary>
        /// <returns></returns>

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"Get book by id {id}");

                Book book = await _bookService.GetBookByIdAsync(id);

                BookResponse bookResponse = _mapper.Map<BookResponse>(book);

                return Ok(bookResponse);
            }
            catch (Exception e)
            {
                await _errorStoreService.RecordException(e, id.ToString());
                return BadRequest();
            }
        }

    }
}
