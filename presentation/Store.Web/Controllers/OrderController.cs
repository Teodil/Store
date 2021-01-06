using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepresetory bookRepresetory;
        private readonly IOrderRepresetory orderRepresetory;

        public OrderController(IBookRepresetory bookRepresetory, IOrderRepresetory orderRepresetory)
        {
            this.bookRepresetory = bookRepresetory;
            this.orderRepresetory = orderRepresetory;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                var order = orderRepresetory.GetById(cart.OrderId);

                OrderModel model = Map(order);
                return View(model);
            }
            return View("Empty");
        }

        private OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);
            var books = bookRepresetory.GetAllByIds(bookIds);
            var itemModels = from item in order.Items
                             join book in books on item.BookId equals book.Id
                             select new OrderItemModel
                             {
                                 BookId = book.Id,
                                 Title = book.Title,
                                 Author = book.Author,
                                 Price = item.Price,
                                 Count = item.Count,
                             };

            return new OrderModel
            {
                Id = order.Id,
                Items = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
            };

        }

        public IActionResult AddItem(int id)
        {
            Order order;
            Cart cart;
            if (HttpContext.Session.TryGetCart(out cart))
            {
                order = orderRepresetory.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepresetory.Create();
                cart = new Cart(order.Id);
            }

            var book = bookRepresetory.GetById(id);
            order.AddItem(book, 1);
            orderRepresetory.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
          
            HttpContext.Session.Set(cart);

            return RedirectToAction("Index","Book",new {id});
        }
    }
}
