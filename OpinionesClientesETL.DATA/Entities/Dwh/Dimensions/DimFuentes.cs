using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimFuentes
    {
        [Ignore]
        public int IDFUENTE { get; set; }

        [Name("TipoFuente")]
        public string? TIPOFUENTE { get; set; }
  
       

    }
}
