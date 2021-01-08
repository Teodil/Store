using Store.Web.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class BookService//Служба для обратоки запроса по книгами
    {
        private readonly IBookRepresetory bookRepresetory;
        public BookService(IBookRepresetory bookRepresetory)
        {
            this.bookRepresetory = bookRepresetory;
        }
        
        public BookModel GetById(int id)
        {
            var book = bookRepresetory.GetById(id);

            return Map(book);
        }

        public IReadOnlyCollection<BookModel> GetAllByQuery(string query)
        {
            var books = Book.IsIsbn(query)
                        ?bookRepresetory.GetAllByIsbn(query)
                        : bookRepresetory.GetAllByTitleOrAuthor(query);

            return books.Select(Map)
                        .ToArray();
        }

        private BookModel Map(Book book)
        {
            return new BookModel
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price,
            };
        }
    }
}
