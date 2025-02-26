using System.Data;
using Microsoft.Data.SqlClient;
using LibADO.Login;
using LibDB;
using System;

namespace LibADO.Login
{
    public interface LoginRepository
    {
        string ValidarLogin(string email, string senha);
        LoginModel ObterUsuarioPorEmail(string email);
    }

    public class LoginRepositoryADO : LoginRepository
    {
        private readonly string _connectionString;

        public LoginRepositoryADO(string connectionString) => _connectionString = connectionString;

        public string ValidarLogin(string email, string senha)
        {
            using var conn = DB.Open(_connectionString);

            string checkUserQuery = "SELECT stat FROM Leitor WHERE email = @Email";
            using (var checkCmd = new SqlCommand(checkUserQuery, conn))
            {
                checkCmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });

                var status = checkCmd.ExecuteScalar()?.ToString();

                if (status == "Inactive")
                {
                    return "Sua conta está inativa e não pode ser acessada.";
                }
            }

            string loginQuery = @"
                SELECT COUNT(*) 
                FROM Leitor 
                WHERE email = @Email 
                AND loggin_password = @Senha 
                AND stat = 'active'";

            using (var command = new SqlCommand(loginQuery, conn))
            {
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });
                command.Parameters.Add(new SqlParameter("@Senha", SqlDbType.NVarChar) { Value = senha });

                bool loginValido = (int)command.ExecuteScalar() > 0;
                return loginValido ? "OK" : "Usuário ou senha inválidos.";
            }
        }

        public LoginModel? ObterUsuarioPorEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"
                SELECT pk_leitor, email, loggin_password, nome_leitor, user_role, stat 
                FROM Leitor 
                WHERE email = @Email
                AND stat = 'active'";

            using var command = new SqlCommand(query, conn);
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });

            using var reader = command.ExecuteReader();
            if (!reader.HasRows)
                return null;

            reader.Read();
            return new LoginModel
            {
                Id = reader["pk_leitor"] != DBNull.Value ? Convert.ToInt32(reader["pk_leitor"]) : 0,
                Email = reader["email"].ToString(),
                Senha = reader["loggin_password"].ToString(),
                Nome = reader["nome_leitor"] != DBNull.Value ? reader["nome_leitor"].ToString() : "Usuário",
                UserRole = reader["user_role"] != DBNull.Value ? reader["user_role"].ToString() : "User"
            };
        }
    }
}
