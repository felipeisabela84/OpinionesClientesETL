using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimFecha
    {
        [Ignore]
        public int IDFECHA { get; set; }
        [Name("Fecha")]
        public string FECHA { get; set; }
        [Ignore]
        public int MES { get; set; }
        [Ignore]
        public int TRIMESTRE { get; set; }
        [Ignore]
        public int ANO { get; set; }
    }
}
