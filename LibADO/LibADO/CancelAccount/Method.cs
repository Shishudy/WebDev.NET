using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;

namespace LibADO.CancelAccount
{
    public class Method
    {
        public static bool sp_cancel_leitor(int pk_leitor, string connectionString)
        {
            using var cn = DB.Open(connectionString);
            using var transaction = cn.BeginTransaction();

            try
            {
                string checkLeitor = "SELECT COUNT(*) FROM dbo.Leitor WHERE pk_leitor = @pk_leitor";
                using (var cmd = new SqlCommand(checkLeitor, cn, transaction))
                {
                    cmd.Parameters.AddWithValue("@pk_leitor", pk_leitor);
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0)
                        throw new Exception("Leitor not found");
                }
                string updateRequisicoes = @"
                UPDATE dbo.Requisicao
                SET stat = 'returned', data_devolucao = GETDATE()
                WHERE pk_leitor = @pk_leitor";
                using (var cmd = new SqlCommand(updateRequisicoes, cn, transaction))
                {
                    cmd.Parameters.AddWithValue("@pk_leitor", pk_leitor);
                    cmd.ExecuteNonQuery();
                }
                string deleteLeitor = @"
                DELETE FROM dbo.Leitor WHERE pk_leitor = @pk_leitor";
                using (var cmd = new SqlCommand(deleteLeitor, cn, transaction))
                {
                    cmd.Parameters.AddWithValue("@pk_leitor", pk_leitor);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
