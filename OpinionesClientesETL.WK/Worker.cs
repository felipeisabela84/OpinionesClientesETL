using Microsoft.Extensions.DependencyInjection;
using OpinionesClientesETL.DATA;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Entities.Dwh.Dimensions;
using OpinionesClientesETL.DATA.Interfaces;
using OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh;

namespace OpinionesClientesETL.WK
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;  

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;             
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando proceso ETL...");

            var extractores = new List<IExtractor<Opinions>>
            {
                new CsvExtractor<Opinions>(@"D:\Sources\surveys_part1.csv"),
                new DatabaseExtractor("Data Source=DESKTOP-1KKHQPG;Initial Catalog=PROYECTO_ETL;Integrated Security=True;TrustServerCertificate=True;"),
                new ApiExtractor(new HttpClient())
            };

            var staging = new List<Opinions>();

            foreach (var extractor in extractores)
            {
                var data = await extractor.ExtractAsync();
                staging.AddRange(data);
                _logger.LogInformation("Registros extraídos: {count}", data.Count());
            }

            _logger.LogInformation("STAGING TOTAL: {total}", staging.Count);

         
            using (var scope = _scopeFactory.CreateScope())
            {
                var dwhRepository = scope.ServiceProvider
                    .GetRequiredService<IDwhRepository>();

                var dimDtos = new DimDtos
                {
                    ProductosFile = @"D:\Sources\products.csv",
                    ClientesFile = @"D:\Sources\clients.csv",
                    FuentesFile = @"D:\Sources\Fuente_Datos.csv",
                    WebReviewsFile = @"D:\Sources\web_reviews.csv",
                    FechasFile = @"D:\Sources\surveys_part1.csv",
                };

                await dwhRepository.LoadDimsDataAsync(dimDtos);
            }

            _logger.LogInformation("DIMENSIONES CARGADAS");
            _logger.LogInformation("Proceso ETL finalizado");
        }
    }
}