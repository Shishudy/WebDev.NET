using System.Numerics;

namespace TesteApp
{
    internal class Program
    {
		public class Model
		{
			static List<(string description, string type, int max_size)> GetData()
			{
				return new List<(string, string, int)>
				{
					("name", "string", 20),
					("password", "string", 20),
					("email", "string", 20),
					("phone", "number", 20)
				};
			}

			public string Register(string name, string password, string email, int phone, string address)
			{

			}

			public void teste()
			{
				var i = Register();
				foreach (var n in i)
					foreach (var s in n)
						Console.WriteLine(n);
			}
		}
		static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
			Model i = new Model();
			i.teste();
        }
    }
}
