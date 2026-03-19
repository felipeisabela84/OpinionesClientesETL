using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using OpinionesClientesETL.DATA.Entities.db;
using System.Globalization;
using System.IO;

namespace OpinionesClientesETL.DATA.Persisitence.Repositories.Csv
{

    public class CsvFileReaderRepository : IFileReaderRepository<Opinions>
    {
        public async Task<IEnumerable<Opinions>> ReadFileAsync(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Opinions>();
            return records.ToList();
        }
    }

}
