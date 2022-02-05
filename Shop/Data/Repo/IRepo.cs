using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Data.Repo
{
    interface IRepo<Table>
    {
        Table Find(int id);
        bool Insert(Table t);
        Table Update(Table t);
        bool Delete(Table t);
        IEnumerable<Table> GetAll(); 

    }
}
