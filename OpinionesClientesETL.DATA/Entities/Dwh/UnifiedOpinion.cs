using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh
{
    public class UnifiedOpinion
    {
            public string? IdCliente { get; set; }
            public string? IdProducto { get; set; }
            public string? Fuente { get; set; }
            public string? Clasificación { get; set; }
            public int? PuntajeSatisfacción { get; set; }
            public string? Fecha { get; set; }
        
    }
}
