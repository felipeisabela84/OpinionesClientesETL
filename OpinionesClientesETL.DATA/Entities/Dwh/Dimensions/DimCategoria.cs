using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimCategoria
    {
        public int? IDCATEGORIA { get; set; }
        [Name("Categoria")]
        public string? CATEGORIA { get; set; }
    }
}
