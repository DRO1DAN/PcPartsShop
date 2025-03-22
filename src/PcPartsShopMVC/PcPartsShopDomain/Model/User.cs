using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class User : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
