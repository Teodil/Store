using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IBookRepresetory bookRepresetory;
        private readonly IOrderRepresetory orderRepresetory;

        public CartController(IBookRepresetory bookRepresetory, IOrderRepresetory orderRepresetory)
        {
            this.bookRepresetory = bookRepresetory;
            this.orderRepresetory = orderRepresetory;
        }

        public IActionResult Add(int id)
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
