using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Bogus;
using NUnit.Framework;
using TechLibrary.Contracts.Responses;
using TechLibrary.Data.Entities;
using TechLibrary.MappingProfiles;
using TechLibrary.Services;
using TechLibrary.ViewModels;

namespace TechLibrary.Test.Services
{
    public class BookGridServiceTest
    {
        private IMapper _mockMapper;

        [OneTimeSetUp]
        public void TestSetup()
        {
            DomainToResponseProfile myProfile = new DomainToResponseProfile();
            MapperConfiguration configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mockMapper = new Mapper(configuration);
        }



        public List<Book> CreateFakeBooks(int num)
        {
            Faker<Book> bookFaker = new Faker<Book>().CustomInstantiator(f => new Book())
                .RuleFor(m => m.BookId,
                    m => m.IndexFaker)
                .RuleFor(m => m.ISBN,
                    k => k.Commerce.Ean13())
                .RuleFor(m => m.Title,
                    m => m.Company.CompanyName())
                .RuleFor(m => m.ShortDescr,
                    m => m.Commerce.ProductDescription())
                .RuleFor(m => m.PublishedDate,
                    m => DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

            List<Book> fakeBooks = bookFaker.Generate(num);

            return fakeBooks;
        }

        [Test(Description = "Testing Grid Response with per page 10")]
        public void TestingPerPage()
        {
            //Act 
        
            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks =  CreateFakeBooks(100);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = false,
                CurrentPage = 1,
                Filter = string.Empty,
                PerPage = 10,
                SortBy = string.Empty
            };

           GridResponse selectedBooks =   bookGridService.
               GetBooksGridAsync(gridRequest, fakeBooks);


           // Assert
           Assert.AreEqual(selectedBooks.TotalBooks, 100);
           Assert.AreEqual(selectedBooks.BookResponses.Count, 10);
        }


        [Test(Description = "Testing with Null Responses")]

        public void TestingNullResponse()
        {
            //Act 
            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange

            GridResponse selectedBooks = bookGridService.
                GetBooksGridAsync(null, null);


            // Assert
            Assert.AreEqual(selectedBooks.TotalBooks, 0);
            Assert.AreEqual(selectedBooks.BookResponses.Count, 0);
        }



        [Test(Description = "Testing Grid Response with per page Negative Number")]
        public void TestingPerPage_NegativeNumber()
        {
            //Act 

            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks = CreateFakeBooks(100);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = false,
                CurrentPage = -1,
                Filter = string.Empty,
                PerPage = -2,
                SortBy = string.Empty
            };

            GridResponse selectedBooks = bookGridService.
                GetBooksGridAsync(gridRequest, fakeBooks);


            // Assert
            Assert.AreEqual(selectedBooks.TotalBooks, 0);
            Assert.AreEqual(selectedBooks.BookResponses.Count, 0);
        }




        [Test(Description = "Testing Grid Response with 7 books")]
        public void TestingGridResponse_LimitedBooks()
        {
            //Act 

            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks = CreateFakeBooks(7);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = false,
                CurrentPage = 1,
                Filter = string.Empty,
                PerPage = 10,
                SortBy = string.Empty
            };

            GridResponse selectedBooks = bookGridService.
                GetBooksGridAsync(gridRequest, fakeBooks);


            // Assert
            Assert.AreEqual(selectedBooks.TotalBooks, 7);
            Assert.AreEqual(selectedBooks.BookResponses.Count, 7);
        }


        [Test(Description = "Testing Grid Response with 14 books and 2 page")]
        public void TestingGridResponse_LimitedBooks_NextPage()
        {
            //Act 

            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks = CreateFakeBooks(14);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = false,
                CurrentPage = 2,
                Filter = string.Empty,
                PerPage = 10,
                SortBy = string.Empty
            };

            GridResponse selectedBooks = bookGridService.
                GetBooksGridAsync(gridRequest, fakeBooks);


            // Assert
            Assert.AreEqual(selectedBooks.TotalBooks, 14);
            Assert.AreEqual(selectedBooks.BookResponses.Count, 4);
        }



        [Test(Description = "Testing Grid Response with 14 books and High page number")]
        public void TestingGridResponse_LimitedBooks_HighPageNumber()
        {
            //Act 

            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks = CreateFakeBooks(14);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = false,
                CurrentPage = 10,
                Filter = string.Empty,
                PerPage = 10,
                SortBy = string.Empty
            };

            GridResponse selectedBooks = bookGridService.
                GetBooksGridAsync(gridRequest, fakeBooks);


            // Assert
            Assert.AreEqual(selectedBooks.TotalBooks, 14);
            Assert.AreEqual(selectedBooks.BookResponses.Count, 0);
        }



        [Test(Description = "Testing Sort Ordered books as per grid property desc")]
        public void TestingSortOrderDesc()
        {
            //Act 

            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks = CreateFakeBooks(14);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = true,
                CurrentPage = 10,
                Filter = string.Empty,
                PerPage = 10,
                SortBy = "isbn"
            };

            List<Book> selectedBooks = bookGridService.SortOrderedBooks( fakeBooks, gridRequest);

            fakeBooks = fakeBooks.OrderByDescending(m => m.ISBN).ToList();


            // Assert
            Assert.AreEqual(selectedBooks, fakeBooks);
        }


        [Test(Description = "Testing Sort Ordered books as per grid property asc")]
        public void TestingSortOrderAesc()
        {
            //Act 

            BookGridService bookGridService = new BookGridService(_mockMapper);

            //Arrange
            List<Book> fakeBooks = CreateFakeBooks(14);


            GridRequest gridRequest = new GridRequest
            {
                SortDesc = false,
                CurrentPage = 10,
                Filter = string.Empty,
                PerPage = 10,
                SortBy = "isbn"
            };

            List<Book> selectedBooks = bookGridService.SortOrderedBooks(fakeBooks, gridRequest);

            fakeBooks = fakeBooks.OrderBy(m => m.ISBN).ToList();


            // Assert
            Assert.AreEqual(selectedBooks, fakeBooks);
        }
    }
}