﻿using System;
using System.Collections.Generic;

namespace WebApp2.Models;

public partial class BrosShopCategory
{
    public int BrosShopCategoryId { get; set; }

    public string BrosShopCategoryTitle { get; set; } = null!;

    public virtual ICollection<BrosShopProduct> BrosShopProducts { get; set; } = new List<BrosShopProduct>();
}