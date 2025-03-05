using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibADO.RequisitionMake;
using System;
using System.Collections.Generic;

namespace UserMPA.Pages
{
    public class RequisitarModel : PageModel
    {
        private readonly string _connectionString;

        public List<Dictionary<string, object>> ObrasNosNucleos { get; set; } = new();
        public int PkLeitor { get; set; }

        public RequisitarModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IActionResult OnGet(int pk_obra)
        {
            int? idUsuario = HttpContext.Session.GetInt32("PkLeitor");

            if (idUsuario == null)
            {
                return RedirectToPage("/Index");
            }

            PkLeitor = idUsuario.Value;

            ObrasNosNucleos = Method.GetBookDetails(_connectionString, pk_obra);

            if (ObrasNosNucleos == null || ObrasNosNucleos.Count == 0)
            {
                TempData["ErrorMessage"] = "Esta obra n�o est� dispon�vel para requisi��o no momento.";
                return RedirectToPage("/Search");
            }

            return Page();
        }

        public IActionResult OnPost(int pk_obra, int pk_leitor, int pk_nucleo)
        {
            try
            {
                var detalhesObra = Method.GetBookDetails(_connectionString, pk_obra);
                if (detalhesObra == null || detalhesObra.Count == 0)
                {
                    TempData["ErrorMessage"] = "Esta obra n�o pode ser requisitada, pois n�o est� associada a um n�cleo.";
                    return RedirectToPage("/Search");
                }
                Method.ProcessRequisition(_connectionString, pk_leitor, pk_obra, pk_nucleo);
                TempData["SuccessMessage"] = "Requisi��o realizada com sucesso!";

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

                return RedirectToPage("/Search");
            }
        }
    }
}
