using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Storage
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public long Capacity { get; set; }

    public string Interface { get; set; } = null!;

    public string MemoryType { get; set; } = null!;

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
