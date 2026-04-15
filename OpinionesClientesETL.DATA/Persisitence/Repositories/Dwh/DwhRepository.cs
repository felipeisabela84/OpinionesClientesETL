using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Interfaces;
using OpinionesClientesETL.DATA.Persisitence.Repositories.Csv;


namespace OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh
{
    public class DwhRepository : IDwhRepository
    {
        private readonly DWHInventoryContext _dWHInventoryContext;
        private readonly ICsvFileReaderRepository _csvInventoryFileReaderRepository;
        private readonly ILogger<DWHInventoryContext> _logger;

        public DwhRepository(
            DWHInventoryContext dWHInventoryContext,
            ICsvFileReaderRepository csvInventoryFileReaderRepository,
            ILogger<DWHInventoryContext> logger)
        {
            _dWHInventoryContext = dWHInventoryContext;
            _csvInventoryFileReaderRepository = csvInventoryFileReaderRepository;
            _logger = logger;
        }

        public async Task<ServiceResult> LoadDimsDataAsync(DimDtos dimDtos)
        {
            using var transaction = await _dWHInventoryContext.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando carga de dimensiones...");


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
                    .Select(x => x.IDPRODUCTO)
                    .ToListAsync();

                dimProducts = dimProducts
                    .Where(p => !existingProducts.Contains(p.IDPRODUCTO))
                    .ToList();

                await _dWHInventoryContext.DimProductos.AddRangeAsync(dimProducts);


                var categories = products
                    .Select(p => p.CATEGORIA?.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .Select(c => new DimCategoria { CATEGORIA = c })
                    .ToList();

                var existingCategories = await _dWHInventoryContext.DimCategorias
                    .Select(x => x.CATEGORIA)
                    .ToListAsync();

                categories = categories
                    .Where(c => !existingCategories.Contains(c.CATEGORIA))
                    .ToList();

                await _dWHInventoryContext.DimCategorias.AddRangeAsync(categories);


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
                    .Select(x => x.IDCLIENTE)
                    .ToListAsync();

                dimClients = dimClients
                    .Where(c => !existingClients.Contains(c.IDCLIENTE))
                    .ToList();

                await _dWHInventoryContext.DimClientes.AddRangeAsync(dimClients);

                var fuentes = await _csvInventoryFileReaderRepository
    .ReadFileAsync<DimFuentes>(dimDtos.FuentesFile);

                var dimFuentes = fuentes
                    .Where(f => !string.IsNullOrEmpty(f.TIPOFUENTE))
                    .Select(f => new DimFuentes
                    {
                        TIPOFUENTE = f.TIPOFUENTE?.Trim()
                    })
                    .DistinctBy(f => f.TIPOFUENTE)
                    .ToList();

                var existingFuentes = await _dWHInventoryContext.DimFuentes
                    .Select(x => x.TIPOFUENTE)
                    .ToListAsync();

                dimFuentes = dimFuentes
                    .Where(f => !existingFuentes.Contains(f.TIPOFUENTE))
                    .ToList();

                await _dWHInventoryContext.DimFuentes.AddRangeAsync(dimFuentes);


                var webReviews = await _csvInventoryFileReaderRepository
                    .ReadFileAsync<DimRating>(dimDtos.WebReviewsFile);

                var dimRatings = webReviews
                    .Where(r => r.NUMERO_RATING.HasValue)
                    .Select(r => new DimRating
                    {
                        NUMERO_RATING = r.NUMERO_RATING
                    })
                    .DistinctBy(r => r.NUMERO_RATING)
                    .ToList();

                var existingRatings = await _dWHInventoryContext.DimRatings
                    .Select(x => x.NUMERO_RATING)
                    .ToListAsync();

                dimRatings = dimRatings
                    .Where(r => !existingRatings.Contains(r.NUMERO_RATING))
                    .ToList();

                await _dWHInventoryContext.DimRatings.AddRangeAsync(dimRatings);


                var fechas = await _csvInventoryFileReaderRepository
                .ReadFileAsync<DimFecha>(dimDtos.FechasFile);
           
                var maxId = await _dWHInventoryContext.DimFechas
                    .Select(x => (int?)x.IDFECHA)
                    .MaxAsync() ?? 0;

                int contador = maxId + 1;

                var dimFechas = fechas
                    .Select(f =>
                    {
                        if (!DateTime.TryParse(f.FECHA, out var parsed))
                            return null;

                        return new DimFecha
                        {
                            IDFECHA = contador++, 
                            FECHA = parsed,       
                            MES = parsed.Month,
                            TRIMESTRE = (parsed.Month - 1) / 3 + 1,
                            ANO = parsed.Year
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                var existing = (await _dWHInventoryContext.DimFechas
                    .Select(x => x.FECHA)
                    .ToListAsync())
                    .ToHashSet();

      
                dimFechas = dimFechas
                    .Where(x => !existing.Contains(x.FECHA))
                    .ToList();

                await _dWHInventoryContext.DimFechas.AddRangeAsync(dimFechas);

                await _dWHInventoryContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Dimensiones cargadas correctamente");
                return ServiceResult.Success("Dimensiones cargadas correctamente");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cargar dimensiones");
                return ServiceResult.Failure($"Error ETL dimensiones: {ex.Message}");
            }
        }
    }
}