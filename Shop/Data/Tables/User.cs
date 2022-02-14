using Microsoft.AspNetCore.Identity;
using Shop.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Data.Tables
{
    public class User : IdentityUser
    {
        public Status Status { get; set; } = Status.pinding;
        public string Address { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Checkout> Checkouts { get; set; }
    }
}
