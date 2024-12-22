using System;
using System.Collections.Generic;

namespace WebApp2.Models;

public partial class BrosShopProductAttribute
{
    public int BrosShopAttributesId { get; set; }

    public int BrosShopProductId { get; set; }

    public int BrosShopCount { get; set; }

    public int? BrosShopColorId { get; set; }

    public int? BrosShopSize { get; set; }

    public virtual BrosShopColor? BrosShopColor { get; set; }

    public virtual BrosShopProduct BrosShopProduct { get; set; } = null!;

    public virtual BrosShopSize? BrosShopSizeNavigation { get; set; }
}
