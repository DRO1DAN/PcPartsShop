using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PcPartsShopDomain.Model;

public partial class Storage : Entity
{
    //[Display(Name = "Накопичувач")]
    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public long Capacity { get; set; }

    public string Interface { get; set; } = null!;

    public string MemoryType { get; set; } = null!;

    public long ProductId { get; set; }

    //[Display(Name = "Накопичувач")]
    public virtual Product Product { get; set; } = null!;
}
