using ProductoFwkTest.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductoFwkTest.Services
{
    public class ProductoCatService
    {
        CategoriaProductoRepository _productoRepository { get; }
        public ProductoCatService(CategoriaProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.IList<Entities.ProductoCat>> GetAll()
        {
            return await _productoRepository.GetAll();
        }
        public async System.Threading.Tasks.Task<Entities.ProductoCat> FirstOrDefault<Tid>(Tid id)
        {
            return await _productoRepository.FirstOrDefault(id);
        }

        public async System.Threading.Tasks.Task<bool> Any<Tid>(Tid id)
        {
            return await _productoRepository.Any(id);
        }

        public async System.Threading.Tasks.Task<Entities.ProductoCat> FirstOrDefault(Func<Entities.ProductoCat, bool> selector)
        {
            return await _productoRepository.FirstOrDefault(selector);
        }
        public async System.Threading.Tasks.Task<Entities.ProductoCat> Update<Tid>(Tid id, Entities.ProductoCat s)
        {
            return await _productoRepository.Update(id, s);
        }
        public async System.Threading.Tasks.Task<System.Collections.Generic.IList<Entities.ProductoCat>> GetAll(Func<Entities.ProductoCat, bool> selector)
        {
            return await _productoRepository.GetAll(selector);
        }

        public async System.Threading.Tasks.Task<bool> Remove<Tid>(Tid id)
        {
            return await _productoRepository.Remove(id);
        }

        public async System.Threading.Tasks.Task<bool> RemoveRange(Func<Entities.ProductoCat, bool> selector)
        {
            return await _productoRepository.Remove(selector);
        }

        public async System.Threading.Tasks.Task<int> Count(Func<Entities.ProductoCat, bool> selector)
        {
            return await _productoRepository.Count(selector);
        }

        public async System.Threading.Tasks.Task<int> Count()
        {
            return await _productoRepository.Count();
        }
    }
}
