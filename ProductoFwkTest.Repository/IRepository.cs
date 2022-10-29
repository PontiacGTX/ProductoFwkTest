using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductoFwkTest.Repository
{
    interface IRepository<T> where T: class
    {
        Task<IList<T>> GetAll();
        Task<T> FirstOrDefault<Tid>(Tid id);
        Task<T> FirstOrDefault(Func<T,bool> selector);
        Task<IList<T>> GetAll(Func<T, bool> selector);
        Task<bool> RemoveRange(Func<T, bool> selector);
        Task<bool> Remove(T s);
        Task<bool> Remove<Tid>(Tid id);
        Task<T> Update<Tid>(Tid id, T s);
        Task<bool> Any<Tid>(Tid id);
        Task<bool> Any<Tid>(Func<T, bool> selector);
    }
}
