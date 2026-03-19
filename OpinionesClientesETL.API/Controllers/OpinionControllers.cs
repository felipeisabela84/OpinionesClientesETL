using Microsoft.AspNetCore.Mvc;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces; 

namespace OpinionesClientesETL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionsController : ControllerBase
    {
        private readonly IExtractor<Opinions> _extractor;

        public OpinionsController(IExtractor<Opinions> extractor)
        {
            _extractor = extractor;
        }

        [HttpGet("extraer-csv")]
        public async Task<IActionResult> GetOpinionsFromCsv()
        {
 
            var opiniones = await _extractor.ExtractAsync();
            return Ok(opiniones);
        }
    }
}