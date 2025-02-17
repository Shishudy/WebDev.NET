using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EfProcedures;
using System.Transactions;

namespace DataAccessLayer
{

    public class AppDbContext : DbContext
    {
        public DbSet<Leitor> Leitors { get; set; }
        public DbSet<Requisicao> Requisicaos { get; set; }
        public DbSet<NucleoObra> NucleoObras { get; set; }
        public DbSet<Obra> Obras { get; set; 
        public DbSet<ImageReference> ImageReferences { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("your_connection_string");
        }
        //TODO: scaffolded code and use Fluent API configurations to update the model (database-first approach)
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // Apply configurations using Fluent API
        //     modelBuilder.ApplyConfiguration(new UserConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderConfiguration());
        // }
    }
}

public class Procedures
{
    //dependency injection: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0
    private readonly AppDbContext context;
    public Procedures(AppDbContext dbContext)
    {
        this.context = dbContext;
    }


namespace EfProcedures
{
    public class Procedures
    {
        private readonly AppDbContext context;

        public Procedures(AppDbContext dbContext)
        {
            context = dbContext;
        }


        /////////////////////
        //		1
        /////////////////////

        public int GetTotalObras()
        {
            return context.NucleoObras.Sum(no => no.Quantidade);
        }
        public List<(string NomeGenero, int TotalQuantidade)> GetTotalObrasPorGenero()
        {
            var result = from n in context.NucleoObras
                         join go in context.GeneroObras on n.PkObra equals go.PkObra
                         join g in context.Generos on g.PkGenero equals g.PkGenero
                         group n by g.NomeGenero into g
                         select new
                         {
                             NomeGenero = g.Key,
                             TotalQuantidade = g.Sum(n => n.Quantidade)
                         };
            return result.ToList().Select(r => (r.NomeGenero, r.TotalQuantidade)).ToList();
        }

        /////////////////////
        //		2
        /////////////////////
        //
        /////////////////////
        //		3
        /////////////////////

        public List<(string NomeObra, int TimesRequested)> GetTopRequestedByTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = context.Requisicaos.Include(r => r.Obra).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.DataLevantamento >= startDate);
            if (endDate.HasValue)
                query = query.Where(r => r.DataLevantamento <= endDate);
            var result = query.GroupBy(r => new { r.Obra.NomeObra, r.PkObra })
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

        public List<(string NomeNucleo, int TotalRequisicoes)> GetRequisicoesByNucleo(DateTime startDate, DateTime endDate)
        {
            var result = from r in context.Requisicaos
                         join n in context.Nucleos on r.PkNucleo equals n.PkNucleo

            if (startDate != null)
                result = result.Where(r.DataLevantamento >= startDate);
            if (endDate != null)
                result = result.Where(r.DataLevantamento <= endDate);
            result = result.group r by new { n.PkNucleo, n.NomeNucleo } into g
                select new somethingsomething
                {
                    NomeNucleo = g.Key.NomeNucleo,
                    TotalRequisicoes = g.Count()
                };
            return result.OrderByDescending(r => r.TotalRequisicoes).ToList().Select(r => (r.NomeNucleo, r.TotalRequisicoes)).ToList();
        }

        /////////////////////
        //		5
        /////////////////////

        public bool InsertObra(int? pkNucleo, string nomeObra, string isbn, string autor, string editora, int ano, string imagePath = null, int quantidade = 0)
        {
            using (var transaction = Database.BeginTransaction())
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
                    if (pkNucleo.HasValue && !Nucleos.Any(n => n.PkNucleo == pkNucleo))
                        throw new Exception("Error: nucleo not found");
                    if (!pkNucleo.HasValue)
                        pkNucleo = GetCentralNucleo(); //TODO: implement GetCentralNucleo
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
                        PkNucleo = pkNucleo.Value,
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
                        throw new Exception("Obra not found.");
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

        public bool AddObra(int pkObra, int pkNucleo, int quantidade)
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (!context.Obras.Any(o => o.PkObra == pkObra))
                            throw new Exception("Error: obra not found");

                        var nucleoObra = context.NucleoObras.FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == pkNucleo);
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
        }

        public bool RemoveObra(int pkObra, int pkNucleo, int quantidade)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var nucleoObra = context.NucleoObras.FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == pkNucleo);
                    if (nucleoObra == null)
                        throw new Exception("Error: obra not found in given nucleo");
                    int available_copies = available_copies(pkObra, pkNucleo);
                    if (available_copies < quantidade)
                        throw new Exception("Error: insufficient obras for request in given nucleo");
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

        public int available_copies(int pk_obra, int pk_nucleo)
        {
            int count_in_nucleo = context.NucleoObras
                .Where(n => n.PkObra == pk_obra && n.PkNucleo == pk_nucleo)
                .Select(n => n.Quantidade)
                .FirstOrDefault();
            int count_in_requisitions = context.Requisicaos
                .Where(r => r.PkObra == pk_obra && r.PkNucleo == pk_nucleo && r.Stat == "borrowed")
                .Count();
            int available_copies = count_in_nucleo - count_in_requisitions;
            return available_copies;
        }

        /////////////////////
        //		7
        /////////////////////

        public bool AddObraInNucleo(int pkObra, int pkNucleo, int quantidade)
        {//TODO maybe not needed
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var nucleoObraDestino = context.NucleoObras.FirstOrDefault(no => no.PkObra == pkObra && no.PkNucleo == pkNucleoDestino);
                    if (nucleoObraDestino != null)
                    {
                        AddObra(pkObra, pkNucleo, quantidade);
                    }
                    else
                    {
                        var novaNucleoObra = new NucleoObra
                        {
                            PkNucleo = pkNucleo,
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

        public bool TransferObra(int pkObra, int pkNucleoOrigem, int pkNucleoDestino, int quantidade)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    RemoveObra(pkObra, pkNucleoOrigem, quantidade);
                    AddObraInNucleo(pkObra, pkNucleoDestino, quantidade);
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

        public bool InsertLeitor(string nome, string? morada, string? telefone, string? email, string? userPassword, string userRole = "USER")
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



        public bool SuspendLateLeitor(int pk_leitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    if (leitor != null && leitor.Stat == "suspended")
                        throw new Exception("leitor is suspended");
                    var leitor_requesitions = context.Requisicaos.Where(r => r.pk_leitor == pk_leitor);
                    var countLate = leitor_requesitions.Where(fn_check_overtime(r.DataLevantamento, r.DataDevolucao.Value, 15)).Count();
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
                            requisition.AlreadySuspend = 1;
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

        public bool SuspendLeitor(int pk_leitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
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

        public List<Requisicao> GetRequisicoesLeitor(int pk_leitor)
        {
            return context.Requisicaos.Where(r => r.pk_leitor == pk_leitor).ToList();
        }

        /////////////////////
        //		bonus
        /////////////////////

        public bool Devolution(int pk_leitor, int pkObra, int nucleo)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    var requisition = context.Requisicaos.FirstOrDefault(r => r.pk_leitor == pk_leitor && r.PkObra == pkObra && r.PkNucleo == nucleo);
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

        public bool Requisition(int pk_leitor, int pkObra, int pkNucleo)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    var obra = context.Obras.FirstOrDefault(o => o.PkObra == pkObra);
                    if (obra == null)
                        throw new Exception("obra not found");
                    var nucleo = context.Nucleos.FirstOrDefault(n => n.PkNucleo == pkNucleo);
                    if (nucleo == null)
                        throw new Exception("nucleo not found");
                    SuspendLateLeitor(pk_leitor);
                    if (leitor.Stat == "suspended")
                        throw new Exception("leitor is suspended");
                    if (available_copies(pkObra, pkNucleo) < 2)
                        throw new Exception("no available copies");
                    var leitor_requisitions = context.Requisicaos.Where(r => r.pk_leitor == pk_leitor && r.Stat == "borrowed").ToList();
                    if (leitor_requisitions.Any(r => r.pk_leitor == pk_leitor && r.PkObra == pkObra))
                        throw new Exception("leitor already requested this obra");
                    if (leitor_requisitions.Count() == 4)
                        throw new Exception("leitor has 4 requesitions already");
                    var requisition = new Requisicao
                    {
                        pk_leitor = pk_leitor,
                        PkObra = pkObra,
                        PkNucleo = pkNucleo,
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

        public bool sp_leitor_reactivate(int pk_leitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
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

        public bool sp_delete_inactive_leitors(DateTime? date_since)
        {// TODO: not correct i am only deleting people that have done rquesitions and are inactive
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (date_since == null)
                        date_since = 365;
                    // linq commands will be efficiently combined by the provider
                    var requesitions = context.Requisicaos
                        .Where(r => DateTime.Now.Subtract(r.DataLevantamento) > date_since)
                        .GroupBy(r => r.pk_leitor);
                    foreach (var leitores in requesitions)
                        sp_del_leitor(leitores.pk_leitor);
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

        public bool sp_del_leitor(int pk_leitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    sp_save_leitor_history(pk_leitor);
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

        public bool sp_save_leitor_history(int pk_leitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    // Perform the insert into the History table
                    var historyEntries = from l in leitor
                                         join r in context.Requisicaos on l.pk_leitor equals r.pk_leitor
                                         join obra in context.Obras on r.PkObra equals obra.PkObra
                                         join no in context.NucleoObras on obra.PkObra equals no.PkObra
                                         join n in context.Nucleos on no.PkNucleo equals n.PkNucleo
                                         select new History
                                         { //same time wedo select wedo create new object
                                             NomeObra = obra.NomeObra,
                                             IdObra = r.PkObra,
                                             Nucleo = n.NomeNucleo,
                                             DataRequisicao = r.DataLevantamento,
                                             DataDevolucao = r.DataDevolucao,
                                             NomeLeitor = l.NomeLeitor,
                                             IdLeitor = l.pk_leitor
                                         };
                    if (historyEntries.Any())
                    {
                        // if i want to add single entrie
                        // context.LeitorHistories.Add(leitor_history);
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

        public bool sp_cancel_leitor(int pk_leitor)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var leitor = context.Leitors.FirstOrDefault(l => l.pk_leitor == pk_leitor);
                    if (leitor == null)
                        throw new Exception("leitor not found");
                    var leitor_requisitions = context.Requisicaos.Where(r => r.pk_leitor == pk_leitor);
                    var undelivered_requisitions = leitor_requisitions.Where(r => r.Stat == "borrowed");
                    foreach (var requisition in undelivered_requisitions)
                        Devolution(pk_leitor, requisition.PkObra, requisition.PkNucleo);
                    sp_del_leitor(pk_leitor);
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // 3. search for obras by name

        public List<Obra> sp_search_obras(string obraName)
        {
            return context.Obras.Where(o => o.NomeObra.Contains(obraName)).ToList();
        }

        public List<Nucleo> sp_search_nucleos(string nucleoName)
        {
            return context.Nucleos.Where(n => n.NomeNucleo.Contains(nucleoName)).ToList();
        }


        public List<Obra> sp_search_obras_genero(string generoName)
        {
            var obras_genero = context.Obras
                .Join(context.GeneroObras, o => o.PkObra, go => go.PkObra, (o, go) => new { o, go })
                .Join(context.Generos, og => og.go.PkGenero, g => g.PkGenero, (og, g) => new { og, g })
                .Where(og => og.g.NomeGenero.Contains(generoName))
                .Select(og => og.og.o)
                .ToList();
            return obras_genero;
        }

        public class RequisicaoStatus
        {
            public string NomeObra { get; set; }
            public int PkNucleo { get; set; }
            public string NomeNucleo { get; set; }
            public DateTime DataLevantamento { get; set; }
            public DateTime DataDevolucao { get; set; }
            public string StatusMessage { get; set; }
        }

        public List<RequisicaoStatus> requesicao_status(int pk_leitor, int? pk_nucleo = null)
        {
            var leitoresFiltradas = from l in context.Leitors
                                    where l.pk_leitor == pk_leitor select l;

            var requisicoesFiltradas = from l in context.Requisicaos
                                       where l.Stat == "borrowed" select l;
            var query = from l in leitoresFiltradas
                        join r in requisicoesFiltradas on l.pk_leitor equals r.pk_leitor
                        join o in context.Obras on r.PkObra equals o.PkObra
                        join no in context.NucleoObras on o.PkObra equals no.PkObra
                        join n in context.Nucleos on no.PkNucleo equals n.PkNucleo // Get Núcleo Name
                        select new RequisicaoStatus// anonymous type
                        {
                            NomeObra = o.NomeObra,
                            PkNucleo = n.PkNucleo,
                            NomeNucleo = n.NomeNucleo,
                            DataLevantamento = r.DataLevantamento,
                            DataDevolucao = r.DataDevolucao,
                            StatusMessage = ""
                        };

            // Apply filter if pk_nucleo is provided
            if (pk_nucleo.HasValue)
            {
                query = query.Where(x => x.PkNucleo == pk_nucleo.Value);
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
    }
}

        // public async Task<bool> InsertObraAsync(int? pkNucleo, string nomeObra, string isbn, string autor, string editora, int ano, string imagePath = null, int quantidade = 0)
        // {
        // 	using (var transaction = await Database.BeginTransactionAsync())
        // 	{
        // 		try
        // 		{
        // 			// Asynchronous database operations
        // 			if (await Obras.AnyAsync(o => o.Isbn == isbn))
        // 				throw new Exception("Error: obra already exists");
        // 			if (pkNucleo.HasValue && !await Nucleos.AnyAsync(n => n.PkNucleo == pkNucleo))
        // 				throw new Exception("Error: nucleo not found");
        // 			if (!pkNucleo.HasValue)
        // 				pkNucleo = await GetCentralNucleoAsync();
        // 			var obra = new Obra
        // 			{
        // 				NomeObra = nomeObra,
        // 				Isbn = isbn,
        // 				Editora = editora,
        // 				Ano = ano
        // 			};
        // 			Obras.Add(obra);
        // 			await SaveChangesAsync();
        // 			var nucleoObra = new NucleoObra
        // 			{
        // 				PkNucleo = pkNucleo.Value,
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
