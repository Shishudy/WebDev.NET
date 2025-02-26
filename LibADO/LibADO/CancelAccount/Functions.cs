using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;

namespace LibADO.CancelAccount
{
    public class Functions
    {

        public static int? GetLeitorIdByEmail(string email, string connectionString)
        {
            using var cn = DB.Open(connectionString);
            using var cmd = new SqlCommand("SELECT pk_leitor FROM dbo.Leitor WHERE email = @Email", cn);

            cmd.Parameters.AddWithValue("@Email", email);

            var result = cmd.ExecuteScalar();

            return result != DBNull.Value && result != null ? Convert.ToInt32(result) : (int?)null;
        }


    }
}
