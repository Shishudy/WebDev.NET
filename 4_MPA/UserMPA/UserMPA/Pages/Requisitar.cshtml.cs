using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibADO.RequisitionMake;
using System;
using System.Collections.Generic;

namespace UserMPA.Pages
{
    public class RequisitarModel : PageModel
    {
        private readonly string _connectionString = "Server=PC013562;Database=Projecto;Integrated Security=True;TrustServerCertificate=True;";

        public List<Dictionary<string, object>> ObrasNosNucleos { get; set; } = new();
        public int PkLeitor { get; set; }

        public IActionResult OnGet(int pk_obra)
        {
            Console.WriteLine($"DEBUG: Tentando carregar detalhes da obra com ID {pk_obra}");

            int? idUsuario = HttpContext.Session.GetInt32("PkLeitor");

            if (idUsuario == null)
            {
                Console.WriteLine("ERRO: PkLeitor n�o encontrado na sess�o. Redirecionando...");
                return RedirectToPage("/Index");
            }

            PkLeitor = idUsuario.Value;
            ObrasNosNucleos = Method.GetBookDetails(_connectionString, pk_obra);

            if (ObrasNosNucleos == null || ObrasNosNucleos.Count == 0)
            {
                Console.WriteLine($"ERRO: Nenhuma obra encontrada para pk_obra = {pk_obra}");
                return NotFound();
            }

            return Page();
        }


        public IActionResult OnPost(int pk_obra, int pk_leitor, int pk_nucleo)
        {
            try
            {
                Console.WriteLine($"?? Tentando realizar requisi��o: PkLeitor={pk_leitor}, PkObra={pk_obra}, PkNucleo={pk_nucleo}");

                Method.ProcessRequisition(_connectionString, pk_leitor, pk_obra, pk_nucleo);
                TempData["SuccessMessage"] = "Requisi��o realizada com sucesso!";

                Console.WriteLine("Requisi��o conclu�da com sucesso!");

                return RedirectToPage("/Requisitions");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("usu�rio j� possui 4 requisi��es")) 
                {
                    TempData["ErrorMessage"] = "Voc� j� atingiu o limite de 4 requisi��es!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Erro ao requisitar livro: {ex.Message}";
                }

                Console.WriteLine($"ERRO:{ex.Message}");
                return Page();
            }
        }
    }
}