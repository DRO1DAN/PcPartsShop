using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Gpu
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Series { get; set; } = null!;

    public string Generation { get; set; } = null!;

    public long MemoryAmount { get; set; }

    public string MemoryType { get; set; } = null!;

    public decimal BaseClock { get; set; }

    public decimal BoostClock { get; set; }

    public string PcieVersion { get; set; } = null!;

    public int PcieCount { get; set; }

    public long Tdp { get; set; }

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
