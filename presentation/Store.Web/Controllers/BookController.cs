using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepresetory bookRepresetory;
        public BookController(IBookRepresetory bookRepresetory)
        {
            this.bookRepresetory = bookRepresetory;
        }
        public IActionResult Index(int id)
        {
            Book book = bookRepresetory.GetById(id);
            return View(book);
        }
    }
}
