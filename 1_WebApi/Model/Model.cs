using Azure;
using LibEF.Models;
using LibEF;
using System.Text.Json;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;

//TODO make overall / Utilizadores / nucleos / obras pages
//each will auto show all values and will have an search bar // bonus maybe search from what is shown?

//TODO add JWT token auth
//TODO Seperate stuff into classes
//TODO list EF_methods really needed

namespace WebAPI.Model
{
	public class Model
	{

		private readonly ProjectoContext _context;
		private readonly methodsMapping _map_method;

		public Model(string conn_str)
		{
			_context = CreateContext(conn_str);
			_map_method = new methodsMapping();
		}

		private ProjectoContext CreateContext(string connectionString)
		{
			var optionsBuilder = new DbContextOptionsBuilder<ProjectoContext>();
			optionsBuilder.UseSqlServer(connectionString);
			return new ProjectoContext(optionsBuilder.Options);
		}

		public object MethodCaller (string description, JsonElement param)
		{
			string method = _map_method.searchbyDescrition(description);
			Console.WriteLine(method);
			var methods_dic = _map_method.MethodsDict;
			if (!methods_dic.ContainsKey(method))
				throw new Exception("no such method listed.");
			var ParamList = _map_method.GetParamList(method, param);
			// retorna os parametros necessarios
			if ((ParamList == null || ParamList.Count == 0) && methods_dic[method] != null)
				return Results.Ok(JsonSerializer.Serialize(methods_dic[method]));
			// vai buscar a lista
			var result = ResolveMethod(method, ParamList);
			return (result); // se nao for lista, nao paginar
		}

		public object paginationrow(object result, string page, string size)
		{
			var pagedResult = result as List<object>;
			if (pagedResult == null)
				return pagedResult;
			int count = pagedResult.Count;
			// separar pacotes a enviar
			int sizeInt = int.Parse(size);
			int pageInt = int.Parse(page) * sizeInt;
			pagedResult = pagedResult.Skip(pageInt * sizeInt).Take(sizeInt).ToList();
			var response = new Dictionary<string, object>
			{
				{ "page", count },
				{ "table", pagedResult }
			};
			return response;
		}

		public object ResolveMethod (string method, List <object>? ParamList)
		{
			EF_methods ef = new EF_methods(_context);
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
				return false;
			}
		}

		public List<dynamic> GetData(string tab, string data)
		{
			ProjectoContext context = new ProjectoContext();
			EF_methods ef = new EF_methods(_context);
			if (tab == "reservas")
				return ef.GetAllRequisicoes();
			else if (tab == "obras")
				return ef.GetAllObras();
			else if (tab == "nucleos")
				return ef.GetAllNucleos();
			else if (tab == "utilizadores")
				return ef.GetAllLeitores();
		}
	}
}
