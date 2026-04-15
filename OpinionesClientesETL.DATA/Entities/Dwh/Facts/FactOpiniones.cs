using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Facts
{
    public class FactOpiniones
    {
        public int IDOPINION { get; set; }
        public int IDCLIENTE { get; set; }
        public int IDPRODUCTO { get; set; }
        public int IDFUENTE { get; set; }
        public int IDCLASIFICACION { get; set; }
        public int IDRATING { get; set; }
        public int IDFECHA { get; set; }
        public int TOTAL_COMENTARIOS { get; set; }
    }
}
