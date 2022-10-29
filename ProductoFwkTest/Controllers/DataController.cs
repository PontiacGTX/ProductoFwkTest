using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using ProductoFwkTest.Services;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPutAttribute = System.Web.Http.HttpPutAttribute;
using HttpDeleteAttribute = System.Web.Http.HttpDeleteAttribute;
using System.Web.Http.Cors;

namespace ProductoFwkTest.Controllers
{
    [System.Web.Http.RoutePrefix("api/Data")]
    [EnableCors(origins: "http://localhost:10466/", headers: "*", methods: "*")]
    public class DataController : ApiController
    {

        private ProductoService _productoService;
        public DataController(ProductoService productoService)
        {
            _productoService = productoService;
        }
        // GET: api/Data
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var data = await _productoService.GetAll();
            return Ok(data);
        }

        // GET: api/Data/5
        [HttpGet]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Data
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]object value)
        {
            var data = await _productoService.GetAll();
            return Ok(data);
        }

        // PUT: api/Data/5
        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Data/5
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
           var removed =await _productoService.Remove(id);
           return Ok(removed);
        }
    }
}
