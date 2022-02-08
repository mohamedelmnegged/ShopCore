using Shop.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Data.Tables
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public Status Status { get; set; }
        public int Amount { get; set; }
        public Purchase Purchase { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
        public IEnumerable<Cart> Carts { get; set; }

    }
}
