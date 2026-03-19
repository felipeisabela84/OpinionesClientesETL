using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA
{
    public interface IFileReaderRepository<T> where T : class
    {
        Task<IEnumerable<T>> ReadFileAsync(string filePath);
    }
}