using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using LibEF.Models;

namespace LibEF
{
	public class EF_methods
	{
		private readonly ProjectoContext context;

		public EF_methods(ProjectoContext dbContext)
		{
			context = dbContext;
		}

		public int available_copies(int PkObra, int PkNucleo)
		{
			int count_in_nucleo = context.NucleoObras
						.Where(n => n.PkObra == PkObra && n.PkNucleo == PkNucleo)
						.Select(n => n.Quantidade)
						.FirstOrDefault();
			int count_in_requisitions = context.Requisicaos
						.Where(r => r.PkObra == PkObra && r.PkNucleo == PkNucleo && r.Stat == "borrowed")
						.Count();
			int available_copies = count_in_nucleo - count_in_requisitions;
			return available_copies;
		}

		public string GetPassWordbyLogin(string name)
		{
			try
			{
				var leitor = context.Leitors.FirstOrDefault(l => l.NomeLeitor == name);
				if (leitor == null)
					throw new Exception("leitor not found");
				return leitor.UserPassword;
			}
			catch (Exception)
			{
				throw;
			}
		}




        /////////////////////
        //		1
        /////////////////////

        public int GetTotalObra()
        {
            return context.NucleoObras.Sum(no => no.Quantidade);
        }

        /////////////////////
        //		2
        /////////////////////

        public List<(string NomeGenero, int TotalQuantidade)> GetTotalObraPorGenero()
        {
            var result = from g in context.Generos
                        from o in g.PkObras
                        join no in context.NucleoObras on o.PkObra equals no.PkObra
                        group no by g.NomeGenero into grouped
                        select new
                        {
                            NomeGenero = grouped.Key,
                            TotalQuantidade = grouped.Sum(n => n.Quantidade)
                        };
            return result.ToList().Select(r => (r.NomeGenero, r.TotalQuantidade)).ToList();
        }

        /////////////////////
        //		3
        /////////////////////

        public List<(string NomeObra, int TimesRequested)> GetTopRequestedByTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = context.Requisicaos.Include(r => r.PkObraNavigation).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.DataLevantamento >= startDate);
            if (endDate.HasValue)
                query = query.Where(r => r.DataLevantamento <= endDate);
            var result = query.GroupBy(r => new { r.PkObraNavigation.NomeObra, r.PkObra })
                    .Select(g => new
                    {
                        NomeObra = g.Key.NomeObra,
                        TimesRequested = g.Count()
                    })
                    .OrderByDescending(o => o.TimesRequested)
                    .Take(10);
            return result.ToList().Select(r => (r.NomeObra, r.TimesRequested)).ToList();
        }

        /////////////////////
        //		4
        /////////////////////

        // public List<(string NomeNucleo, int TotalRequisicoes)> GetRequisicoesByNucleo(DateTime startDate, DateTime endDate)
        // {
        //     var result = from r in context.Requisicaos
        //                  join n in context.Nucleos on r.PkNucleo equals n.PkNucleo
        //                  select r;
        //     if (startDate != null)
        //         result = result.Where(r => r.DataLevantamento >= startDate);
        //     if (endDate != null)
        //         result = result.Where(r => r.DataLevantamento <= endDate);
        //     result = result.group r by new { n.PkNucleo, n.NomeNucleo } into g
        //         select new somethingsomething
        //         {
        //             NomeNucleo = g.Key.NomeNucleo,
        //             TotalRequisicoes = g.Count()
        //         };
        //     return result.OrderByDescending(r => r.TotalRequisicoes).ToList().Select(r => (r.NomeNucleo, r.TotalRequisicoes)).ToList();
        // }

        /////////////////////
        //		5
        /////////////////////

        public bool InsertObra(int? PkNucleo, string nomeObra, string isbn, string autor, string editora, int ano, string imagePath = null, int quantidade = 0)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var Obras = context.Obras;
                    var Nucleos = context.Nucleos;
                    var NucleoObras = context.NucleoObras;

                    // Check for existing obra and nucleo in a single query
                    // var existingObra = Obras.FirstOrDefault(o => o.Isbn == isbn);
                    // if (existingObra != null)
                    // 	throw new Exception("Error: obra already exists");
                    // Synchronous database operations
                    if (Obras.Any(o => o.Isbn == isbn))
                        throw new Exception("Error: obra already exists");
                    if (PkNucleo.HasValue && !Nucleos.Any(n => n.PkNucleo == PkNucleo))
                        throw new Exception("Error: nucleo not found");
                    if (!PkNucleo.HasValue)
                        PkNucleo = GetCentralNucleo(); //TODO: implement GetCentralNucleo
                    var obra = new Obra
                    {
                        NomeObra = nomeObra,
                        Isbn = isbn,
                        Editora = editora,
                        Ano = ano
                    };
                    Obras.Add(obra);
                    context.SaveChanges();
                    var nucleoObra = new NucleoObra
                    {
                        PkNucleo = PkNucleo.Value,
                        PkObra = obra.PkObra, //autoincremented
                        Quantidade = quantidade
                    };
                    NucleoObras.Add(nucleoObra);
                    context.SaveChanges();
                    if (!string.IsNullOrEmpty(imagePath))
                        UpdateImage(obra.PkObra, imagePath, isbn);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private bool UpdateImage(int pkObra, string imagePath, string isbn)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var obra = context.Obras.FirstOrDefault(o => o.PkObra == pkObra);
                    if (obra == null)
                        throw new Exception("Obras not found.");
                    var novaImagem = new ImageReference
                    {
                        ImagePath = imagePath,
                        ImageName = isbn
                    };
                    context.ImageReferences.Add(novaImagem);
                    context.SaveChanges();
                    obra.FkImagem = novaImagem.PkImage;
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private int GetCentralNucleo()
        {
            return context.Nucleos.Where(n => n.FkCentral == null).Select(n => n.PkNucleo).FirstOrDefault();
        }

        /////////////////////
        //		6
        /////////////////////

        public bool AddObra(int pkObra, int PkNucleo, int quantidade)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (!context.Obras.Any(o => o.PkObra == pkObra))
                        throw new Exception("Error: obra not found");

                    var nucleoObra = context.NucleoObras.FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == PkNucleo);
                    if (nucleoObra == null)
                        throw new Exception("Error: obra not found in given nucleo");
                    nucleoObra.Quantidade += quantidade;
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool RemoveObra(int pkObra, int PkNucleo, int quantidade)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var nucleoObra = context.NucleoObras.FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == PkNucleo);
                    if (nucleoObra == null)
                        throw new Exception("Error: obra not found in given nucleo");
                    int existing_copies = available_copies(pkObra, PkNucleo);
                    if (existing_copies < quantidade)
                        throw new Exception("Error: insufficient Obras for request in given nucleo");
                    nucleoObra.Quantidade -= quantidade;
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /////////////////////
        //		7
        /////////////////////

        public bool AddObraInNucleo(int pkObra, int PkNucleo, int quantidade)
        {//TODO maybe not needed
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var nucleoObraDestino = context.NucleoObras.FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == PkNucleo);
                    if (nucleoObraDestino != null)
                    {
                        AddObra(pkObra, PkNucleo, quantidade);
                    }
                    else
                    {
                        var novaNucleoObra = new NucleoObra
                        {
                            PkNucleo = PkNucleo,
                            PkObra = pkObra,
                            Quantidade = quantidade
                        };
                        context.NucleoObras.Add(novaNucleoObra);
                        context.SaveChanges();
                    }
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool TransferObra(int pkObra, int PkNucleoOrigem, int PkNucleoDestino, int quantidade)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    RemoveObra(pkObra, PkNucleoOrigem, quantidade);
                    AddObraInNucleo(pkObra, PkNucleoDestino, quantidade);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /////////////////////
        //		8
        /////////////////////

        public bool InsertLeitor(string nome, string? morada, string? telefone, string? email, string userPassword, string userRole = "USER")
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (context.Leitors.Any(l => l.Email == email))
                        throw new Exception("matching leitor login already exists");
                    if (context.Leitors.Any(l => l.NomeLeitor == nome && l.Morada == morada && l.Telefone == telefone))
                        throw new Exception("matching leitor already exists");
                    var leitor = new Leitor
                    {
                        NomeLeitor = nome,
                        DataInscricao = DateTime.Now,
                        //DataInscricao = DateTime.Now,
                        Morada = morada,
                        Telefone = telefone,
                        Email = email,
                        UserPassword = userPassword,
                        UserRole = userRole,
                        Stat = "active"
                    };
                    context.Leitors.Add(leitor);
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }



        public bool SuspendLateLeitor(int PkLeitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    if (leitor != null && leitor.Stat == "suspended")
                        throw new Exception("leitor is suspended");
                    var leitor_requesitions = context.Requisicaos.Where(r => r.PkLeitor == PkLeitor);
                    var countLate = leitor_requesitions.Where(r => CheckOvertime(r.DataLevantamento.Value, r.DataDevolucao.Value, 15)).Count();
                    // removed r.DataDevolucao.HasValue && 
                    if (countLate < 3)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    leitor.Stat = "suspended";
                    context.SaveChanges();
                    foreach (var requisition in leitor_requesitions)
                    {
                        if (requisition.Stat == "returned")
                            requisition.AlreadySuspend = true;
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool SuspendLeitor(int PkLeitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    if (leitor != null && leitor.Stat == "suspended")
                        throw new Exception("leitor is suspended");
                    leitor.Stat = "suspended";
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool CheckOvertime(DateTime dataLevantamento, DateTime? dataDevolucao, int diasLimite)
        {
            if (!dataDevolucao.HasValue)
                dataDevolucao = DateTime.Now;
            var diff = (dataDevolucao.Value - dataLevantamento).Days;
            return diff > diasLimite;
        }

        public List<Requisicao> GetRequisicoesLeitor(int PkLeitor)
        {
            return context.Requisicaos.Where(r => r.PkLeitor == PkLeitor).ToList();
        }

        /////////////////////
        //		bonus
        /////////////////////

        public bool Devolution(int PkLeitor, int pkObra, int nucleo)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    var requisition = context.Requisicaos.FirstOrDefault(r => r.PkLeitor == PkLeitor && r.PkObra == pkObra && r.PkNucleo == nucleo);
                    if (requisition == null)
                        throw new Exception("requisition not found");
                    requisition.Stat = "returned";
                    requisition.DataDevolucao = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool Requisition(int PkLeitor, int pkObra, int PkNucleo)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    var obra = context.Obras.FirstOrDefault(o => o.PkObra == pkObra);
                    if (obra == null)
                        throw new Exception("obra not found");
                    var nucleo = context.Nucleos.FirstOrDefault(n => n.PkNucleo == PkNucleo);
                    if (nucleo == null)
                        throw new Exception("nucleo not found");
                    SuspendLateLeitor(PkLeitor);
                    if (leitor.Stat == "suspended")
                        throw new Exception("leitor is suspended");
                    if (available_copies(pkObra, PkNucleo) < 2)
                        throw new Exception("no available copies");
                    var leitor_requisitions = context.Requisicaos.Where(r => r.PkLeitor == PkLeitor && r.Stat == "borrowed").ToList();
                    if (leitor_requisitions.Any(r => r.PkLeitor == PkLeitor && r.PkObra == pkObra))
                        throw new Exception("leitor already requested this obra");
                    if (leitor_requisitions.Count() == 4)
                        throw new Exception("leitor has 4 requesitions already");
                    var requisition = new Requisicao
                    {
                        PkLeitor = PkLeitor,
                        PkObra = pkObra,
                        PkNucleo = PkNucleo,
                        DataLevantamento = DateTime.Now
                    };
                    context.Requisicaos.Add(requisition);
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }



        /////////////////////
        //		10
        /////////////////////

        public bool sp_leitor_reactivate(int PkLeitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    if (leitor.Stat != "suspended")
                        throw new Exception("leitor is not suspended");
                    leitor.Stat = "active";
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /////////////////////
        //		11
        /////////////////////

        public bool sp_delete_inactive_Leitor(DateTime? date_since)
        {// TODO: not correct i am only deleting people that have done rquesitions and are inactive
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    int days_since;
                    if (date_since == null)
                        days_since = 365;
                    else
                        days_since = DateTime.Now.Day;
                    // linq commands will be efficiently combined by the provider
                    var requesitions = context.Requisicaos
                        .Where(r => DateTime.Now.Subtract(r.DataLevantamento.Value).Days > days_since)
                        .GroupBy(r => r.PkLeitor);
                    foreach (var leitores in requesitions)
                        sp_del_leitor(leitores.Key);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool sp_del_leitor(int PkLeitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    sp_save_leitor_history(PkLeitor);
                    context.Leitors.Remove(leitor);
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool sp_save_leitor_history(int PkLeitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    // Perform the insert into the History table
                    var historyEntries = from r in context.Requisicaos where( r.PkLeitor == PkLeitor)
                                         join o in context.Obras on r.PkObra equals o.PkObra
                                         join no in context.NucleoObras on o.PkObra equals no.PkObra
                                         join n in context.Nucleos on no.PkNucleo equals n.PkNucleo
                                         select new History
                                         { //same time wedo select wedo create new object
                                             NomeObra = o.NomeObra,
                                             IdObra = r.PkObra,
                                             Nucleo = n.NomeNucleo,
                                             DataRequisicao = r.DataLevantamento,
                                             DataDevolucao = r.DataDevolucao,
                                             NomeLeitor = leitor.NomeLeitor,
                                             IdLeitor = leitor.PkLeitor
                                         };
                    if (historyEntries.Any())
                    {
                        // if i want to add single entrie
                        // context.LeitorHistorie.Add(leitor_history);
                        context.Histories.AddRange(historyEntries);
                        context.SaveChanges();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // 1. fazer registo do leitor

        // 2. cancelar a inscrição de um leitor

        public bool sp_cancel_leitor(int PkLeitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.PkLeitor == PkLeitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    var leitor_requisitions = context.Requisicaos.Where(r => r.PkLeitor == PkLeitor);
                    var undelivered_requisitions = leitor_requisitions.Where(r => r.Stat == "borrowed");
                    foreach (var requisition in undelivered_requisitions)
                        Devolution(PkLeitor, requisition.PkObra, requisition.PkNucleo);
                    sp_del_leitor(PkLeitor);
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // 3. search for Obras by name

        public List<Obra> sp_search_Obra(string obraName)
        {
            return context.Obras.Where(o => o.NomeObra.Contains(obraName)).ToList();
        }

        public List<Nucleo> sp_search_Nucleo(string nucleoName)
        {
            return context.Nucleos.Where(n => n.NomeNucleo.Contains(nucleoName)).ToList();
        }


        public List<Obra> sp_search_Obra_genero(string generoName)
        {
            var Obras = from g in context.Generos
                        where g.NomeGenero.Contains(generoName)
                        from o in g.PkObras
                        select o;
            return Obras.ToList();
            //var Obra_genero = context.Obras
            //    .Join(context.GeneroObras, o => o.PkObra, go => go.PkObra, (o, go) => new { o, go })
            //    .Join(context.Generos, og => og.go.PkGenero, g => g.PkGenero, (og, g) => new { og, g })
            //    .Where(og => og.g.NomeGenero.Contains(generoName))
            //    .Select(og => og.og.o)
            //    .ToList();
        }

        public class Requisicaotatus
        {
            public string NomeObra { get; set; }
            public int PkNucleo { get; set; }
            public string NomeNucleo { get; set; }
            public DateTime DataLevantamento { get; set; }
            public DateTime DataDevolucao { get; set; }
            public string StatusMessage { get; set; }
        }

        public List<Requisicaotatus> requesicao_status(int PkLeitor, int? PkNucleo = null)
        {
            var leitoresFiltradas = from l in context.Leitors
                                    where l.PkLeitor == PkLeitor
                                    select l;

            var requisicoesFiltradas = from l in context.Requisicaos
                                       where l.Stat == "borrowed"
                                       select l;
            var query = from l in leitoresFiltradas
                        join r in requisicoesFiltradas on l.PkLeitor equals r.PkLeitor
                        join o in context.Obras on r.PkObra equals o.PkObra
                        join no in context.NucleoObras on o.PkObra equals no.PkObra
                        join n in context.Nucleos on no.PkNucleo equals n.PkNucleo // Get Núcleo Name
                        select new Requisicaotatus// anonymous type
                        {
                            NomeObra = o.NomeObra,
                            PkNucleo = n.PkNucleo,
                            NomeNucleo = n.NomeNucleo,
                            //DataLevantamento = r.DataLevantamento,
                            DataLevantamento = r.DataLevantamento.Value,
                            DataDevolucao = r.DataDevolucao.Value,
                            StatusMessage = ""
                        };

            // Apply filter if PkNucleo is provided
            if (PkNucleo.HasValue)
            {
                query = query.Where(x => x.PkNucleo == PkNucleo.Value);
            }

            // Convert to List and Set Status Messages
            foreach (var r in query) //TODO: Botle neck here
            {
                int date_diff = DateTime.Now.Subtract(r.DataDevolucao).Days;
                r.StatusMessage = date_diff > 15 ? "ATRASO" :
                                  date_diff > 12 ? "Devolução URGENTE" :
                                  date_diff > 10 ? "Devolver em breve" : "Em dia";
            }
            return query.ToList();
        }


        // public async Task<bool> InsertObraAsync(int? PkNucleo, string nomeObra, string isbn, string autor, string editora, int ano, string imagePath = null, int quantidade = 0)
        // {
        // 	using (var transaction = await Database.BeginTransactionAsync())
        // 	{
        // 		try
        // 		{
        // 			// Asynchronous database operations
        // 			if (await Obras.AnyAsync(o => o.Isbn == isbn))
        // 				throw new Exception("Error: obra already exists");
        // 			if (PkNucleo.HasValue && !await Nucleos.AnyAsync(n => n.PkNucleo == PkNucleo))
        // 				throw new Exception("Error: nucleo not found");
        // 			if (!PkNucleo.HasValue)
        // 				PkNucleo = await GetCentralNucleoAsync();
        // 			var obra = new Obras
        // 			{
        // 				NomeObra = nomeObra,
        // 				Isbn = isbn,
        // 				Editora = editora,
        // 				Ano = ano
        // 			};
        // 			Obras.Add(obra);
        // 			await SaveChangesAsync();
        // 			var nucleoObra = new NucleoObras
        // 			{
        // 				PkNucleo = PkNucleo.Value,
        // 				PkObra = obra.PkObra,
        // 				Quantidade = quantidade
        // 			};
        // 			NucleoObras.Add(nucleoObra);
        // 			await SaveChangesAsync();
        // 			if (!string.IsNullOrEmpty(imagePath))
        // 			{
        // 				await UpdateImageAsync(obra.PkObra, imagePath, isbn);
        // 			}
        // 			await transaction.CommitAsync();
        // 			return true;
        // 		}
        // 		catch (Exception)
        // 		{
        // 			await transaction.RollbackAsync();
        // 			throw;
        // 		}
        // 	}
        // }

    }
}
