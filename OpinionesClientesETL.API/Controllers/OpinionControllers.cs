using Microsoft.AspNetCore.Mvc;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces; // Asegúrate de tener esta referencia

namespace OpinionesClientesETL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionsController : ControllerBase
    {
        private readonly IExtractor<Opinions> _extractor;

        // Se recomienda usar IExtractor para ser coherente con tu arquitectura ETL
        public OpinionsController(IExtractor<Opinions> extractor)
        {
            _extractor = extractor;
        }

        [HttpGet("extraer-csv")]
        public async Task<IActionResult> GetOpinionsFromCsv()
        {
            // Nota: Asegúrate de que la ruta del archivo sea accesible por la API
            var opiniones = await _extractor.ExtractAsync();
            return Ok(opiniones);
        }
    }
}