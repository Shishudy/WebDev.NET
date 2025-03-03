using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibADO.Search;
using LibADO.Search.LibADO.Search;

namespace UserMPA.Pages
{
    public class SearchModel : PageModel
    {
        public List<Dictionary<string, object>> Obras { get; set; } = new List<Dictionary<string, object>>();
        public List<string> Categorias { get; set; } = new();
        private readonly string connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";

        public int PaginaAtual { get; set; } = 1;
        public int TotalPaginas { get; set; }

        private const int ItensPorPagina = 6; 

        public void OnGet(string? obra, string? genre, int pagina = 1)
        {
            MethodesSearch searchMethods = new();
            Categorias = searchMethods.ObterCategorias(connectionString);

            PaginaAtual = pagina < 1 ? 1 : pagina;

            var todasObras = SearchSP.sp_search_obras_com_imagem(obra, genre, connectionString);
            TotalPaginas = (int)Math.Ceiling(todasObras.Count / (double)ItensPorPagina);
            Obras = todasObras.Skip((PaginaAtual - 1) * ItensPorPagina).Take(ItensPorPagina).ToList();
        }
    }

}
