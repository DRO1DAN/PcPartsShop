using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Psu : Entity
{
    public string Name { get; set; } = null!;

    public long Wattage { get; set; }

    public bool Modular { get; set; }

    public string Efficiency { get; set; } = null!;

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
