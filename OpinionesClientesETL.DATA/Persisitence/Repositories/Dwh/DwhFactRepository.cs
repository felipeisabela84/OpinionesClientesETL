using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionesClientesETL.DATA.Entities.Dwh;
using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Entities.Dwh.Facts;
using OpinionesClientesETL.DATA.Interfaces;
using System.Data;
using static OpinionesClientesETL.DATA.Entities.Dwh.FactMdls;

namespace OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh
{
    public class DwhFactRepository
    {
        private readonly DWHInventoryContext _context;
        private readonly ILogger<DwhFactRepository> _logger;
        private readonly ICsvFileReaderRepository _csvReader;

        public DwhFactRepository(
            DWHInventoryContext context,
            ILogger<DwhFactRepository> logger,
            ICsvFileReaderRepository csvReader)
        {
            _context = context;
            _logger = logger;
            _csvReader = csvReader;
        }

        public async Task ClearFactsAsync()
        {
            try
            {
                _logger.LogInformation("Limpiando FACT.OPINIONES...");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM FACT.OPINIONES");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('FACT.OPINIONES', RESEED, 0)");
                _logger.LogInformation("FACT limpia correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error limpiando FACT");
                throw;
            }
        }

        public async Task LoadFactOpinionesAsync(DimDtos dimDtos)
        {
            try
            {
                _context.ChangeTracker.Clear();
                _logger.LogInformation("Leyendo fuentes CSV...");

                var surveys = await _csvReader.ReadFileAsync<SurveyOpinion>(dimDtos.EncuestasFile);
                var webReviews = await _csvReader.ReadFileAsync<WebReview>(dimDtos.WebReviewsFile);
                var socialCmts = await _csvReader.ReadFileAsync<SocialComment>(dimDtos.SocialFile);

            
                var unificado = surveys.Select(o => new UnifiedOpinion
                {
                    IdCliente = o.IdCliente?.ToString(),
                    IdProducto = o.IdProducto?.ToString(),
                    Fuente = o.Fuente,
                    Clasificación = o.Clasificación,
                    PuntajeSatisfacción = o.PuntajeSatisfacción,
                    Fecha = o.Fecha?.ToString("yyyy-MM-dd")
                })
                .Concat(webReviews.Select(o => new UnifiedOpinion
                {
                    IdCliente = o.IdCliente,
                    IdProducto = o.IdProducto,
                    Fuente = "WEB",
                    Clasificación = null,
                    PuntajeSatisfacción = o.Rating,
                    Fecha = o.Fecha?.ToString("yyyy-MM-dd")
                }))
                .Concat(socialCmts.Select(o => new UnifiedOpinion
                {
                    IdCliente = o.IdCliente,
                    IdProducto = o.IdProducto,
                    Fuente = o.Fuente,
                    Clasificación = null,
                    PuntajeSatisfacción = null,
                    Fecha = o.Fecha?.ToString("yyyy-MM-dd")
                }))
                .ToList();

                _logger.LogInformation("Total unificado: {count}", unificado.Count);

             
                var unificadoInt = unificado
                    .Select(o =>
                    {
                        var clienteStr = o.IdCliente?.TrimStart('C', 'c', '0');
                        var productoStr = o.IdProducto?.TrimStart('P', 'p', '0');

                        if (!int.TryParse(clienteStr, out var idCliente)) return null;
                        if (!int.TryParse(productoStr, out var idProducto)) return null;

                        return new
                        {
                            IdCliente = idCliente,
                            IdProducto = idProducto,
                            o.Fuente,
                            o.Clasificación,
                            o.PuntajeSatisfacción,
                            o.Fecha
                        };
                    })
                    .Where(o => o != null)
                    .ToList();

                _logger.LogInformation("Total parseado: {count}", unificadoInt.Count);

           
                var dimClientes = await _context.DimClientes.AsNoTracking().ToListAsync();
                var dimProductos = await _context.DimProductos.AsNoTracking().ToListAsync();
                var dimFuentes = await _context.DimFuentes.AsNoTracking().ToListAsync();
                var dimFechas = await _context.DimFechas.AsNoTracking().ToListAsync();
                var dimRatings = await _context.DimRatings.AsNoTracking().ToListAsync();
                var dimClasificaciones = await _context.DimClasificaciones.AsNoTracking().ToListAsync();

                _logger.LogInformation("DIMs - Clientes:{c} Productos:{p} Fuentes:{f} Fechas:{fe} Ratings:{r} Clases:{cl}",
                    dimClientes.Count, dimProductos.Count, dimFuentes.Count,
                    dimFechas.Count, dimRatings.Count, dimClasificaciones.Count);

           
                var fact = from o in unificadoInt
                           join c in dimClientes on o.IdCliente equals c.IDCLIENTE
                           join p in dimProductos on o.IdProducto equals p.IDPRODUCTO
                           join f in dimFuentes on o.Fuente equals f.TIPOFUENTE
                           join fe in dimFechas on o.Fecha equals fe.FECHA_DT?.ToString("yyyy-MM-dd")
                           join r in dimRatings on o.PuntajeSatisfacción equals r.NUMERO_RATING into ratings
                           from r in ratings.DefaultIfEmpty()
                           join cl in dimClasificaciones on o.Clasificación equals cl.NOMBRE_CLASIFICACION into clases
                           from cl in clases.DefaultIfEmpty()

                           select new FactOpiniones
                           {
                               IDCLIENTE = c.IDCLIENTE,
                               IDPRODUCTO = p.IDPRODUCTO,
                               IDFUENTE = f.IDFUENTE,
                               IDFECHA = fe.IDFECHA,
                               IDRATING = r != null ? r.IDRATING : (int?)null,
                               IDCLASIFICACION = cl?.IDCLASIFICACION ?? 1,
                               TOTAL_COMENTARIOS = 1
                           };

                var factList = fact.ToList();
                _logger.LogInformation("Facts a insertar: {count}", factList.Count);

                if (factList.Count == 0)
                {
                    _logger.LogWarning("No hay facts para insertar.");
                    return;
                }

           
                var dataTable = new DataTable();
                dataTable.Columns.Add("IDCLIENTE", typeof(int));
                dataTable.Columns.Add("IDPRODUCTO", typeof(int));
                dataTable.Columns.Add("IDFUENTE", typeof(int));
                dataTable.Columns.Add("IDFECHA", typeof(int));
                dataTable.Columns.Add("IDRATING", typeof(int));
                dataTable.Columns.Add("IDCLASIFICACION", typeof(int));
                dataTable.Columns.Add("TOTAL_COMENTARIOS", typeof(int));

                foreach (var f in factList)
                {
                    dataTable.Rows.Add(
                        f.IDCLIENTE,
                        f.IDPRODUCTO,
                        f.IDFUENTE,
                        f.IDFECHA,
                        f.IDRATING as object ?? DBNull.Value,
                        f.IDCLASIFICACION as object ?? DBNull.Value,
                        f.TOTAL_COMENTARIOS);
                }

                var connection = (SqlConnection)_context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "FACT.OPINIONES",
                    BatchSize = 5000
                };

                bulkCopy.ColumnMappings.Add("IDCLIENTE", "IDCLIENTE");
                bulkCopy.ColumnMappings.Add("IDPRODUCTO", "IDPRODUCTO");
                bulkCopy.ColumnMappings.Add("IDFUENTE", "IDFUENTE");
                bulkCopy.ColumnMappings.Add("IDFECHA", "IDFECHA");
                bulkCopy.ColumnMappings.Add("IDRATING", "IDRATING");
                bulkCopy.ColumnMappings.Add("IDCLASIFICACION", "IDCLASIFICACION");
                bulkCopy.ColumnMappings.Add("TOTAL_COMENTARIOS", "TOTAL_COMENTARIOS");

                await bulkCopy.WriteToServerAsync(dataTable);

                _logger.LogInformation("FACT cargada correctamente con BulkCopy.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cargando FACT");
                throw;
            }
        }
    }
}