using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libDeTeste
{

    public interface ILoginRepository
    {
        bool ValidarLogin(string email, string senha);
        LoginModel ObterUsuarioPorEmail(string email);

    }

    /*

    exemplo em ADO net

    public class LoginRepositoryADO : ILoginRepository
    {
        private readonly string _connectionString;

        public LoginRepositoryADO(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool ValidarLogin(string email, string senha)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Leitor WHERE email = @Email AND login_password = @Senha";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        } */

    }

