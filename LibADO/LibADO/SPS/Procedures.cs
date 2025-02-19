using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDB;
using Microsoft.Data.SqlClient;

namespace LibADO.SPS
{
    internal class Procedures
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
        } //Retorna todas as requisições e cancela a adesão

        public static List<Dictionary<string, object>> sp_requisicoes_by_nucleo(DateTime startDate, DateTime endDate, string connectionString)
        {
            using var cn = DB.Open(connectionString);

            string query = @"
            SELECT n.nome_nucleo, COUNT(r.pk_obra) AS total_requisicoes
            FROM dbo.Requisicao r
                INNER JOIN dbo.Nucleo n ON r.pk_nucleo = n.pk_nucleo
                INNER JOIN dbo.Obra o ON r.pk_obra = o.pk_obra
            WHERE r.data_levantamento BETWEEN @start_date AND @end_date
            GROUP BY n.pk_nucleo, n.nome_nucleo
            ORDER BY total_requisicoes DESC";

            var dt = DB.GetSQLRead(cn, query);
            return DB.ToDictionary(dt);
        } //Verifica requisições por nucleo

        public static List<Dictionary<string, object>> sp_search_obras(string obra, string genre, string nucleo, string connectionString)
        {
            using var cn = DB.Open(connectionString);

            string query = @"
            SELECT o.nome_obra, n.nome_nucleo, no.quantidade
            FROM dbo.NucleoObra no
                INNER JOIN dbo.Obra o ON no.pk_obra = o.pk_obra
                INNER JOIN dbo.Nucleo n ON no.pk_nucleo = n.pk_nucleo
            WHERE 
                no.pk_obra IN (SELECT pk_obra FROM dbo.fn_search_obras(@obra))
                AND no.pk_obra IN (SELECT pk_obra FROM dbo.fn_search_obras_genre(@genre))
                AND no.pk_nucleo IN (SELECT pk_nucleo FROM dbo.fn_search_nucleo(@nucleo))";

            var dt = DB.GetSQLRead(cn, query);
            return DB.ToDictionary(dt);
        } //Procurar obras ou por genero ou por nucleo

        public static List<Dictionary<string, object>> sp_total_obras_por_genero(string connectionString)
        {
            using var cn = DB.Open(connectionString);

            string query = @"
            SELECT g.nome_genero, SUM(no.quantidade) AS total_quantidade
            FROM dbo.NucleoObra no
                INNER JOIN dbo.Obra o ON no.pk_obra = o.pk_obra
                INNER JOIN dbo.GeneroObra go ON o.pk_obra = go.pk_obra
                INNER JOIN dbo.Genero g ON go.pk_genero = g.pk_genero
            GROUP BY g.nome_genero";

            var dt = DB.GetSQLRead(cn, query);
            return DB.ToDictionary(dt);
        } 
    }
}
