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

        public Dictionary<string, object>? Obra { get; set; }
        public int PkLeitor { get; set; }

        public IActionResult OnGet(int pk_obra)
        {
            int? idUsuario = HttpContext.Session.GetInt32("PkLeitor");

            if (idUsuario == null)
            {
                Console.WriteLine(" ERRO: PkLeitor não encontrado na sessão. Redirecionando para Index.");
                return RedirectToPage("/Index");
            }

            Console.WriteLine($" PkLeitor encontrado na sessão: {idUsuario.Value}");

            PkLeitor = idUsuario.Value;
            Obra = Method.GetBookDetails(_connectionString, pk_obra);

            if (Obra == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int pk_obra, int pk_leitor, int pk_nucleo)
        {
            try
            {
                Console.WriteLine($"?? Tentando realizar requisição: PkLeitor={pk_leitor}, PkObra={pk_obra}, PkNucleo={pk_nucleo}");

                Method.ProcessRequisition(_connectionString, pk_leitor, pk_obra, pk_nucleo);
                TempData["SuccessMessage"] = " Requisição realizada com sucesso!";

                Console.WriteLine("Requisição concluída com sucesso!");

                return RedirectToPage("/Requisitions");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $" Erro ao requisitar livro: {ex.Message}";
                Console.WriteLine($" ERRO: {ex.Message}");
                return Page();
            }
        }

    }
}
