using CsvHelper;
using CsvHelper.Configuration;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA
{
    public class CsvExtractor : IExtractor<Opinions>
    {
        private readonly string _filePath;

        public CsvExtractor(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<IEnumerable<Opinions>> ExtractAsync()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
           
                PrepareHeaderForMatch = args => args.Header.ToLower().Trim(),
     
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using (var reader = new StreamReader(_filePath))
            using (var csv = new CsvReader(reader, config))
            {

                var lista = new List<Opinions>();
                var records = csv.GetRecordsAsync<Opinions>();

                await foreach (var record in records)
                {
                    lista.Add(record);
                }

                return lista;
            }
        }
    }
}