using System;
using System.Collections.Generic;

namespace LibEF.Models;

public partial class LookupTable
{
    public int PkLookup { get; set; }

    public int Categoty { get; set; }

    public string SpAdoNet { get; set; } = null!;

    public string SpEfcore { get; set; } = null!;

    public int? NParams { get; set; }

    public string? NameParams { get; set; }

    public string? Description { get; set; }
}
