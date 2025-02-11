using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfProcedures.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfProcedures
{
    internal class Procedures
    {
        public static bool sp_suspend_late(int pkLeitor)
        {
            try
            {
                using (var context = new ProjectoContext())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == pkLeitor);
                        if (leitor == null)
                        {
                            Console.WriteLine("Leitor não encontrado.");
                            return false;
                        }

                        if (leitor.Stat == "suspended")
                        {
                            Console.WriteLine("Leitor já está suspenso.");
                            return false;
                        }

                        // Carregar todos os registros de requisicao para o leitor em memória
                        var requisicoes = context.Requisicaos
                            .Where(r => r.PkLeitor == pkLeitor && r.DataLevantamento.HasValue)
                            .ToList();

                        // Contar os atrasos na memória
                        var atrasos = requisicoes.Count(r =>
                            r.DataDevolucao.HasValue
                            ? ((r.DataDevolucao.Value.ToDateTime(TimeOnly.MinValue) - r.DataLevantamento.Value.ToDateTime(TimeOnly.MinValue)).Days > 15) // Calculando atraso se DataDevolucao existe
                            : ((DateTime.Today - r.DataLevantamento.Value.ToDateTime(TimeOnly.MinValue)).Days > 15) // Calculando atraso se DataDevolucao não existe
                        );

                        if (atrasos > 3)
                        {
                            leitor.Stat = "suspended";
                            context.SaveChanges();
                            transaction.Commit();
                            Console.WriteLine("✅ Leitor suspenso devido a atrasos.");
                            return true;
                        }

                        transaction.Commit();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erro ao suspender leitor: " + ex.Message);
                return false;
            }
        }



        public static List<(string NomeObra, int TimesRequested)> sp_top_requested_by_time (DateOnly? startDate = null, DateOnly? endDate = null)
        {
            try
            {
                using (var context = new ProjectoContext())
                {
                    var query = context.Requisicaos
                        .Include(r => r.PkObraNavigation)
                        .AsQueryable();


                    if (startDate.HasValue && endDate.HasValue)
                    {
                        query = query.Where(r => r.DataLevantamento.HasValue &&
                                                 r.DataLevantamento.Value >= startDate.Value &&
                                                 r.DataLevantamento.Value <= endDate.Value);
                    }

                    var resultado = query
                        .GroupBy(r => new { r.PkObraNavigation.NomeObra, r.PkObra })
                        .Select(g => new
                        {
                            NomeObra = g.Key.NomeObra,
                            TimesRequested = g.Count()
                        })
                        .OrderByDescending(o => o.TimesRequested)
                        .Take(10)
                        .ToList();

                    return resultado.Select(r => (r.NomeObra, r.TimesRequested)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao buscar as obras mais requisitadas: " + ex.Message);
                return new List<(string, int)>();
            }
        }
        public static int sp_total_obras()
        {
            try
            {
                using (var context = new ProjectoContext())
                {
                    return context.NucleoObras.Sum(no => (int?)no.Quantidade) ?? 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao obter o total de obras: " + ex.Message);
                return 0;
            }
        }
        public static List<(string NomeGenero, int TotalQuantidade)> sp_total_obras_por_genero()
        {
            try
            {
                using (var context = new ProjectoContext())
                {
                    var resultado = context.NucleoObras
                        .Include(no => no.PkObraNavigation)
                            .ThenInclude(o => o.PkGeneros)
                        .SelectMany(no => no.PkObraNavigation.PkGeneros, (no, genero) => new
                        {
                            NomeGenero = genero.NomeGenero,
                            Quantidade = no.Quantidade
                        })
                        .GroupBy(g => g.NomeGenero)
                        .Select(g => new
                        {
                            NomeGenero = g.Key,
                            TotalQuantidade = g.Sum(no => no.Quantidade)
                        })
                        .Where(g => g.NomeGenero != null)
                        .ToList();

                    return resultado.Select(r => (r.NomeGenero, r.TotalQuantidade)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao obter total de obras por gênero: " + ex.Message);
                return new List<(string, int)>();
            }
        }
        public static bool sp_transfer_obra (int pkObra, int pkNucleoOrigem, int pkNucleoDestino, int quantidade)
        {
            using (var context = new ProjectoContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        var nucleoOrigem = context.NucleoObras
                            .FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == pkNucleoOrigem);

                        if (nucleoOrigem == null || nucleoOrigem.Quantidade < quantidade)
                            throw new Exception("Quantidade insuficiente no núcleo de origem.");


                        nucleoOrigem.Quantidade -= quantidade;
                        if (nucleoOrigem.Quantidade == 0)
                            context.NucleoObras.Remove(nucleoOrigem);


                        var nucleoDestino = context.NucleoObras
                            .FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == pkNucleoDestino);

                        if (nucleoDestino != null)
                        {

                            nucleoDestino.Quantidade += quantidade;
                        }
                        else
                        {

                            var novaEntrada = new NucleoObra
                            {
                                PkObra = pkObra,
                                PkNucleo = pkNucleoDestino,
                                Quantidade = quantidade
                            };
                            context.NucleoObras.Add(novaEntrada);
                        }


                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Erro ao transferir obra: " + ex.Message);
                        return false;
                    }
                }
            }
        }
        public static bool sp_update_image (int pkObra, string imagePath, string isbn)
        {
            using (var context = new ProjectoContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(imagePath))
                            throw new Exception("O caminho da imagem não pode ser nulo.");

                   
                        var novaImagem = new ImageReference
                        {
                            ImagePath = imagePath,
                            ImageName = isbn
                        };

                        context.ImageReferences.Add(novaImagem);
                        context.SaveChanges(); 

                      
                        var obra = context.Obras.FirstOrDefault(o => o.PkObra == pkObra);
                        if (obra == null)
                            throw new Exception("Obra não encontrada.");

                        obra.FkImagem = novaImagem.PkImage;
                        context.SaveChanges();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Erro ao atualizar imagem da obra: " + ex.Message);
                        return false;
                    }
                }
            }
        }



    }

}

