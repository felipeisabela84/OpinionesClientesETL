using Microsoft.AspNetCore.Mvc;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces;
using OpinionesClientesETL.DATA.Services;

namespace OpinionesClientesETL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionsController : ControllerBase
    {
        private readonly OpinionsService _service;

        public OpinionsController(OpinionsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Opinions>>> Get()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }
    }
}