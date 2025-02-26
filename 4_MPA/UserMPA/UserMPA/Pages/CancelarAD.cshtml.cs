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
            Console.WriteLine($"DEBUG: ID do usu�rio na sess�o -> {userId}");

            if (userId.HasValue)
            {
                bool resultado = Method.sp_cancel_leitor(userId.Value, connectionstring);
                Console.WriteLine($"DEBUG: Resultado de sp_cancel_leitor -> {resultado}");

                if (resultado)
                {
                    Mensagem = "Ades�o cancelada com sucesso!";
                    Sucesso = true;

                    // Opcional: Remover sess�o ap�s cancelamento
                    HttpContext.Session.Clear();
                }
                else
                {
                    Mensagem = "Erro ao cancelar a ades�o. Tente novamente";
                    Sucesso = false;
                }
            }
            else
            {
                Mensagem = "Erro: Usu�rio n�o autenticado.";
                Sucesso = false;
            }
        }

    }
}