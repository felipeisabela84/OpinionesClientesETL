using OpinionesClientesETL.DATA;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces;
using System.Linq;

namespace OpinionesClientesETL.WK
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando proceso ETL...");

            var extractores = new List<IExtractor<Opinions>>
    {
        new CsvExtractor(@"D:\Sources\surveys_part1.csv"),
        new DatabaseExtractor("Data Source=DESKTOP-1KKHQPG;Initial Catalog=PROYECTO_ETL;Integrated " +
                                "Security=True;TrustServerCertificate=True;"),
        new ApiExtractor(new HttpClient())
    };

            var staging = new List<Opinions>();

            foreach (var extractor in extractores)
            {
                var data = await extractor.ExtractAsync();

                staging.AddRange(data);

                _logger.LogInformation("Registros extraídos: {count}", data.Count());
            }

            _logger.LogInformation("Total en staging: {total}", staging.Count);

            _logger.LogInformation("Proceso ETL finalizado");
        }
    }
}
