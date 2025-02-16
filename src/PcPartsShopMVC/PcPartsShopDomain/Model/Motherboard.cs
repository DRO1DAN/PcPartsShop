using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Motherboard : Entity
{
    public string Name { get; set; } = null!;

    public string Socket { get; set; } = null!;

    public string Chipset { get; set; } = null!;

    public string FormFactor { get; set; } = null!;

    public int MemorySlotsCount { get; set; }

    public string MemoryType { get; set; } = null!;

    public long MaxMemory { get; set; }

    public string PcieVersion { get; set; } = null!;

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
