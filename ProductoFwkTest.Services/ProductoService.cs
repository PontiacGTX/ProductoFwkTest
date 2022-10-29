using ProductoFwkTest.Repository;
using System;

namespace ProductoFwkTest.Services
{
    public class ProductoService
    {
        ProductoRepository _productoRepository { get; }
        public ProductoService(ProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.IList<Entities.Producto>>  GetAll()
        {
            return await _productoRepository.GetAll();
        }
        public async System.Threading.Tasks.Task<Entities.Producto> FirstOrDefault<Tid>(Tid id)
        {
            return await _productoRepository.FirstOrDefault(id);
        }

        public async System.Threading.Tasks.Task<bool> Any<Tid>(Tid id)
        {
            return await _productoRepository.Any(id);
        }

        public async System.Threading.Tasks.Task<Entities.Producto> FirstOrDefault(Func<Entities.Producto, bool> selector)
        {
            return await _productoRepository.FirstOrDefault(selector);
        }  public async System.Threading.Tasks.Task<Entities.Producto> Update<Tid>(Tid id, Entities.Producto s)
        {
            return await _productoRepository.Update(id,s);
        } 
        public async System.Threading.Tasks.Task<System.Collections.Generic.IList<Entities.Producto>> GetAll(Func<Entities.Producto, bool> selector)
        {
            return await _productoRepository.GetAll(selector);
        }

        public async System.Threading.Tasks.Task<bool> Remove<Tid>(Tid id)
        {
            return await _productoRepository.Remove(id);
        }

        public async System.Threading.Tasks.Task<bool> RemoveRange(Func<Entities.Producto, bool> selector)
        {
            return await _productoRepository.Remove(selector);
        }
        
        public async System.Threading.Tasks.Task<int> Count(Func<Entities.Producto, bool> selector)
        {
            return await _productoRepository.Count(selector);
        }

        public async System.Threading.Tasks.Task<int> Count()
        {
            return await _productoRepository.Count();
        }
    }
      
}
