﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Store.Tests
{
    public class OrderItemTests
    {
        [Fact]
        public void OrederItem_WithZeroCount()
        {
            int count = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(1, 0m, count));
        }
        [Fact]
        public void OrederItem_WithNegativeCount()
        {
            int count = -1;
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(1, 0m, count));
        }
        [Fact]
        public void OrederItem_WithPositiveCount()
        {
            var ordetItem = new OrderItem(1, 3m, 2);

            Assert.Equal(1, ordetItem.BookId);
            Assert.Equal(2, ordetItem.Count);
            Assert.Equal(3m, ordetItem.Price);
        }
    }
}
