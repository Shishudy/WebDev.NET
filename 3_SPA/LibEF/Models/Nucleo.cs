using System;
using System.Collections.Generic;

namespace LibEF.Models;

public partial class Nucleo
{
    public int PkNucleo { get; set; }

    public int? FkCentral { get; set; }

    public string NomeNucleo { get; set; } = null!;

    public string? Morada { get; set; }

    public string? Telefone { get; set; }

    public virtual Nucleo? FkCentralNavigation { get; set; }

    public virtual ICollection<Nucleo> InverseFkCentralNavigation { get; set; } = new List<Nucleo>();

    public virtual ICollection<NucleoObra> NucleoObras { get; set; } = new List<NucleoObra>();
}
