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
        public void OnGet(string? obra, string? genre, string? nucleo)
        {
            MethodesSearch searchMethods = new();
            Categorias = searchMethods.ObterCategorias(connectionString);
            Obras = SearchSP.sp_search_obras_com_imagem(obra, genre, nucleo, connectionString);
        }


    }

}
