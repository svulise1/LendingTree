using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Bogus;
using NUnit.Framework;
using TechLibrary.Data;
using TechLibrary.Data.Entities;
using TechLibrary.MappingProfiles;
using TechLibrary.Services;
using TechLibrary.ViewModels;

namespace TechLibrary.Test.Services
{
    public class BookServiceTest
    {




        [Test(Description = "Testing Grid Response with Current Page and 10 items")]
        public async Task TestingPerPage()
        {
            var gridRequest = new GridRequest
            {
                CurrentPage = 1,
                PerPage = 10,
                Filter = string.Empty,
                SortBy = "isdb",
                SortDesc = false
            };
            using SampleDbContextFactory factory = new SampleDbContextFactory();
            // Get a context
            await using (DataContext context = factory.CreateContext())
            {
                var fakeBooks = CreateFakeBooks(10);
                await context.Books.AddRangeAsync(fakeBooks);
                await context.SaveChangesAsync();
            }

            using (var context = factory.CreateContext())
            {
                var bookService = new BookService(context);
                var selectedBooks =  await bookService.GetBooksGridRequest(gridRequest);

                Assert.AreEqual(selectedBooks.Count, 10);
            }
        }



        [Test(Description = "Testing Grid Response with Current Page as 2 and 15 items")]
        public async Task TestingPerPage_PerPage10_CurrentPage2()
        {
            var gridRequest = new GridRequest
            {
                CurrentPage = 2,
                PerPage = 10,
                Filter = string.Empty,
                SortBy = "isdb",
                SortDesc = false
            };
            using SampleDbContextFactory factory = new SampleDbContextFactory();
            // Get a context
            await using (DataContext context = factory.CreateContext())
            {
                var fakeBooks = CreateFakeBooks(15);
                await context.Books.AddRangeAsync(fakeBooks);
                await context.SaveChangesAsync();
            }

            using (var context = factory.CreateContext())
            {
                var bookService = new BookService(context);
                var selectedBooks = await bookService.GetBooksGridRequest(gridRequest);

                Assert.AreEqual(selectedBooks.Count, 5);
            }
        }


        [Test(Description = "Testing Grid Response with Current Page as 30 and 15 items")]
        public async Task TestingPerPage_PerPage10_CurrentPage30()
        {
            var gridRequest = new GridRequest
            {
                CurrentPage = 30,
                PerPage = 10,
                Filter = string.Empty,
                SortBy = "isdb",
                SortDesc = false
            };
            using SampleDbContextFactory factory = new SampleDbContextFactory();
            // Get a context
            await using (DataContext context = factory.CreateContext())
            {
                var fakeBooks = CreateFakeBooks(15);
                await context.Books.AddRangeAsync(fakeBooks);
                await context.SaveChangesAsync();
            }

            using (var context = factory.CreateContext())
            {
                var bookService = new BookService(context);
                var selectedBooks = await bookService.GetBooksGridRequest(gridRequest);

                Assert.AreEqual(selectedBooks.Count, 0);
            }
        }

        public List<Book> CreateFakeBooks(int num)
        {
            Faker<Book> bookFaker = new Faker<Book>().CustomInstantiator(f => new Book())
                .RuleFor(m => m.BookId,
                    0)
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


    }
}