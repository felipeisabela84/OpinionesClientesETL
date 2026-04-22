using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimFecha
    {
        [Ignore]
        public int IDFECHA { get; set; }

        // ✅ CsvHelper lee "Fecha" del CSV
        [Name("Fecha")]
        public DateTime? FECHA_DT { get; set; }

        // ✅ EF usa esto, CsvHelper lo ignora
        [Ignore]
        [NotMapped]
        public string? FECHA
        {
            get => FECHA_DT?.ToString("yyyy-MM-dd");
            set => FECHA_DT = DateTime.TryParse(value, out var d) ? d : null;
        }

        [Ignore]
        public int MES { get; set; }
        [Ignore]
        public int TRIMESTRE { get; set; }
        [Ignore]
        public int ANO { get; set; }
    }
}