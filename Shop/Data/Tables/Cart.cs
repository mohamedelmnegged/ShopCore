﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Data.Tables
{
    public class Cart
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
