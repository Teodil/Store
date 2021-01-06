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
            new Book(1,"Art of Programming","ISBN 12312-31231","D. Knuth",
                "This multivolume work on the analysis of algorithms has long been recognized as the definitive description of classical computer science. The four volumes published to date already comprise a unique and invaluable resource in programming theory and practice. Countless readers have spoken about the profound personal influence of Knuth’s writings",26.99m),
            new Book(2,"Refactoring","ISBN 12312-31232","M. Flowler",
                "Whenever you read [Refactoring], it’s time to read it again. And if you haven’t read it yet, please do before writing another line of code",45.14m),
            new Book(3,"C Programming Lanuage","ISBN 12312-31233","B. Kernighan, D. Ritchie",
                "The authors present the complete guide to ANSI standard C language programming. Written by the developers of C, this new version helps readers keep up with the finalized ANSI standard for C while showing how to take advantage of C's rich set of operators, economy of expression, improved control flow, and data structures.",38.89m),
        };

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(book => book.Isbn == isbn)
                .ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            if (titleOrAuthor != null)
            {
                return books.Where(book => book.Author.Contains(titleOrAuthor)
                        || book.Title.Contains(titleOrAuthor))
                        .ToArray();
            }
            return books;
        }

        public Book GetById(int id)
        {
            return books.Single(book => book.Id == id);
        }

        public Book[] GetAllByIds(IEnumerable<int> bookIds)
        {
            var foundBooks = from book in books
                             join bookId in bookIds on book.Id equals bookId
                             select book;

            return foundBooks.ToArray();
        }
    }
}
