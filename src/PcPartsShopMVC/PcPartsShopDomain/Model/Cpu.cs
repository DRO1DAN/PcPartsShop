using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Cpu
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Series { get; set; } = null!;

    public string Generation { get; set; } = null!;

    public int Cores { get; set; }

    public int Threads { get; set; }

    public decimal BaseClock { get; set; }

    public decimal BoostClock { get; set; }

    public long Cache { get; set; }

    public string SupportedMemoryType { get; set; } = null!;

    public string Socket { get; set; } = null!;

    public long Tdp { get; set; }

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
