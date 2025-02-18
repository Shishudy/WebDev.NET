using System;
using System.Collections.Generic;

namespace LibEF.Models;

public partial class Obra
{
    public int PkObra { get; set; }

    public string NomeObra { get; set; } = null!;

    public string? Isbn { get; set; }

    public string? Editora { get; set; }

    public int Ano { get; set; }

    public int? FkImagem { get; set; }

    public virtual ImageReference? FkImagemNavigation { get; set; }

    public virtual ICollection<NucleoObra> NucleoObras { get; set; } = new List<NucleoObra>();

    public virtual ICollection<Requisicao> Requisicaos { get; set; } = new List<Requisicao>();

    public virtual ICollection<Autor> PkAutors { get; set; } = new List<Autor>();

    public virtual ICollection<Genero> PkGeneros { get; set; } = new List<Genero>();
}
