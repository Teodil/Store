using Store.Contractors;
using System;
using System.Collections.Generic;
using System.Text;
using Store.Web.Contractors;

namespace Store.YandexKassa
{
    public class YandexKassaPaymentService : IPaymentService, IWebContractorServise
    {
        public string UniqueCode => "YandexKassa";

        public string Title => "Оплата банковской картой";

        public string GetUrl => "/YandexKassa/";

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.Id, 1, true, new Field[0]);
        }

        public OrderPayment GetPayment(Form form)
        {
            return new OrderPayment(UniqueCode, "Оплата картой", new Dictionary<string, string>());
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            return new Form(UniqueCode, orderId, 2, true, new Field[0]);
        }
    }
}
