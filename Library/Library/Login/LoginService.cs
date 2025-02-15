using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libDeTeste
{
    public class LoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)  
        {
            _loginRepository = loginRepository;
        }

        public bool ValidarLogin(string email, string senha)
        {
            return _loginRepository.ValidarLogin(email, senha);
        }

        public LoginModel ObterUsuario(string email)
        {
            return _loginRepository.ObterUsuarioPorEmail(email);
        }
    }
}
