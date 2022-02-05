using Shop.Data;
using Shop.Data.Repo;
using Shop.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class OrderModel : IRepo<Order>
    {
        private readonly ApplicationDbContext context;

        public OrderModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        public bool Delete(Order t)
        {
            throw new NotImplementedException();
        }

        public Order Find(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetAll()
        {
            return context.Orders.Where(a => a.Id > 0); 
        }

        public bool Insert(Order t)
        {
            throw new NotImplementedException();
        }

        public Order Update(Order t)
        {
            throw new NotImplementedException();
        }
    }
}
