using System.Data;
using Microsoft.Data.SqlClient;
using LibADO.Login;
using LibDB;

namespace LibADO.Login
{
    public interface LoginRepository
    {
        bool ValidarLogin(string email, string senha);
        LoginModel ObterUsuarioPorEmail(string email);
    }

    public class LoginRepositoryADO : LoginRepository
    {
        private readonly string _connectionString;

        public LoginRepositoryADO(string connectionString) => _connectionString = connectionString;

        public bool ValidarLogin(string email, string senha)
        {
            using var conn = DB.Open(_connectionString);
            string query = "SELECT COUNT(*) FROM Leitor WHERE email = @Email AND loggin_password = @Senha";
            return (int)new SqlCommand(query, conn)
            {
                Parameters = { new("@Email", email), new("@Senha", senha) }
            }.ExecuteScalar() > 0;
        }

        public LoginModel? ObterUsuarioPorEmail(string email)
        {
            using var conn = DB.Open(_connectionString);
            string query = "SELECT email, loggin_password FROM Leitor WHERE email = @Email";

            DataTable dt = DB.GetSQLRead(conn, query);
            return dt.Rows.Count > 0 ? new LoginModel
            {
                Email = dt.Rows[0]["email"].ToString(),
                Senha = dt.Rows[0]["loggin_password"].ToString()
            } : null;
        }
    }
}
