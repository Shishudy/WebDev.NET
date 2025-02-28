using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;

namespace LibADO.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LibDB;

    namespace LibADO.Search
    {
        public class SearchSP
        {
            public static List<Dictionary<string, object>> sp_search_obras_com_imagem(
    string? obra, string? genre, string? nucleo, string connectionString)
            {
                using var cn = DB.Open(connectionString);

                string filtroObra = string.IsNullOrEmpty(obra) ? "" : $"AND no.pk_obra IN (SELECT pk_obra FROM dbo.fn_search_obras('{obra}'))";
                string filtroGenero = string.IsNullOrEmpty(genre) ? "" : $"AND no.pk_obra IN (SELECT pk_obra FROM dbo.fn_search_obras_genre('{genre}'))";
                string filtroNucleo = string.IsNullOrEmpty(nucleo) ? "" : $"AND no.pk_nucleo IN (SELECT pk_nucleo FROM dbo.fn_search_nucleo('{nucleo}'))";

                string query = $@"
    SELECT no.pk_obra, o.nome_obra, n.nome_nucleo, no.quantidade, 
           COALESCE(ir.image_path, '/images/default.jpg') AS image_path
    FROM dbo.NucleoObra no
        INNER JOIN dbo.Obra o ON no.pk_obra = o.pk_obra
        INNER JOIN dbo.Nucleo n ON no.pk_nucleo = n.pk_nucleo
        LEFT JOIN dbo.ImageReferences ir ON o.fk_imagem = ir.pk_image
    WHERE 1=1 {filtroObra} {filtroGenero} {filtroNucleo}";

                var dt = DB.GetSQLRead(cn, query);
                return DB.ToDictionary(dt);
            }

        }
    }


}

