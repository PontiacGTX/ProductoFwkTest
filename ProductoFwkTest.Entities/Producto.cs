using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductoFwkTest.Entities
{
    [Table("Producto")]
    public class Producto: IEntity
    {
        [Key]
        public int ProductoId { get; set; }
        public string Valor { get; set; }
        public bool Activo { get; set; }
        public double Precio { get; set; }
        public int ProductoCatId { get; set; }
        public int Cantidad { get; set; }
    }
}
