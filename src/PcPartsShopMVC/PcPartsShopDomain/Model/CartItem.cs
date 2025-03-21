﻿using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class CartItem : Entity
{
    public long CartId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
