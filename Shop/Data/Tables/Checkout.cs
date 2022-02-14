using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Data.Tables
{
    public class Checkout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Provance { get; set; }
        public int Postal { get; set; }
        public int Phone { get; set; }
        public string NameOnCard { get; set; }
        public int Card { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
