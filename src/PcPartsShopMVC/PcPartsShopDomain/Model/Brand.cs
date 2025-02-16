﻿using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Brand
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
