using LibADO.CancelAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserMPA.Pages
{
    public class CancelarADModel : PageModel
    {
        private readonly string _connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";

        public string Mensagem { get; set; } = string.Empty;
        public bool Sucesso { get; set; }

        public IActionResult OnGet()
        {
            int? pkLeitor = HttpContext.Session.GetInt32("PkLeitor");

            if (pkLeitor == null)
            {
                Console.WriteLine(" ERRO: PkLeitor n�o encontrado na sess�o. Redirecionando para Index.");
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"PkLeitor encontrado na sess�o: {pkLeitor.Value}");

            try
            {
                bool cancelado = Method.sp_cancel_leitor(pkLeitor.Value, _connectionString);
                if (cancelado)
                {
                    HttpContext.Session.Clear();
                    Sucesso = true;
                    Mensagem = " Ades�o cancelada com sucesso.";
                }
                else
                {
                    Sucesso = false;
                    Mensagem = " Falha ao cancelar ades�o.";
                }
            }
            catch (Exception ex)
            {
                Sucesso = false;
                Mensagem = $" Erro ao cancelar ades�o: {ex.Message}";
            }

            return Page();
        }
    }
}
