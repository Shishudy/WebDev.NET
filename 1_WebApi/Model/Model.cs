using Azure;
using LibEF.Models;
using LibEF;
using System.Text.Json;

namespace WebAPI.Model
{
	public class Model
	{
		public WebApplication App { get; set; }

		public readonly string Methods;

		public Model(WebApplication AppBuilt)
		{
			App = AppBuilt;
			Dictionary<string, object>MethodsList = new Dictionary<string, object>();
			MethodsList["GetTotalObraPorGenero"] = { name = "Nome", "type": ""}
			Methods = JsonSerializer.Serialize(MethodsList);

		}

		//public List<(string NomeGenero, int TotalQuantidade)> GetTotalObraPorGenero()
		//{
		//	var result = from g in context.Generos
		//				 from o in g.PkObras
		//				 join no in context.NucleoObras on o.PkObra equals no.PkObra
		//				 group no by g.NomeGenero into grouped
		//				 select new
		//				 {
		//					 NomeGenero = grouped.Key,
		//					 TotalQuantidade = grouped.Sum(n => n.Quantidade)
		//				 };
		//	return result.ToList().Select(r => (r.NomeGenero, r.TotalQuantidade)).ToList();
		//}

		public string Login(Object response)
		{
			ProjectoContext context = new ProjectoContext();
			EF_methods ef = new EF_methods(context);
			string jsonRes = JsonSerializer.Serialize(response);
			Dictionary<string, string> res = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
			try
			{
				var forecast = ef.GetPassWordbyLogin(res.GetValueOrDefault("username"));
				var obj = new { result = forecast};
				var json = JsonSerializer.Serialize(obj);
				return (json);
			}
			catch
			{
				var obj = new { result = "User not found."};
				var json = JsonSerializer.Serialize(obj);
				return (json);
			}
		}
	}
}
