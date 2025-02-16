using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Case : Entity
{
    public string Name { get; set; } = null!;

    public string FormFactor { get; set; } = null!;

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
