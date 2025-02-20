using System;
using System.Collections.Generic;

namespace LibEF.Models;

public partial class Leitor
{
    public int PkLeitor { get; set; }

    public DateTime? DataInscricao { get; set; }

    public string UserPassword { get; set; } = null!;

    public string? UserRole { get; set; }

    public string? Stat { get; set; }

    public string NomeLeitor { get; set; } = null!;

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? Morada { get; set; }

    public virtual ICollection<Requisicao> Requisicaos { get; set; } = new List<Requisicao>();
}
