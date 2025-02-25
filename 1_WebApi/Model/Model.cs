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
			MethodsList["GetTopRequestedByTime"] = new List<object> { new { name = "Data início", type = "date", size = 0 }, new { name = "Data fim", type = "date", size = 0} };
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
