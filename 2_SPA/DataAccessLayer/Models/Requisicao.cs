﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Requisicao
{
    public int PkLeitor { get; set; }

    public int PkObra { get; set; }

    public int PkNucleo { get; set; }

    public string? Stat { get; set; }

    public DateOnly? DataLevantamento { get; set; }

    public DateOnly? DataDevolucao { get; set; }

    public bool? AlreadySuspend { get; set; }

    public virtual Leitor PkLeitorNavigation { get; set; } = null!;

    public virtual Obra PkObraNavigation { get; set; } = null!;
}
