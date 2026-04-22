using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionesClientesETL.DATA.Entities.Dwh.Dimensions
{
    public class DimDtos
    {
            public string ProductosFile { get; set; }
            public string ClientesFile { get; set; }
            public string FuentesFile { get; set; }
            public string WebReviewsFile { get; set; } = string.Empty; 
            public string CategoriasFile { get; set; }
            public string FechasFile { get; set; }
            public string EncuestasFile { get; set; }
            public string SocialFile { get; set; }

    }
}
