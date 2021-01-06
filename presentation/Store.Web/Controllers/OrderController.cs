using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Store.Messages;
using Microsoft.AspNetCore.Http;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepresetory bookRepresetory;
        private readonly IOrderRepresetory orderRepresetory;
        private readonly INotificationService notificationService;

        public OrderController(IBookRepresetory bookRepresetory, IOrderRepresetory orderRepresetory,INotificationService notificationService)
        {
            this.bookRepresetory = bookRepresetory;
            this.orderRepresetory = orderRepresetory;
            this.notificationService = notificationService;
        }
        [HttpPost]
        public IActionResult SendConfirmationCode(int id,string cellPhone)
         {
             var order = orderRepresetory.GetById(id);
             var model = Map(order);

             if (!IsValidCellPhone(cellPhone))
             {
                 model.Errors["cellPhone"] = "Номер телефона не соотвествет";
                 return View("Index", model);
             }
             int code = 1111;//Random.Next(1000,10000)
            HttpContext.Session.SetInt32(cellPhone, code);
            notificationService.SendConfirmationCode(cellPhone, code);

             return View("Confirmation",new ConfirmationModel
             {
                 OrderId=id,
                 CellPhone =cellPhone,
             });
         }

        private bool IsValidCellPhone(string cellPhone)
        {
            if (cellPhone == null)
                return false;
            cellPhone = cellPhone.Replace(" ", "")
                                  .Replace("-", "");

            return Regex.IsMatch(cellPhone, @"^\+?\d{11}$");

        }
        [HttpGet]
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
        [HttpPost]
        public IActionResult AddItem(int bookId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            var book = bookRepresetory.GetById(bookId);

            order.AddOrUpdateItem(book, count);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.GetItem(bookId).Count = count;

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        private (Order order, Cart cart) GetOrCreateOrderAndCart()
        {
            Order order;
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepresetory.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepresetory.Create();
                cart = new Cart(order.Id);
            }

            return (order, cart);
        }

        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepresetory.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);
        }

        public IActionResult RemoveItem(int bookId)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.RemoveItem(bookId);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }
        [HttpPost]
        public IActionResult StartDelivery(int id,string cellPhone,int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone);
            if(storedCode == null)
            {
                return View("Confirmation", new ConfirmationModel
                {
                    OrderId = id,
                    CellPhone = cellPhone,
                    Errors = new Dictionary<string, string>
                 {
                     { "code", "Пустой код, повторите отправку" },
                 }
                });
            }

            if(storedCode!= code)
            {
                return View("Confirmation", new ConfirmationModel
                {
                    OrderId = id,
                    CellPhone = cellPhone,
                    Errors = new Dictionary<string, string>
                 {
                     { "code", "Неверно введён код" },
                 }
                });
            }

            //
            return View();
        }
    }
}
