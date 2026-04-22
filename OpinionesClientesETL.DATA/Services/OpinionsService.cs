using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Entities.Api;


namespace OpinionesClientesETL.DATA.Services
    
{
    public class OpinionsService
    {
        public async Task<List<OpinionsApi>> GetAllAsync()
        {
            var encuestas = (await new CsvExtractor<OpinionsApi>(
                @"D:\Sources\surveys_part1.csv"
            ).ExtractAsync()).ToList();

            encuestas.ForEach(x => x.Fuente = "ENCUESTA");

            var web = (await new CsvExtractor<OpinionsApi>(
                @"D:\Sources\web_reviews.csv"
            ).ExtractAsync()).ToList();

            web.ForEach(x => x.Fuente = "WEB");

            var redes = (await new CsvExtractor<OpinionsApi>(
                @"D:\Sources\social_comments.csv"
            ).ExtractAsync()).ToList();

            redes.ForEach(x => x.Fuente = "REDES");

            return encuestas
                .Concat(web)
                .Concat(redes)
                .ToList();
        }
    }
}