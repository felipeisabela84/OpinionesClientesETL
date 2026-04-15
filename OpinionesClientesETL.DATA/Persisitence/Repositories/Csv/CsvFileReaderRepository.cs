using CsvHelper;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Persisitence.Repositories.Csv
{

    public class CsvFileReaderRepository : ICsvFileReaderRepository
    {
        public async Task<List<T>> ReadFileAsync<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<T>();
            return await Task.FromResult(records.ToList());
        }
    }

}
