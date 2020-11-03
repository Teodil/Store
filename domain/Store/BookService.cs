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
        public Book[] GetAllByQuery(string query)
        {
            if (Book.IsIsbn(query))
                return bookRepresetory.GetAllByIsbn(query);
            return bookRepresetory.GetAllByTitleOrAuthor(query);
        }
    }
}
