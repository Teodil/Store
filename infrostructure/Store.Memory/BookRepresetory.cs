using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Memory
{
    public class BookRepresetory : IBookRepresetory
    {
        private readonly Book[] books = new[]
        {
            new Book(1,"Art of Programming","ISBN 12312-31231","D. Knuth"),
            new Book(2,"Refactoring","ISBN 12312-31232","M. Flowler"),
            new Book(3,"C Programming Lanuage","ISBN 12312-31233","B. Kernighan, D. Ritchie"),
        };

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(book => book.Isbn == isbn)
                .ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            return books.Where(book => book.Author.Contains(titleOrAuthor)
                                    || book.Title.Contains(titleOrAuthor))
                        .ToArray();
        }
    }
}
