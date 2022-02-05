﻿using Microsoft.AspNetCore.Identity;
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
        public IEnumerable<Order> Orders { get; set; }

    }
}