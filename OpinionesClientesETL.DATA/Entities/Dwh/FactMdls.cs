using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh
{
    public class FactMdls
    {
        
        public class SurveyOpinion
        {
            public int? IdOpinion { get; set; }
            public int? IdCliente { get; set; }
            public int? IdProducto { get; set; }
            public DateTime? Fecha { get; set; }
            public string? Comentario { get; set; }
            public string? Clasificación { get; set; }
            public int? PuntajeSatisfacción { get; set; }
            public string? Fuente { get; set; }
        }

        public class WebReview
        {
            public string? IdReview { get; set; }
            public string? IdCliente { get; set; }
            public string? IdProducto { get; set; }
            public DateTime? Fecha { get; set; }
            public string? Comentario { get; set; }
            public int? Rating { get; set; }
        }

       
        public class SocialComment
        {
            public string? IdComment { get; set; }
            public string? IdCliente { get; set; }
            public string? IdProducto { get; set; }
            public string? Fuente { get; set; }
            public DateTime? Fecha { get; set; }
            public string? Comentario { get; set; }
        }
    }
}
