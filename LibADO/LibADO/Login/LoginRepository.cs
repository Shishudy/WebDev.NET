using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibADO.Login;

namespace LibADO
{

    public interface LoginRepository
    {
        bool ValidarLogin(string email, string senha);
        LoginModel ObterUsuarioPorEmail(string email, string senha);
    }


    public class LoginRepositoryADO : LoginRepository
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
                string query = "SELECT COUNT(*) FROM Leitor WHERE email = @Email AND loggin_password = @Senha";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public LoginModel ObterUsuarioPorEmail(string email, string senha)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT email, loggin_password FROM Leitor WHERE email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LoginModel
                            {
                                Email = reader["email"].ToString(),
                                Senha = reader["loggin_password"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
