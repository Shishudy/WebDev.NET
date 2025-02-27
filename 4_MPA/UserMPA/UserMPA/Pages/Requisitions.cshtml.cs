using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using LibADO.UserRequisitions;

namespace UserMPA.Pages
{
    namespace UserMPA.Pages
    {
        public class RequisitionsModel : PageModel
        {
            private readonly GetUserRequisitions _requisicaoRepository;

            public List<Dictionary<string, object>> ObrasRequisitadas { get; set; }

            public RequisitionsModel()
            {
                string connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";

                _requisicaoRepository = new GetUserRequisitions(connectionString);
            }

            public IActionResult OnGet()
            {
                int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                if (idUsuario == null)
                {
                    Console.WriteLine("ID do Usuário não encontrado na sessão.");
                    return RedirectToPage("/Login");
                }

                Console.WriteLine($"ID do Usuário: {idUsuario.Value}");

                ObrasRequisitadas = _requisicaoRepository.GetObrasRequisitadas(idUsuario.Value);

                Console.WriteLine($"Quantidade de obras retornadas: {ObrasRequisitadas.Count}");

                return Page();
            }
        }
    }
}