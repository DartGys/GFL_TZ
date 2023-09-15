using System;
using System.Collections.Generic;

namespace GFL_TZ.Models;

public partial class Catalogy
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ForeignKey { get; set; }
}
