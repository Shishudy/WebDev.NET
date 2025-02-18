using System;
using System.Collections.Generic;

namespace LibEF.Models;

public partial class ImageReference
{
    public int PkImage { get; set; }

    public string? ImageName { get; set; }

    public string? ImagePath { get; set; }

    public byte[]? ImageData { get; set; }

    public virtual ICollection<Obra> Obras { get; set; } = new List<Obra>();
}
