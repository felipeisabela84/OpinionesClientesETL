using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.db
{
    public class Sources
    {
        public string? IdFuentes { get; set; }
        public string? TipoFuente { get; set; }
        public DateTime? FechaCarga { get; set; }
    }
}
