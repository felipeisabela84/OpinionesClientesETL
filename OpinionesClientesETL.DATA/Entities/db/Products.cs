using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.db
{
    public class Products
    {
        public int? IdProducts { get; set; }
        public string? Nombre { get; set; }
        public string? Categoría { get; set; }
    }
}
