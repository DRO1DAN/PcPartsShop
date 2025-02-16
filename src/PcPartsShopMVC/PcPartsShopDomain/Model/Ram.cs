using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Ram : Entity
{
    public string Name { get; set; } = null!;

    public long Capacity { get; set; }

    public long Speed { get; set; }

    public string Type { get; set; } = null!;

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
