using LibADO.CancelAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace UserMPA.Pages
{
    public class CancelarADModel : PageModel
    {
        private readonly string _connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";

        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }

        public IActionResult OnGet()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                bool cancelado = Method.sp_cancel_leitor(idUsuario.Value, _connectionString);
                if (cancelado)
                {
                    HttpContext.Session.Clear(); 
                    Sucesso = true;
                    Mensagem = "Adesão cancelada com sucesso.";
                }
            }
            catch (Exception ex)
            {
                Sucesso = false;
                Mensagem = ex.Message; 
            }

            return Page();
        }
    }
}