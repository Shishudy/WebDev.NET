using System;
using System.Collections.Generic;
using System.Data;
using LibDB;
using Microsoft.Data.SqlClient;

namespace LibADO.UserRequisitions
{
    public class GetUserRequisitions
    {
        private readonly string _connectionString;

        public GetUserRequisitions(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Dictionary<string, object>> GetObrasRequisitadas(int pkLeitor)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            string query = @"SELECT O.pk_obra, O.nome_obra, O.image_path, N.nome_nucleo, COUNT(R.pk_obra) AS quantidade
                         FROM dbo.Requisicao R
                         INNER JOIN dbo.Obra O ON R.pk_obra = O.pk_obra
                         INNER JOIN dbo.Nucleo N ON O.pk_nucleo = N.pk_nucleo
                         WHERE R.pk_leitor = @PkLeitor AND R.stat = 'borrowed'
                         GROUP BY O.pk_obra, O.nome_obra, O.image_path, N.nome_nucleo
                         ORDER BY O.nome_obra ASC";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PkLeitor", pkLeitor);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            result = DB.ToDictionary(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter obras requisitadas: {ex.Message}");
                }
            }

            return result;
        }
    }
}

