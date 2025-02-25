using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibADO.SPS;

namespace UserMPA.Pages
{
    public class SearchModel : PageModel
    {
        public List<Dictionary<string, object>> Obras { get; set; } = new List<Dictionary<string, object>>();
        private readonly string connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";
        public void OnGet(string? obra, string? genre, string? nucleo)
        {
            Obras = Procedures.sp_search_obras_com_imagem(obra, genre, nucleo, connectionString);
        }
    }
    
}
