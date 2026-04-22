using Microsoft.EntityFrameworkCore;
using OpinionesClientesETL.DATA.Interfaces;
using OpinionesClientesETL.DATA.Persisitence;
using OpinionesClientesETL.DATA.Persisitence.Repositories.Csv;
using OpinionesClientesETL.DATA.Persisitence.Repositories.Dwh;
using OpinionesClientesETL.WK;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddScoped<IDwhRepository, DwhDimRepository>();
builder.Services.AddScoped<ICsvFileReaderRepository, CsvFileReaderRepository>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<DwhFactRepository>();
builder.Services.AddDbContext<DWHInventoryContext>(options =>
    options.UseSqlServer("Server=.;Database=ANALISIS_OPINIONES;Trusted_Connection=True;TrustServerCertificate=True;"));


var host = builder.Build();
host.Run();