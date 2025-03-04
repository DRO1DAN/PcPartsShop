using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PcPartsShopDomain.Model;

public partial class Product : Entity
{
    [Required(ErrorMessage = "Поле \"Назва комплектуючих\" не має бути порожнім")]
    [Display(Name = "Назва комплектуючих")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле \"Ціна\" не має бути порожнім")]

    [Display(Name = "Ціна")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Поле \"Категорія\" не має бути порожнім")]

    [Display(Name = "Категорія")]
    public string Category { get; set; } = null!;

    [Required(ErrorMessage = "Поле \"Бренд\" не має бути порожнім")]
    public long BrandId { get; set; }

    [Display(Name = "Бренд")]
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
