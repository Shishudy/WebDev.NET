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
            if (_loginService.ValidarLogin(Email, Senha))
            {
                var usuario = _loginService.ObterUsuario(Email);
                if (usuario != null)
                {
                    HttpContext.Session.SetString("NomeUsuario", usuario.Nome);
                    return RedirectToPage("/Index");
                }
            }

            MensagemErro = "Usuário ou senha inválidos.";
            return Page();
        }
    }
}
