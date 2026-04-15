using OpinionesClientesETL.DATA.Entities.Api;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA
{
    public class ApiExtractor : IExtractor<Opinions>
    {
        private readonly HttpClient _httpClient;

        public ApiExtractor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Opinions>> ExtractAsync()
        {
            var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/comments");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var apiData = JsonSerializer.Deserialize<List<ApiComments>>(json);

            var lista = new List<Opinions>();

            foreach (var item in apiData)
            {
                lista.Add(new Opinions
                {
                    IdOpinion = item.id,
                    Comentario = item.body,
                    Fecha = DateTime.Now,
                    Fuente = "API",
                    Clasificación = "Externa",
                    PuntajeSatisfacción = null,
                    IdCliente = null,
                    IdProducto = null
                });
            }

            return lista;
        }
    }
}