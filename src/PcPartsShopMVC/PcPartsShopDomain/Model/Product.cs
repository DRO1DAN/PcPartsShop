using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PcPartsShopDomain.Model;

public partial class Product : Entity
{
    [Required(ErrorMessage = "The field \"Product Name\" cannot be empty")]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "The field \"Price\"cannot be empty")]

    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "The field \"Category\" cannot be empty")]

    [Display(Name = "Category")]
    public string Category { get; set; } = null!;

    [Required(ErrorMessage = "The field \"Brand\" cannot be empty")]
    public long BrandId { get; set; }

    [Display(Name = "Brand")]
    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<ComputerCase> Cases { get; set; } = new List<ComputerCase>();

    public virtual ICollection<Cpu> Cpus { get; set; } = new List<Cpu>();

    public virtual ICollection<Gpu> Gpus { get; set; } = new List<Gpu>();

    public virtual ICollection<Motherboard> Motherboards { get; set; } = new List<Motherboard>();

    public virtual ICollection<Psu> Psus { get; set; } = new List<Psu>();

    public virtual ICollection<Ram> Rams { get; set; } = new List<Ram>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
