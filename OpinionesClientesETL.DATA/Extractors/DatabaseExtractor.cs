using Microsoft.Data.SqlClient;
using OpinionesClientesETL.DATA.Entities.db;
using OpinionesClientesETL.DATA.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA
{
    public class DatabaseExtractor : IExtractor<Opinions>
    {
        private readonly string _connectionString;

        public DatabaseExtractor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Opinions>> ExtractAsync()
        {
            var lista = new List<Opinions>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = @"SELECT 
                    C.IDCOMMENT,
                    C.IDCLIENTE,
                    C.IDPRODUCTO,
                    C.FECHA,
                    C.COMENTARIO,
                    CL.NOMBRE_CLASIFICACION,
                    R.NUMERO_RATING,
                    F.NOMBREFUENTE
                FROM COMENTARIOS C
                INNER JOIN CLASIFICACION CL ON C.IDCLASIFICACION = CL.IDCLASIFICACION
                INNER JOIN RATING R ON C.IDRATING = R.IDRATING
                INNER JOIN FUENTES F ON C.IDFUENTE = F.IDFUENTE";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Opinions
                        {
                            IdOpinion = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                            IdCliente = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            IdProducto = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                            Fecha = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                            Comentario = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Clasificación = reader.IsDBNull(5) ? null : reader.GetString(5),
                            PuntajeSatisfacción = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                            Fuente = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }
            }

            return lista;
        }
    }
}