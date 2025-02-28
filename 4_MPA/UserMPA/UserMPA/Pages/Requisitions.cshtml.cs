using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using LibADO.UserRequisitions;

namespace UserMPA.Pages
{
    public class RequisitionsModel : PageModel
    {
        private readonly GetUserRequisitions _requisicaoRepository;

        public List<Dictionary<string, object>> ObrasRequisitadas { get; set; } = new();

        public RequisitionsModel()
        {
            string connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";
            _requisicaoRepository = new GetUserRequisitions(connectionString);
        }

        public IActionResult OnGet()
        {
            int? pkLeitor = HttpContext.Session.GetInt32("PkLeitor"); 

            if (pkLeitor == null)
            {
                Console.WriteLine(" ERRO: PkLeitor n�o encontrado na sess�o.");
                return RedirectToPage("/Index");
            }

            Console.WriteLine($" PkLeitor encontrado na sess�o: {pkLeitor.Value}");

            ObrasRequisitadas = _requisicaoRepository.GetObrasRequisitadas(pkLeitor.Value);

            Console.WriteLine($" Quantidade de obras retornadas: {ObrasRequisitadas.Count}");

            return Page();
        }
    }
}

