using Azure;
using LibEF.Models;
using LibEF;
using System.Text.Json;
using System.Reflection;

//TODO make overall / Utilizadores / nucleos / obras pages
//each will auto show all values and will have an search bar // bonus maybe search from what is shown?

//TODO add JWT token auth
//TODO Seperate stuff into classes
//TODO list EF_methods really needed

namespace WebAPI.Model
{
	public class MethodParameter
	{
		public string name { get; set; }
		public string type { get; set; }
		public int? size { get; set; }
		public bool mandatory { get; set; }
	}

	public class Model
	{
		public WebApplication App { get; set; }

		public readonly string MethodsJson;

		public readonly Dictionary<string, List<MethodParameter>> MethodsDict;

		public Model(WebApplication AppBuilt)
		{
			App = AppBuilt;
			Dictionary<string, List<MethodParameter>>MethodsList = new Dictionary<string, List<MethodParameter>>();
			MethodsList["GetTotalObra"] = null;
			MethodsList["GetTotalObraPorGenero"] = null;
			MethodsList["GetTopRequestedByTime"] = new List<MethodParameter> { 
				new MethodParameter { name = "Data início", type = "date", size = 0, mandatory = true}, 
				new MethodParameter { name = "Data fim", type = "date", size = 0, mandatory = true} 
			};
			MethodsList["GetRequisicoesByNucleo"] = new List<MethodParameter> { 
				new MethodParameter { name = "Data início", type = "date", size = 0, mandatory = true}, 
				new MethodParameter { name = "Data fim", type = "date", size = 0, mandatory = true}
			};
			MethodsList["InsertObra"] = new List<MethodParameter> {
				new MethodParameter { name = "Núcleo a adicionar (opcional)", type = "number", mandatory = false},
				new MethodParameter { name = "Nome da obra", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "ISBN", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Autor", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Editora", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Ano", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Imagem", type = "file", mandatory = true},
				new MethodParameter { name = "Quantidade (opcional)", type = "number", mandatory = false},

			};
			MethodsList["UpdateImage"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra a adicionar", type = "number", mandatory = true},
				new MethodParameter { name = "Imagem", type = "file", mandatory = true},
				new MethodParameter { name = "ISBN", type = "text", size = 50, mandatory = true}
			};
			MethodsList["GetCentralNucleo"] = null;
			MethodsList["AddObraInNucleo"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo a adicionar", type = "number", mandatory = true},
				new MethodParameter { name = "Quantidade", type = "number", mandatory = true},
			};
			MethodsList["RemoveObra"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra a remover", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo a remover", type = "number", mandatory = true},
				new MethodParameter { name = "Quantidade", type = "number", mandatory = true},
			};
			MethodsList["TransferObra"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra a transferir", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo de origem", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo de destino", type = "number", mandatory = true},
				new MethodParameter { name = "Quantidade", type = "number", mandatory = true},

			};
			MethodsList["InsertLeitor"] = new List<MethodParameter> {
				new MethodParameter { name = "Nome do utilizador", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Morada", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Telemóvel", type = "number", size = 9, mandatory = true},
				new MethodParameter { name = "Email", type = "email", size = 50, mandatory = true},
				new MethodParameter { name = "Password", type = "password", size = 50, mandatory = true},
				new MethodParameter { name = "Tipo de utilizador", type = "radio", mandatory = true},
			};
			MethodsList["SuspendLateLeitor"] = new List<MethodParameter> {
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["GetRequisicoesLeitor"] = new List<MethodParameter> {
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["Devolution"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
				new MethodParameter { name = "ID da obra a devolver", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo", type = "number", mandatory = true},
			};
			MethodsList["Requisition"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
				new MethodParameter { name = "ID da obra a devolver", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo", type = "number", mandatory = true},
			};
			MethodsList["sp_leitor_reactivate"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},

			};
			MethodsList["sp_delete_inactive_Leitor"] = new List<MethodParameter>
			{
				//NOT DONE, NEEDS CLARIFICATION
			};
			MethodsList["sp_del_leitor"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["sp_save_leitor_history"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["sp_cancel_leitor"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsList["sp_search_Obra"] = new List<MethodParameter>
			{
				new MethodParameter { name = "Obra a pesquisar", type = "text", size = 50, mandatory = true},
			};

			MethodsList["sp_search_Obra_genero"] = new List<MethodParameter>
			{
				new MethodParameter { name = "Género a pesquisar", type = "text", size = 50, mandatory = true},
			};

			MethodsList["requesicao_status"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo", type = "number", mandatory = false},

			};
			MethodsDict = MethodsList;
			MethodsJson = JsonSerializer.Serialize(MethodsList);

		}

		public List <object>? GetParamList (string method, JsonElement param)
		{
			if (param.ValueKind == JsonValueKind.Undefined || param.ValueKind == JsonValueKind.Null)
			{
				return null;
			}
			var paramDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(param.GetRawText());
            var paramList = new List<object>();
            foreach (var item in MethodsDict[method])
            {
                var paramName = item.GetType().GetProperty("name")?.GetValue(item)?.ToString();
                var paramType = item.GetType().GetProperty("type")?.GetValue(item)?.ToString();

                if (paramName != null && paramDict.ContainsKey(paramName))
                {
                    switch (paramType)
                    {
                        case "number":
                            paramList.Add(paramDict[paramName].GetInt32());
                            break;
                        case "text":
                        case "email":
                        case "password":
                        case "radio":
                            paramList.Add(paramDict[paramName].GetString());
                            break;
                        case "date":
                            paramList.Add(paramDict[paramName].GetDateTime());
                            break;
                        default:
                            paramList.Add(paramDict[paramName].ToString());
                            break;
                    }
                }
				else
					paramList.Add(null);
            }
			foreach (var val in paramList)
				Console.WriteLine(val ?? "null");
			return paramList;
        }

        public object ResolveMethod (string method, List <object>? ParamList)
		{
            ProjectoContext context = new ProjectoContext();
            EF_methods ef = new EF_methods(context);
            Type type = typeof(EF_methods);
			MethodInfo? method_fun = type.GetMethod(method);
			try
			{
				if (method_fun != null)
				{
					if (ParamList != null && ParamList.Count > 0)
						return method_fun.Invoke(ef, ParamList.ToArray());  // Call the method with multiple arguments
					else
						return method_fun.Invoke(ef, null);
				}
				else 
					throw new Exception("failed, no such method");
			}
			catch (TargetInvocationException ex)
			{
				throw new Exception(ex.InnerException?.Message ?? ex.Message);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

        public bool Login(JsonElement jsonRes)
		{
            ProjectoContext context = new ProjectoContext();
            EF_methods ef = new EF_methods(context);
			try
			{
				Dictionary<string, string> res = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
				string pw_res = ef.GetPassWordbyLogin(res["email"]);
				string pw_login = res["password"];
				if (pw_login != pw_res)
				{
					throw new Exception("Invalid Password");
				}	
				return true;
            }
			catch
			{
				throw;
			}
				
        }
        //public string Login(Object response)
        //{
        //	ProjectoContext context = new ProjectoContext();
        //	EF_methods ef = new EF_methods(context);
        //	string jsonRes = JsonSerializer.Serialize(response);
        //	Dictionary<string, string> res = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
        //	try
        //	{
        //		var forecast = ef.GetPassWordbyLogin(res.GetValueOrDefault("username"));
        //		var obj = new { result = forecast};
        //		var json = JsonSerializer.Serialize(obj);
        //		return (json);
        //	}
        //	catch
        //	{
        //		var obj = new { result = "User not found."};
        //		var json = JsonSerializer.Serialize(obj);
        //		return (json);
        //	}
        //}
    }
}
