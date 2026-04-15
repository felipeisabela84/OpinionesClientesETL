using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimClasificacion
    {
        [Ignore]
        public int? IDCLASIFICACION { get; set; }
        public string? NOMBRE_CLASIFICACION { get; set; }
    }
}
