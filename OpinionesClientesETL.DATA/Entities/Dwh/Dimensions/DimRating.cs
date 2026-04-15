using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    
    public class DimRating
    {
        [Ignore]
        public int? IDRATING { get; set; }

        [Name("Rating")]     
        public int? NUMERO_RATING { get; set; }
    }
}
