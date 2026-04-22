using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Interfaces;
using OpinionesClientesETL.DATA.Services;
using static OpinionesClientesETL.DATA.Entities.Dwh.FactMdls;

namespace OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh
{
    public class DwhDimRepository : IDwhRepository
    {
        private readonly DWHInventoryContext _dWHInventoryContext;
        private readonly ICsvFileReaderRepository _csvInventoryFileReaderRepository;
        private readonly ILogger<DWHInventoryContext> _logger;

        public DwhDimRepository(
            DWHInventoryContext dWHInventoryContext,
            ICsvFileReaderRepository csvInventoryFileReaderRepository,
            ILogger<DWHInventoryContext> logger)
        {
            _dWHInventoryContext = dWHInventoryContext;
            _csvInventoryFileReaderRepository = csvInventoryFileReaderRepository;
            _logger = logger;
        }

        private async Task CleanTableAsync(string table)
        {
            await _dWHInventoryContext.Database.ExecuteSqlRawAsync($"DELETE FROM {table}");
            try
            {
                await _dWHInventoryContext.Database.ExecuteSqlRawAsync(
                    $"DBCC CHECKIDENT ('{table}', RESEED, 0)");
            }
            catch (Microsoft.Data.SqlClient.SqlException) { }
        }

        public async Task<ServiceResult> LoadDimsDataAsync(DimDtos dimDtos)
        {

            await CleanTableAsync("DIM.CATEGORIA");
            await CleanTableAsync("DIM.PRODUCTOS");
            await CleanTableAsync("DIM.CLIENTES");
            await CleanTableAsync("DIM.FUENTES");
            await CleanTableAsync("DIM.RATING");
            await CleanTableAsync("DIM.FECHA");
            await CleanTableAsync("DIM.CLASIFICACION");

            using var transaction = await _dWHInventoryContext.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando carga de dimensiones...");

                // PRODUCTOS
                var products = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<DimProductos>(dimDtos.ProductosFile);

                var dimProducts = products
                    .Where(p => !string.IsNullOrEmpty(p.NOMBRE))
                    .Select(p => new DimProductos
                    {
                        IDPRODUCTO = p.IDPRODUCTO,
                        NOMBRE = p.NOMBRE?.Trim(),
                        CATEGORIA = p.CATEGORIA?.Trim()
                    })
                    .DistinctBy(p => p.IDPRODUCTO)
                    .ToList();

                var existingProducts = await _dWHInventoryContext.DimProductos
                    .Select(x => x.IDPRODUCTO).ToListAsync();

                dimProducts = dimProducts
                    .Where(p => !existingProducts.Contains(p.IDPRODUCTO)).ToList();

                await _dWHInventoryContext.DimProductos.AddRangeAsync(dimProducts);

                // CATEGORIAS
                var categories = products
                    .Select(p => p.CATEGORIA?.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .Select(c => new DimCategoria { CATEGORIA = c })
                    .ToList();

                var existingCategories = await _dWHInventoryContext.DimCategorias
                    .Select(x => x.CATEGORIA).ToListAsync();

                categories = categories
                    .Where(c => !existingCategories.Contains(c.CATEGORIA)).ToList();

                await _dWHInventoryContext.DimCategorias.AddRangeAsync(categories);

                // CLIENTES
                var clients = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<DimClientes>(dimDtos.ClientesFile);

                var dimClients = clients
                    .Where(c => !string.IsNullOrEmpty(c.NOMBRE))
                    .Select(c => new DimClientes
                    {
                        IDCLIENTE = c.IDCLIENTE,
                        NOMBRE = c.NOMBRE?.Trim(),
                        EMAIL = c.EMAIL?.Trim()
                    })
                    .DistinctBy(c => c.IDCLIENTE)
                    .ToList();

                var existingClients = await _dWHInventoryContext.DimClientes
                    .Select(x => x.IDCLIENTE).ToListAsync();

                dimClients = dimClients
                    .Where(c => !existingClients.Contains(c.IDCLIENTE)).ToList();

                await _dWHInventoryContext.DimClientes.AddRangeAsync(dimClients);

            
                var fuentesFile = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<DimFuentes>(dimDtos.FuentesFile);
                var encuestasParaFuentes = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<SurveyOpinion>(dimDtos.EncuestasFile);
                var webParaFuentes = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<WebReview>(dimDtos.WebReviewsFile);
                var socialParaFuentes = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<SocialComment>(dimDtos.SocialFile);

                var existingFuentes = await _dWHInventoryContext.DimFuentes
                    .Select(x => x.TIPOFUENTE).ToListAsync();

                var todasFuentes = fuentesFile.Select(f => f.TIPOFUENTE?.Trim())
                    .Concat(encuestasParaFuentes.Select(o => o.Fuente?.Trim()))
                    .Concat(webParaFuentes.Select(o => "WEB"))
                    .Concat(socialParaFuentes.Select(o => o.Fuente?.Trim()))
                    .Where(f => !string.IsNullOrEmpty(f))
                    .Distinct()
                    .Where(f => !existingFuentes.Contains(f))
                    .Select(f => new DimFuentes { TIPOFUENTE = f })
                    .ToList();

                await _dWHInventoryContext.DimFuentes.AddRangeAsync(todasFuentes);

                // RATING
                var webReviews = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<DimRating>(dimDtos.WebReviewsFile);

                var dimRatings = webReviews
                    .Where(r => r.NUMERO_RATING.HasValue)
                    .Select(r => new DimRating { NUMERO_RATING = r.NUMERO_RATING })
                    .DistinctBy(r => r.NUMERO_RATING)
                    .ToList();

                var existingRatings = await _dWHInventoryContext.DimRatings
                    .Select(x => x.NUMERO_RATING).ToListAsync();

                dimRatings = dimRatings
                    .Where(r => !existingRatings.Contains(r.NUMERO_RATING)).ToList();

                await _dWHInventoryContext.DimRatings.AddRangeAsync(dimRatings);

                // CLASIFICACIONES
                var encuestas = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<Opinions>(dimDtos.EncuestasFile);

                var validClasificaciones = encuestas
                    .Where(o => !string.IsNullOrEmpty(o.Clasificación))
                    .Select(o => new DimClasificacion
                    {
                        NOMBRE_CLASIFICACION = o.Clasificación?.Trim()
                    })
                    .DistinctBy(x => x.NOMBRE_CLASIFICACION)
                    .ToList();

                var existingClasificaciones = await _dWHInventoryContext.DimClasificaciones
                    .Select(x => x.NOMBRE_CLASIFICACION).ToListAsync();

                validClasificaciones = validClasificaciones
                    .Where(x => !existingClasificaciones.Contains(x.NOMBRE_CLASIFICACION)).ToList();

                await _dWHInventoryContext.DimClasificaciones.AddRangeAsync(validClasificaciones);

                // FECHAS
                var fechas = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<DimFecha>(dimDtos.FechasFile);

                var dimFechas = fechas
                    .Where(f => f.FECHA_DT.HasValue)
                    .Select(f => new DimFecha
                    {
                        FECHA_DT = f.FECHA_DT,
                        MES = f.FECHA_DT!.Value.Month,
                        TRIMESTRE = (f.FECHA_DT.Value.Month - 1) / 3 + 1,
                        ANO = f.FECHA_DT.Value.Year
                    })
                    .DistinctBy(f => f.FECHA_DT)
                    .ToList();

                var existingFechas = await _dWHInventoryContext.DimFechas
                    .Select(x => x.FECHA).ToListAsync();

                dimFechas = dimFechas
                    .Where(x => !existingFechas.Contains(x.FECHA)).ToList();

                await _dWHInventoryContext.DimFechas.AddRangeAsync(dimFechas);

                await _dWHInventoryContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Dimensiones cargadas correctamente");
                return ServiceResult.Success("Dimensiones cargadas correctamente");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error al cargar dimensiones: {msg}", ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError("Inner: {msg}", ex.InnerException.Message);
                return ServiceResult.Failure($"Error ETL dimensiones: {ex.Message}");
            }
        }
    }
}