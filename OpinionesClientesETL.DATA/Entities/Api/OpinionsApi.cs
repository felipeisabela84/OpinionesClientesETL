using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Api
{
    public class OpinionsApi
    {
        
            public string IdOpinion { get; set; }
            public string IdCliente { get; set; }
            public string IdProducto { get; set; }
            public DateTime? Fecha { get; set; }
            public string Comentario { get; set; }
            public string Clasificación { get; set; }
            public int  PuntajeSatisfacción { get; set; }
            public string Fuente { get; set; }
        
    }

}

