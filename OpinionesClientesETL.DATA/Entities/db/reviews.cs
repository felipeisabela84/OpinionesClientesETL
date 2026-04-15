using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.db
{
    public class reviews
    {   public int? IdReview { get; set; }
        public int? IdCliente { get; set; }
        public int? IdProducto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }
        public string? Rating { get; set; }

    }
   
}
