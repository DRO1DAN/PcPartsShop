using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Cart : Entity
{
    public string UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; } = null!;
}