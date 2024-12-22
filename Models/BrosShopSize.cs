using System;
using System.Collections.Generic;

namespace WebApp2.Models;

public partial class BrosShopSize
{
    public int SizeId { get; set; }

    public string Size { get; set; } = null!;

    public virtual ICollection<BrosShopProductAttribute> BrosShopProductAttributes { get; set; } = new List<BrosShopProductAttribute>();
}
