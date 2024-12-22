using System;
using System.Collections.Generic;

namespace WebApp2.Models;

public partial class BrosShopColor
{
    public int ColorId { get; set; }

    public string ColorTitle { get; set; } = null!;

    public virtual ICollection<BrosShopProductAttribute> BrosShopProductAttributes { get; set; } = new List<BrosShopProductAttribute>();
}
