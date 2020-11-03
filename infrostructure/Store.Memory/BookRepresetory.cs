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
            new Book(1,"Art of Programming"),
            new Book(2,"Refactoring"),
            new Book(3,"C Programming Lanuage"),
        };
        public Book[] GetAllByTitle(string titlePart)
        {
            return books.Where(book => book.Title.Contains(titlePart))
                        .ToArray();
        }
    }
}
