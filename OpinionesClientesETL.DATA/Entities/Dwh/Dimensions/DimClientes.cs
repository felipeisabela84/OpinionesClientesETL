using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimClientes
    {
        [Name("IdCliente")]
        public int IDCLIENTE { get; set; }
        [Name("Nombre")]
        public string? NOMBRE { get; set; }
        [Name("Email")]
        public string? EMAIL { get; set; }

    }
}
