using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Interfaces
{
    public interface ICsvFileReaderRepository
    {
        Task<List<T>> ReadFileAsync<T>(string filePath);
    }
}
