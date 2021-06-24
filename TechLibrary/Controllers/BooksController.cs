using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TechLibrary.Data.Entities;
using TechLibrary.Models;
using TechLibrary.Services;
using Newtonsoft.Json;
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
        private readonly IBookGridService _bookGridService;
        private readonly IErrorStoreService _errorStoreService;
        public BooksController(ILogger<BooksController> logger,
            IBookService bookService,
            IMapper mapper,
            IBookGridService bookGridService, IErrorStoreService errorStoreService)
        {
            _logger = logger;
            _bookService = bookService;
            _mapper = mapper;
            _bookGridService = bookGridService;
            _errorStoreService = errorStoreService;
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
                _logger.LogInformation("Get all books with grid conditions");
                if (string.IsNullOrEmpty(gridRequest?.Filter))
                {
                    var books =await _bookService.GetBooksAsync();
                    return Ok( _bookGridService.GetBooksGridAsync(gridRequest, books));
                }

                var filteredBooks = await  _bookService.GetFilteredBooks(gridRequest?.Filter);
                return Ok( _bookGridService.GetBooksGridAsync(gridRequest, filteredBooks));
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

                return Ok(await _bookService.UpdateBook(bookResponse));
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
