using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Store.Messages;
using Microsoft.AspNetCore.Http;
using Store.Contractors;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepresetory bookRepresetory;
        private readonly IOrderRepresetory orderRepresetory;
        private readonly INotificationService notificationService;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;

        public OrderController(IBookRepresetory bookRepresetory,
                                IOrderRepresetory orderRepresetory,
                                INotificationService notificationService,
                                IEnumerable<IDeliveryService> deliveryServices,
                                IEnumerable<IPaymentService> paymentServices)
        {
            this.bookRepresetory = bookRepresetory;
            this.orderRepresetory = orderRepresetory;
            this.notificationService = notificationService;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
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

             return View("Confirmation", new ConfirmationModel
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
        public IActionResult Confirmate(int id,string cellPhone,int code)
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

            var order = orderRepresetory.GetById(id);
            order.CellPhone = cellPhone;
            orderRepresetory.Update(order);

            HttpContext.Session.Remove(cellPhone);

            var model = new DeliveryModel
            {
                OrderId = id,
                Methods = deliveryServices.ToDictionary(service => service.UniqueCode,
                                                      service => service.Title),
            };

            return View("DeliveryMethod",model);
        }
        [HttpPost]
        public IActionResult StartDelivery(int id,string uniqueCode)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepresetory.GetById(id);

            var form = deliveryService.CreateForm(order);

            return View("DeliveryStep", form);
        }
        [HttpPost]
        public IActionResult NextDelivery(int id,string uniqueCode,int step,Dictionary<string,string> values)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);

            var form = deliveryService.MoveNextForm(id, step, values);
            if (form.IsFinal)
            {
                var order = orderRepresetory.GetById(id);
                order.Delivery = deliveryService.GetDelivery(form);
                orderRepresetory.Update(order);

                var model = new DeliveryModel
                {
                    OrderId = id,
                    Methods = paymentServices.ToDictionary(service => service.UniqueCode,
                                                           service => service.Title),
                };

                return View("PaymentMethod", model);
            }

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult StartPayment(int id, string uniqueCode)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepresetory.GetById(id);

            var form = paymentService.CreateForm(order);

            return View("PaymentStep", form);
        }
        [HttpPost]
        public IActionResult NextPayment(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);

            var form = paymentService.MoveNextForm(id, step, values);
            if (form.IsFinal)
            {
                var order = orderRepresetory.GetById(id);
                order.Payment = paymentService.GetPayment(form);
                orderRepresetory.Update(order);

                return View("Finish");
            }

            return View("PaymentStep", form);
        }
    }
}
