using LibADO.SPS;
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
            int? pk_leitor = HttpContext.Session.GetInt32("UserId");

            if (pk_leitor.HasValue)
            {
                bool resultado = Procedures.sp_cancel_leitor(pk_leitor.Value,connectionstring);
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
            else
            {
                Mensagem = "Erro: Usuário não autenticado.";
                Sucesso = false;
            }
        }
    }
}
