using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IBookRepresetory bookRepresetory;
        public SearchController(IBookRepresetory bookRepresetory)
        {
            this.bookRepresetory = bookRepresetory;
        }
        public IActionResult Index(string query)
        {
            var books = bookRepresetory.GetAllByTitle(query);
            return View(books);
        }
    }
}