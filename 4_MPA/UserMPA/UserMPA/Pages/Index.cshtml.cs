using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibADO.Login;

namespace UserMPA.Pages
{
    public class IndexModel : PageModel
    {
        private readonly LoginService _loginService;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Senha { get; set; }

        public string MensagemErro { get; set; }

        public IndexModel(LoginService loginService)
        {
            _loginService = loginService;
        }

        public IActionResult OnPost()
        {
            string resultadoLogin = _loginService.TentarLogin(Email, Senha);

            if (resultadoLogin == "OK")
            {
                var usuario = _loginService.ObterUsuario(Email);
                if (usuario != null)
                {
                    HttpContext.Session.SetString("NomeUsuario", usuario.Nome);
                    HttpContext.Session.SetInt32("PkLeitor", usuario.Id); 
                    Console.WriteLine($" PkLeitor salvo na sessão: {usuario.Id}");
                    return RedirectToPage("/Index");
                }
            }

            MensagemErro = resultadoLogin;
            return Page();
        }
    }
}
