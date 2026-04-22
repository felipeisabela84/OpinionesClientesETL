using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Services;

namespace OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh
{
    public interface IDwhRepository
    {
        Task<ServiceResult> LoadDimsDataAsync(DimDtos dimDtos);
    }
}
