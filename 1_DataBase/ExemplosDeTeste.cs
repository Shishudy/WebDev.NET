using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfProcedures
{
    internal class ExemplosDeTeste
    {
        /* sp_search_obras :

        
        
 
     static void Main(string[] args)
{
 Console.WriteLine("Digite o nome da obra (ou pressione Enter para ignorar):");
 string obra = Console.ReadLine();

 Console.WriteLine("Digite o gênero da obra (ou pressione Enter para ignorar):");
 string genre = Console.ReadLine();

 Console.WriteLine("Digite o núcleo (ou pressione Enter para ignorar):");
 string nucleo = Console.ReadLine();

 TestarPesquisaObras(obra, genre, nucleo); 
        }

     static void TestarPesquisaObras(string obra, string genre, string nucleo)
{
 var resultados = Procedures.sp_search_obras(
 string.IsNullOrWhiteSpace(obra) ? null : obra,
 string.IsNullOrWhiteSpace(genre) ? null : genre,
 string.IsNullOrWhiteSpace(nucleo) ? null : nucleo
 );

 if (resultados.Count == 0)
 {
     Console.WriteLine("Nenhuma obra encontrada.");
     return;
 }

 Console.WriteLine("-------------------------------------------------------------------");
 Console.WriteLine("| ID   | Nome da Obra                 | Ano  | Editora           |");
 Console.WriteLine("-------------------------------------------------------------------");

 foreach (var obraItem in resultados)
 {
     Console.WriteLine($"| {obraItem.PkObra,-4} | {obraItem.NomeObra,-30} | {obraItem.Ano,-4} | {obraItem.Editora,-15} |");
 }

 Console.WriteLine("-------------------------------------------------------------------");
}
}
 } 
}
     */

/* TimesRequested :




static void Main(string[] args)
{
    Console.WriteLine("Digite a data de início (YYYY-MM-DD) ou pressione Enter para ignorar:");
    string inputStartDate = Console.ReadLine();

    Console.WriteLine("Digite a data de fim (YYYY-MM-DD) ou pressione Enter para ignorar:");
    string inputEndDate = Console.ReadLine();

    DateOnly? startDate = string.IsNullOrWhiteSpace(inputStartDate) ? null : DateOnly.Parse(inputStartDate);
    DateOnly? endDate = string.IsNullOrWhiteSpace(inputEndDate) ? null : DateOnly.Parse(inputEndDate);

    TestarTopObras(startDate, endDate);
}

static void TestarTopObras(DateOnly? startDate, DateOnly? endDate)
{
    var resultados = Procedures.sp_top_requested_by_time(startDate, endDate);

    if (resultados.Count == 0)
    {
        Console.WriteLine("Nenhuma obra encontrada.");
        return;
    }

    Console.WriteLine("-----------------------------------------------------");
    Console.WriteLine("| Rank | Nome da Obra                | Requisições |");
    Console.WriteLine("-----------------------------------------------------");

    int rank = 1;
    foreach (var (nomeObra, timesRequested) in resultados)
    {
        Console.WriteLine($"| {rank,-4} | {nomeObra,-25} | {timesRequested,-11} |");
        rank++;
    }

    Console.WriteLine("-----------------------------------------------------");
}

 */

/* sp_total_obras()


static void Main(string[] args)
{
    TestarTotalObras();
}

static void TestarTotalObras()
{
    int totalObras = Procedures.sp_total_obras();

    Console.WriteLine("=====================================");
    Console.WriteLine($"📚 Total de Obras no Sistema: {totalObras}");
    Console.WriteLine("=====================================");

}*/

/* sp_total_obras_por_genero()

static void Main(string[] args)
{
    TestarTotalObrasPorGenero();
}

static void TestarTotalObrasPorGenero()
{
    var totalPorGenero = Procedures.sp_total_obras_por_genero();

    Console.WriteLine("=========================================");
    Console.WriteLine("📊 Total de Obras por Gênero");
    Console.WriteLine("=========================================");
    foreach (var (nomeGenero, totalQuantidade) in totalPorGenero)
    {
        Console.WriteLine($"🎭 {nomeGenero}: {totalQuantidade}");
    }
    Console.WriteLine("=========================================");
}
*/

/* sp_transfer_obra :

static void Main(string[] args)
{
    int pkObra = 1;             
    int pkNucleoOrigem = 1;     
    int pkNucleoDestino = 2;   
    int quantidade = 2;        

    Console.WriteLine("=========================================");
    Console.WriteLine("🔄 Teste de Transferência de Obra");
    Console.WriteLine("=========================================");

    bool sucesso = Procedures.sp_transfer_obra(pkObra, pkNucleoOrigem, pkNucleoDestino, quantidade);

    if (sucesso)
    {
        Console.WriteLine($"✅ Transferência de {quantidade} unidade(s) da obra {pkObra} do núcleo {pkNucleoOrigem} para {pkNucleoDestino} realizada com sucesso!");
    }
    else
    {
        Console.WriteLine("❌ Falha ao transferir obra.");
    }

    Console.WriteLine("=========================================");
} */

/*   sp_update_image

static void Main(string[] args)
{
    int pkObra = 1;                          // Defina um ID válido de obra
    string imagePath = @"C:\imagens\livro.jpg"; // Caminho válido da imagem
    string isbn = "978-3-16-148410-0";       // Defina um ISBN válido

    Console.WriteLine("=========================================");
    Console.WriteLine("🖼️ Teste de Atualização de Imagem");
    Console.WriteLine("=========================================");

    bool sucesso = Procedures.sp_update_image(pkObra, imagePath, isbn);

    if (sucesso)
    {
        Console.WriteLine($"✅ Imagem da obra {pkObra} atualizada com sucesso! Novo caminho: {imagePath}");
    }
    else
    {
        Console.WriteLine("❌ Falha ao atualizar a imagem da obra.");
    }

    Console.WriteLine("=========================================");
} */

/* sp_suspend_late

static void Main(string[] args)
{
    int pkLeitor = 1; 

    Console.WriteLine("=========================================");
    Console.WriteLine("📌 Teste de Suspensão de Leitor");
    Console.WriteLine("=========================================");

    bool suspenso = Procedures.sp_suspend_late(pkLeitor);

    if (suspenso)
    {
        Console.WriteLine($"✅ Leitor {pkLeitor} foi suspenso por atrasos.");
    }
    else
    {
        Console.WriteLine($"⚠️ Leitor {pkLeitor} não foi suspenso.");
    }

    Console.WriteLine("=========================================");
} */

}
}
