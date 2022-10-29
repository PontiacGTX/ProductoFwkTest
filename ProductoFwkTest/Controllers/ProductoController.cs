using ProductoFwkTest.Entities;
using ProductoFwkTest.Entities.Models;
using ProductoFwkTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProductoFwkTest.Controllers
{
    public class ProductoController : Controller
    {
        private ProductoService _productoService;
        private ProductoCatService _productoCatService;

        public ProductoController(ProductoService productoService,ProductoCatService productoCatService)
        {
            _productoService = productoService;
            _productoCatService = productoCatService;
        }


        // GET: Producto
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var Productos =await _productoService.GetAll();
            return View(Productos);
        }

        // GET: Producto/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var prod = await _productoService.FirstOrDefault(id);
            return View(prod);
        }

        // GET: Producto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Producto/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producto/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var prod = await _productoService.FirstOrDefault(id);
            var select = (await _productoCatService.GetAll()).Select(x=>new SelectListItem { Selected = prod.ProductoCatId ==x.ProductoCatId ,
            Disabled =false, 
            Text = x.Valor,
            Value = x.ProductoCatId.ToString()
            }).ToList();
            return View(new EditProductViewModel(prod, select));
        }

        // POST: Producto/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Producto p)
        {
            try
            {
                await _productoService.Update(p.ProductoId,p);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
       
        // GET: Producto/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Producto/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
