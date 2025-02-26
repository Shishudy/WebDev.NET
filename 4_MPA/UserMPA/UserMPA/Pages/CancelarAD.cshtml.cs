using LibADO.CancelAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace UserMPA.Pages
{
    public class CancelarADModel : PageModel
    {
        private readonly string connectionstring = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";
        public string Mensagem { get; set; } = "";
        public bool Sucesso { get; set; } = false;


        public void OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("IdUsuario");
            if (userId.HasValue)
            {
                bool resultado = Method.sp_cancel_leitor(userId.Value, connectionstring);
                if (resultado)
                {

                    Mensagem = "Adesão cancelada com sucesso!";

                }
                else
                {
                    Mensagem = "Erro ao cancelar a adesão. Tente novamente";
                    Sucesso = false;

                }
            }
        }
    }
}