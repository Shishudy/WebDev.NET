using Azure;
using LibEF.Models;
using LibEF;
using System.Text.Json;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

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

	public class MethodDetails
    {
        public string Description { get; set; }
        public string MethodName { get; set; }
    }

	public class Model
	{
		private readonly ProjectoContext _context;
		public readonly Dictionary<string, Dictionary<string, MethodDetails>> method_category = new Dictionary<string, Dictionary<string, MethodDetails>>();
        public Dictionary<string, List<MethodParameter>> MethodsDict = new Dictionary<string, List<MethodParameter>>();
        public readonly string MethodsJson;
		public void MethodCategory ()
		{
			method_category["Obras"] = new Dictionary<string, MethodDetails>
			{
				["get"] = new MethodDetails{
					Description = "Numero total de obras", 
					MethodName = "GetTotalObra",
				},
				["get"] = new MethodDetails{
					Description = "Get Obras by genre", 
					MethodName = "GetTotalObraPorGenero",
				},
				["add"] = new MethodDetails{
					Description = "Insert Obra", 
					MethodName = "InsertObra",
				},
				["update"] = new MethodDetails{
					Description = "Update Obra", 
					MethodName = "UpdateObra",
				
				},
				["delete"] = new MethodDetails{
					Description = "Delete Obra", 
					MethodName = "DeleteObra",
					
				},
				["search"] = new MethodDetails{
					Description = "Search Obra", 
					MethodName = "sp_search_Obra_genero",
					
				},
			};
			method_category["Nucleos"] = new Dictionary<string, MethodDetails>
			{
				["get"] = new MethodDetails{
					Description = "Get all Nucleos", 
					MethodName = "GetAllNucleos",
				},
				["get"] = new MethodDetails{
					Description = "Get Nucleo by ID", 
					MethodName = "GetNucleoByID",
				},
				["get"] = new MethodDetails{
					Description = "Get Nucleo by Name", 
					MethodName = "GetNucleoByName",
				},
				["add"] = new MethodDetails{
					Description = "Insert Núcleo", 
					MethodName = "InsertNucleo",
				},
				["add"] = new MethodDetails{
					Description = "Add Obra to Núcleo", 
					MethodName = "AddObraInNucleo",

				},
				["remove"] = new MethodDetails{
					Description = "Remove Obra from Núcleo", 
					MethodName = "RemoveObra",
				},
				["transfer"] = new MethodDetails{
					Description = "Transfer Obra", 
					MethodName = "TransferObra",
				},
			};

			method_category["Reservas"] = new Dictionary<string, MethodDetails>
			{
				["get"] = new MethodDetails{
					Description = "Get all Reservas", 
					MethodName = "GetAllReservas",
				},
				["get"] = new MethodDetails{
					Description = "Get Reservas by Leitor", 
					MethodName = "GetReservasByLeitor",
				},
				["get"] = new MethodDetails{
					Description = "Get Reservas by nucleo", 
					MethodName = "GetRequisicoesByNucleo",
				},
				["get"] = new MethodDetails{
					Description = "Get Reservas by Obra", 
					MethodName = "GetReservasByObra",
				},
				["add"] = new MethodDetails{
					Description = "Insert Reserva", 
					MethodName = "InsertReserva",
				},
				["delete"] = new MethodDetails{
					Description = "Delete Reserva", 
					MethodName = "DeleteReserva",
				},
				["get"] = new MethodDetails{
					Description = "Get Reservas by Time", 
					MethodName = "GetReservasByTime",
				},
				["update"] = new MethodDetails{
					Description = "Devolution", 
					MethodName = "Devolution",
				},
				["update"] = new MethodDetails{
					Description = "Requisition", 
					MethodName = "Requisition",
				},
			};

			method_category["Utilizadores"] = new Dictionary<string, MethodDetails>
			{
				["get"] = new MethodDetails{
					Description = "Get all Utilizadores", 
					MethodName = "GetAllUtilizadores",
				},
				["get"] = new MethodDetails{
					Description = "Get requesitions status", 
					MethodName = "requesicao_status",
				},
				["add"] = new MethodDetails{
					Description = "Insert Utilizador", 
					MethodName = "InsertLeitor",
				},
				["update"] = new MethodDetails{
					Description = "Suspender Utilizador", 
					MethodName = "SuspendLeitor",
				},
				["update"] = new MethodDetails{
					Description = "Reactivar Utilizador", 
					MethodName = "sp_leitor_reactivate",
				},
				["update"] = new MethodDetails{
					Description = "Get Requisições do Leitor", 
					MethodName = "GetRequisicoesLeitor",
				},
				["delete"] = new MethodDetails{
					Description = "sp_delete_inactive_Leitor", 
					MethodName = "sp_delete_inactive_Leitor",
				},
				["delete"] = new MethodDetails{
					Description = "sp_del_leitor", 
					MethodName = "sp_del_leitor",
				},
				["update"] = new MethodDetails{
					Description = "sp_save_leitor_history", 
					MethodName = "sp_save_leitor_history",
				},
				["update"] = new MethodDetails{
					Description = "sp_cancel_leitor", 
					MethodName = "sp_cancel_leitor",
				},
			};
		}

		public Model(string connectionString)
		{
            _context = CreateContext(connectionString);
			MethodCategory();
			MethodsDict["GetTotalObra"] = null;
			MethodsDict["GetTotalObraPorGenero"] = null;
			MethodsDict["GetTopRequestedByTime"] = new List<MethodParameter> { 
				new MethodParameter { name = "Data início", type = "date", size = 0, mandatory = true}, 
				new MethodParameter { name = "Data fim", type = "date", size = 0, mandatory = true} 
			};
			MethodsDict["GetRequisicoesByNucleo"] = new List<MethodParameter> { 
				new MethodParameter { name = "Data início", type = "date", size = 0, mandatory = true}, 
				new MethodParameter { name = "Data fim", type = "date", size = 0, mandatory = true}
			};
			MethodsDict["InsertObra"] = new List<MethodParameter> {
				new MethodParameter { name = "Núcleo a adicionar (opcional)", type = "number", mandatory = false},
				new MethodParameter { name = "Nome da obra", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "ISBN", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Autor", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Editora", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Ano", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Imagem", type = "file", mandatory = true},
				new MethodParameter { name = "Quantidade (opcional)", type = "number", mandatory = false},

			};
			MethodsDict["UpdateImage"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra a adicionar", type = "number", mandatory = true},
				new MethodParameter { name = "Imagem", type = "file", mandatory = true},
				new MethodParameter { name = "ISBN", type = "text", size = 50, mandatory = true}
			};
			MethodsDict["GetCentralNucleo"] = null;
			MethodsDict["AddObraInNucleo"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo a adicionar", type = "number", mandatory = true},
				new MethodParameter { name = "Quantidade", type = "number", mandatory = true},
			};
			MethodsDict["RemoveObra"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra a remover", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo a remover", type = "number", mandatory = true},
				new MethodParameter { name = "Quantidade", type = "number", mandatory = true},
			};
			MethodsDict["TransferObra"] = new List<MethodParameter> {
				new MethodParameter { name = "ID da obra a transferir", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo de origem", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo de destino", type = "number", mandatory = true},
				new MethodParameter { name = "Quantidade", type = "number", mandatory = true},

			};
			MethodsDict["InsertLeitor"] = new List<MethodParameter> {
				new MethodParameter { name = "Nome do utilizador", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Morada", type = "text", size = 50, mandatory = true},
				new MethodParameter { name = "Telemóvel", type = "number", size = 9, mandatory = true},
				new MethodParameter { name = "Email", type = "email", size = 50, mandatory = true},
				new MethodParameter { name = "Password", type = "password", size = 50, mandatory = true},
				new MethodParameter { name = "Tipo de utilizador", type = "radio", mandatory = true},
			};
			MethodsDict["SuspendLateLeitor"] = new List<MethodParameter> {
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsDict["GetRequisicoesLeitor"] = new List<MethodParameter> {
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsDict["Devolution"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
				new MethodParameter { name = "ID da obra a devolver", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo", type = "number", mandatory = true},
			};
			MethodsDict["Requisition"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
				new MethodParameter { name = "ID da obra a devolver", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo", type = "number", mandatory = true},
			};
			MethodsDict["sp_leitor_reactivate"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},

			};
			MethodsDict["sp_delete_inactive_Leitor"] = null;
			MethodsDict["sp_del_leitor"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsDict["sp_save_leitor_history"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsDict["sp_cancel_leitor"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
			};
			MethodsDict["sp_search_Obra"] = new List<MethodParameter>
			{
				new MethodParameter { name = "Obra a pesquisar", type = "text", size = 50, mandatory = true},
			};

			MethodsDict["sp_search_Obra_genero"] = new List<MethodParameter>
			{
				new MethodParameter { name = "Género a pesquisar", type = "text", size = 50, mandatory = true},
			};

			MethodsDict["requesicao_status"] = new List<MethodParameter>
			{
				new MethodParameter { name = "ID do leitor", type = "number", mandatory = true},
				new MethodParameter { name = "Núcleo", type = "number", mandatory = false},

			};
			MethodsJson = JsonSerializer.Serialize(MethodsDict);
		}

		public void AddMethod(string category, string method, string description, List<MethodParameter> parameters)
		{
			if (!method_category.ContainsKey(category))
			{
				method_category[category] = new Dictionary<string, MethodDetails>();
			}
			method_category[category][method] = new MethodDetails { Description = description, MethodName = method };
			MethodsDict[method] = parameters;
		}

		// so i want an method where i pass the category and get back an list of methods with their method name as key and parameters as value

	public Dictionary<string, List<MethodParameter>> GetMethods(string level)
	{
		if (level != "all" && !method_category.ContainsKey(level))
			throw new Exception("level not found try any:" + string.Join(", ", method_category.Keys));
		var result = new Dictionary<string, List<MethodParameter>>();
		List<dynamic> methods = new List<dynamic>();
        if (level == "all")
        {
			foreach (var cat in method_category.Values)
			{
				methods.AddRange(cat.Values);
			}
        }
        else if (method_category.ContainsKey(level))
			methods = method_category[level].Values.ToList<dynamic>();
		foreach (var method in methods)
		{
			if (MethodsDict.ContainsKey(method.MethodName))
				result[method.MethodName] = MethodsDict[method.MethodName];
		}
		return result;
	}

        //Dictionary<string, Dictionary<string, MethodDetails>>()
        public Dictionary<string, List<MethodParameter>> GetMethods(string level, string cat)
        {
            if (level != "all" && !method_category.ContainsKey(level))
                throw new Exception("level not found try any:" + string.Join(", ", method_category.Keys));
            Dictionary<string, MethodDetails> methods = new Dictionary<string, MethodDetails>();
            if (level == "all")
            {
                foreach (var meth in method_category)
                {
                    foreach (var method in meth.Value)
                    {
                        methods[method.Key] = method.Value;
                    }
                }
            }
            else if (method_category.ContainsKey(level))
                methods = method_category[level];
            var result = new Dictionary<string, List<MethodParameter>>();
            foreach (var method in methods)
            {
                if (cat != "all" && method.Key != cat)
                    continue;
                if (MethodsDict.ContainsKey(method.Value.MethodName))
                    result[method.Value.MethodName] = MethodsDict[method.Value.MethodName];
            }
			if (result.Count == 0)
				throw new Exception("No methods found for <" + cat + "> try: " + string.Join(", ", methods.Select(x => x.Key)));
            return result;
        }
        private ProjectoContext CreateContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectoContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new ProjectoContext(optionsBuilder.Options);
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
			foreach (var val in paramList) //TODO remove degub lines
				Console.WriteLine(val ?? "null");
			return paramList;
        }

        public object ResolveMethod (string method, List <object>? ParamList)
		{
            EF_methods ef = new EF_methods(_context);
            Type type = typeof(EF_methods);
			MethodInfo? method_fun = type.GetMethod(method);
			try
			{
				if (method_fun != null){
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
            EF_methods ef = new EF_methods(_context);
			try
			{
				Dictionary<string, string> res = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
				string pw_res = ef.GetPassWordbyLogin(res["email"]);
				string pw_login = res["password"];
				if (pw_login != pw_res){
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
