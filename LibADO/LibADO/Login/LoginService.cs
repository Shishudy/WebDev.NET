using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibADO.Login;
using LibADO;

namespace LibADO
{
    public class LoginService
    {
        private readonly LoginRepository _loginRepository;

        public LoginService(LoginRepository loginRepository) => _loginRepository = loginRepository;

        public bool ValidarLogin(string email, string senha) => _loginRepository.ValidarLogin(email, senha);

        public LoginModel? ObterUsuario(string email) => _loginRepository.ObterUsuarioPorEmail(email);
    }
}
}

