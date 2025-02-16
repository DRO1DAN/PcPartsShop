using System;
using System.Collections.Generic;

namespace PcPartsShopDomain.Model;

public partial class Product
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public long BrandId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

    public virtual ICollection<Cpu> Cpus { get; set; } = new List<Cpu>();

    public virtual ICollection<Gpu> Gpus { get; set; } = new List<Gpu>();

    public virtual ICollection<Motherboard> Motherboards { get; set; } = new List<Motherboard>();

    public virtual ICollection<Psu> Psus { get; set; } = new List<Psu>();

    public virtual ICollection<Ram> Rams { get; set; } = new List<Ram>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
