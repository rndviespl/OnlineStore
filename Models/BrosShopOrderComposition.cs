using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp2.Models;

public partial class BrosShopOrderComposition
{
    [Key]
    public int BrosShopOrderId { get; set; }
    [Key]
    public int BrosShopAttributesId { get; set; }

    public sbyte BrosShopQuantity { get; set; }

    public decimal BrosShopCost { get; set; }

    public virtual BrosShopProductAttribute BrosShopAttributes { get; set; } = null!;

    public virtual BrosShopOrder BrosShopOrder { get; set; } = null!;
}
