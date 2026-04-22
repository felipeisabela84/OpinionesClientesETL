using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Interfaces;
using OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh;

namespace OpinionesClientesETL.WK
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Iniciando proceso ETL...");

                using var scope = _scopeFactory.CreateScope();
                var dwhRepository = scope.ServiceProvider.GetRequiredService<IDwhRepository>();
                var factRepository = scope.ServiceProvider.GetRequiredService<DwhFactRepository>();

                var dimDtos = new DimDtos
                {
                    ProductosFile = @"D:\Sources\products.csv",
                    ClientesFile = @"D:\Sources\clients.csv",
                    FuentesFile = @"D:\Sources\Fuente_Datos.csv",
                    FechasFile = @"D:\Sources\surveys_part1.csv",
                    EncuestasFile = @"D:\Sources\surveys_part1.csv",
                    WebReviewsFile = @"D:\Sources\web_reviews.csv",
                    SocialFile = @"D:\Sources\social_comments.csv"
                };

                _logger.LogInformation("Limpiando facts...");
                await factRepository.ClearFactsAsync();
                _logger.LogInformation("Facts limpiados ");

                _logger.LogInformation("Cargando dimensiones...");
                var result = await dwhRepository.LoadDimsDataAsync(dimDtos);
                if (!result.IsSuccess)
                {
                    _logger.LogError("Error cargando dimensiones: {msg}", result.Message);
                    return;
                }
                _logger.LogInformation("Dimensiones cargadas");

                _logger.LogInformation("Insertando facts...");
                await factRepository.LoadFactOpinionesAsync(dimDtos);

                _logger.LogInformation("Proceso ETL finalizado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR en el proceso ETL: {message}", ex.Message);
                await Task.Delay(15000, stoppingToken);
                throw;
            }
        }
    }
}