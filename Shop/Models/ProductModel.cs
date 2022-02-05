using Shop.Data;
using Shop.Data.Repo;
using Shop.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class ProductModel : IRepo<Product>
    {
        private readonly ApplicationDbContext context;  

        public ProductModel(ApplicationDbContext context)
        {
            this.context = context;
        }
        public bool Delete(Product t)
        {
            throw new NotImplementedException();
        }

        public Product Find(int id)
        {
            return this.context.Products.Find(id); 
        }

        public IEnumerable<Product> GetAll()
        {
            return this.context.Products.Where(a => a.Id > 0); 
        }

        public bool Insert(Product t)
        {
            throw new NotImplementedException();
        }

        public Product Update(Product t)
        {
            throw new NotImplementedException();
        }
    }
}
