using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Store.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithNullItems()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(1, null));
        }
        [Fact]
        public void TotalCount_WithEmtyItems_ReturnsZero()
        {
            var order = new Order(1, new OrderItem[0]);

            Assert.Equal(0, order.TotalCount);
        }

        [Fact]
        public void TotalPirce_WithEmtyItems_ReturnsZero()
        {
            var order = new Order(1, new OrderItem[0]);

            Assert.Equal(0m, order.TotalPrice);
        }
        [Fact]
        public void TotalCount_WithNotEmtyItems_CalcualeteTotalCount()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1,10m,3),
                new OrderItem(2,100m,5),
            });

            Assert.Equal(3 + 5, order.TotalCount);

        }
        [Fact]
        public void TotalPrice_WithNotEmtyItems_CalcualeteTotalPrice()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1,10m,3),
                new OrderItem(2,100m,5),
            });

            Assert.Equal(3 * 10m + 5 * 100m, order.TotalPrice);

        }
    }
}
