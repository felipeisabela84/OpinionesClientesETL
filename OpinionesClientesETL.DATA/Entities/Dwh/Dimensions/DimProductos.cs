using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
  
        public class DimProductos
        {
            [Name("IdProducto")]
            public int IDPRODUCTO { get; set; }

            [Name("Nombre")]
            public string? NOMBRE { get; set; }

            [Name("Categoria")]
            public string? CATEGORIA { get; set; }
        }
    
}
