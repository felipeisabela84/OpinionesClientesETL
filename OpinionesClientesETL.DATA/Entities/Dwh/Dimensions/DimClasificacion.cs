using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;


namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{   [Table("CLASIFICACION", Schema = "DIM")]
    public class DimClasificacion
    {
        [Ignore]
        public int IDCLASIFICACION { get; set; }
        [Name("Clasificación")]
        public string? NOMBRE_CLASIFICACION { get; set; }
    }
}
