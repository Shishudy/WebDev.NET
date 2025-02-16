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

	public class AppDbContext : DbContext
	{
		public DbSet<Leitor> Leitors { get; set; }
		public DbSet<Requisicao> Requisicaos { get; set; }
		public DbSet<NucleoObra> NucleoObras { get; set; }
		public DbSet<Obra> Obras { get; set; }
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

	public List<RequisicaoStatus> GetLeitorSituation(int pkLeitor, string nucleo = null)
	{
		var currentDate = DateTime.Now;
		var date_diff;

		var query = from r in context.Requisicaos
					join o in context.Obras on r.PkObra equals o.PkObra
					join n in context.Nucleos on r.PkNucleo equals n.PkNucleo
					where r.PkLeitor == pkLeitor && r.Stat == "borrowed"
					select new
					{
						o.NomeObra,
						n.NomeNucleo,
						r.DataLevantamento,
						r.DataDevolucao,
						date_diff = (currentDate - r.DataLevantamento).Days,
						StatusMessage = date_diff > 15 ? "ATRASO" :
										date_diff > (15 - 3) ? "Devolução URGENTE" :
										date_diff > (15 - 5) ? "Devolver em breve" : "Em dia"
					};
		if (!string.IsNullOrEmpty(nucleo))
		{
			var nucleoIds = context.Nucleos
				.Where(n => EF.Functions.Like(n.NomeNucleo, $"%{nucleo}%"))
				.Select(n => n.PkNucleo)
				.ToList();

			query = query.Where(r => nucleoIds.Contains(r.NomeNucleo));
		}

		return query.Select(r => new RequisicaoStatus
		{
			NomeObra = r.NomeObra,
			NomeNucleo = r.NomeNucleo,
			DataLevantamento = r.DataLevantamento,
			DataDevolucao = r.DataDevolucao,
			StatusMessage = r.StatusMessage
		}).ToList();
	}

	public	void LeitorReactivate(int pkLeitor)
	{
		using (var transaction = Database.BeginTransaction())
		{
			try
			{
				var Leitors = context.Leitors;
				
				var leitor = Leitors.FirstOrDefault(l => l.PkLeitor == pkLeitor);
				// var leitor = Leitors.Where(l => l.PkLeitor == pkLeitor).FirstOrDefault(); //single element
				// var leitor = Leitors.Where(l => l.PkLeitor == pkLeitor); //iEnumerable
				if (leitor == null)
					throw new Exception("Error: leitor not found");
				if (leitor.Stat != "suspended")
					throw new Exception("Error: leitor is not suspended");
				leitor.Stat = "active";
				SaveChanges();
				transaction.Commit();
			}
			catch (Exception)
			{
				transaction.Rollback();
				throw;
			}
		}
	}
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
				SaveChanges();
				var nucleoObra = new NucleoObra
				{
					PkNucleo = pkNucleo.Value, //TODO why
					PkObra = obra.PkObra,
					Quantidade = quantidade
				};
				NucleoObras.Add(nucleoObra);
				SaveChanges();
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
}