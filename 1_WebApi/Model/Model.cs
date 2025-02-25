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
			Dictionary<string, List<object>>MethodsList = new Dictionary<string, List<object>>();
			MethodsList["GetTotalObra"] = null;
			MethodsList["GetTotalObraPorGenero"] = null;
			MethodsList["GetTopRequestedByTime"] = new List<object> { 
				new { name = "Data início", type = "date", size = 0, mandatory = true}, 
				new { name = "Data fim", type = "date", size = 0, mandatory = true} 
			};
			MethodsList["GetRequisicoesByNucleo"] = new List<object> { 
				new { name = "Data início", type = "date", size = 0, mandatory = true}, 
				new { name = "Data fim", type = "date", size = 0, mandatory = true}
			};
			MethodsList["InsertObra"] = new List<object> {
				new { name = "Núcleo a adicionar (opcional)", type = "number", size = 5, mandatory = false},
				new { name = "Nome da obra", type = "text", size = 50, mandatory = true},
				new { name = "ISBN", type = "text", size = 50, mandatory = true},
				new { name = "Autor", type = "text", size = 50, mandatory = true},
				new { name = "Editora", type = "text", size = 50, mandatory = true},
				new { name = "Ano", type = "text", size = 50, mandatory = true},
				new { name = "Imagem", type = "file", size = 50, mandatory = true},
				new { name = "Quantidade (opcional)", type = "number", size = 50, mandatory = false},


			};
			//MethodsList["InsertObra"] = null;

			//MethodsList["GetTotalObraPorGenero"] = new List<object> { new { name = "Nome", type = "string", size = 20 } };
			Methods = JsonSerializer.Serialize(MethodsList);

		}

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
