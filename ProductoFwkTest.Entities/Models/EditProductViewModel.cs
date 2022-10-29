using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ProductoFwkTest.Entities.Models
{
    public class EditProductViewModel : Producto
    {
        public EditProductViewModel()
        {

        }
        public EditProductViewModel(Producto prod, List<SelectListItem>  sel)
        {
            this.Precio = prod.Precio;
            this.ProductoCatId = prod.ProductoCatId;
            this.Valor = prod.Valor;
            this.Activo = prod.Activo;
            this.Cantidad = prod.Cantidad;
            this.ProductoId = prod.ProductoId;
            this.SelectCat = sel;

        }
        public EditProductViewModel(Producto prod)
        {
            this.Precio = prod.Precio;
            this.ProductoCatId = prod.ProductoCatId;
            this.Valor = prod.Valor;
            this.Activo = prod.Activo;
            this.Cantidad = prod.Cantidad;
            this.ProductoId = prod.ProductoId;


        }
        public List<SelectListItem> SelectCat { get; set; }

        public Producto ToProducto()
        {
            int val = 0;
            int.TryParse(this.SelectCat.FirstOrDefault(x => x.Selected).Value, out val);
            return new Producto
            {
                Activo = this.Activo,
                Cantidad = this.Cantidad,
                Precio = this.Precio,
                ProductoCatId = val,
                ProductoId = this.ProductoId,
                Valor = this.Valor
            };
        }
    }
}
