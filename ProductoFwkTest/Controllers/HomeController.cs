using ProductoFwkTest.App_Start;
using ProductoFwkTest.Repository;
using ProductoFwkTest.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductoFwkTest.Controllers
{
    public class HomeController : Controller
    {
        private ProductoService _productoService;

        public HomeController(ProductoService productoService)
        {
            _productoService = productoService;
        }
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
          

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}