using Azure;
using LibEF.Models;
using LibEF;
using System.Text.Json;

namespace WebAPI.Model
{
	public class Model
	{
		public WebApplication App { get; set; }

		public readonly string MethodsJson;

		public readonly Dictionary<string, List<object>> MethodsDict;

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
				new { name = "Núcleo a adicionar (opcional)", type = "number", mandatory = false},
				new { name = "Nome da obra", type = "text", size = 50, mandatory = true},
				new { name = "ISBN", type = "text", size = 50, mandatory = true},
				new { name = "Autor", type = "text", size = 50, mandatory = true},
				new { name = "Editora", type = "text", size = 50, mandatory = true},
				new { name = "Ano", type = "text", size = 50, mandatory = true},
				new { name = "Imagem", type = "file", mandatory = true},
				new { name = "Quantidade (opcional)", type = "number", mandatory = false},


			};
			MethodsList["UpdateImage"] = new List<object> {
				new { name = "ID da obra a adicionar", type = "number", mandatory = true},
				new { name = "Imagem", type = "file", mandatory = true},
				new { name = "ISBN", type = "text", size = 50, mandatory = true}
			};
			MethodsList["GetCentralNucleo"] = null;
			MethodsList["AddObraInNucleo"] = new List<object> {
				new { name = "ID da obra", type = "number", mandatory = true},
				new { name = "Núcleo a adicionar", type = "number", mandatory = true},
				new { name = "Quantidade", type = "number", mandatory = true},
			};
			MethodsList["RemoveObra"] = new List<object> {
				new { name = "ID da obra a remover", type = "number", mandatory = true},
				new { name = "Núcleo a remover", type = "number", mandatory = true},
				new { name = "Quantidade", type = "number", mandatory = true},
			};
			MethodsList["TransferObra"] = new List<object> {
				new { name = "ID da obra a transferir", type = "number", mandatory = true},
				new { name = "Núcleo de origem", type = "number", mandatory = true},
				new { name = "Núcleo de destino", type = "number", mandatory = true},
				new { name = "Quantidade", type = "number", mandatory = true},

			};
			MethodsList["InsertLeitor"] = new List<object> {
				new { name = "Nome do utilizador", type = "text", size = 50, mandatory = true},
				new { name = "Morada", type = "text", size = 50, mandatory = true},
				new { name = "Telemóvel", type = "number", size = 9, mandatory = true},
				new { name = "Email", type = "email", size = 50, mandatory = true},
				new { name = "Password", type = "password", size = 50, mandatory = true},
				new { name = "Tipo de utilizador", type = "radio", mandatory = true},
			};
			MethodsList["SuspendLateLeitor"] = new List<object> {
				new { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["GetRequisicoesLeitor"] = new List<object> {
				new { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["Devolution"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},
				new { name = "ID da obra a devolver", type = "number", mandatory = true},
				new { name = "Núcleo", type = "number", mandatory = true},
			};
			MethodsList["Requisition"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},
				new { name = "ID da obra a devolver", type = "number", mandatory = true},
				new { name = "Núcleo", type = "number", mandatory = true},
			};
			MethodsList["sp_leitor_reactivate"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},

			};
			MethodsList["sp_delete_inactive_Leitor"] = new List<object>
			{
				//NOT DONE, NEEDS CLARIFICATION
			};
			MethodsList["sp_del_leitor"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["sp_save_leitor_history"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["sp_cancel_leitor"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["sp_search_Obra"] = new List<object>
			{
				new { name = "Obra a pesquisar", type = "text", size = 50, mandatory = true},
			};

			MethodsList["sp_search_Obra_genero"] = new List<object>
			{
				new { name = "Género a pesquisar", type = "text", size = 50, mandatory = true},
			};

			MethodsList["requesicao_status"] = new List<object>
			{
				new { name = "ID do leitor", type = "number", mandatory = true},
				new { name = "Núcleo", type = "number", mandatory = false},

			};
			MethodsDict = MethodsList;
			MethodsJson = JsonSerializer.Serialize(MethodsList);

		}

		//public object ResolveMethod (string method, object response)
		//{
		//	string val = method + " was called and recieved response, reflection still needed"
		//	return val;
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
