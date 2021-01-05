﻿using Microsoft.AspNetCore.Mvc;
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

        public CartController(IBookRepresetory bookRepresetory)
        {
            this.bookRepresetory = bookRepresetory;
        }

        public IActionResult Add(int id)
        {
            var book = bookRepresetory.GetById(id);
            Cart cart;
            if (!HttpContext.Session.TryGetCart(out cart))
                cart = new Cart();

            if (cart.Items.ContainsKey(id))
                cart.Items[id]++;
            else
                cart.Items[id] = 1;

            cart.Amount += book.Price;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index","Book",new {id});
        }
    }
}
