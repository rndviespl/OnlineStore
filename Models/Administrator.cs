﻿using System;
using System.Collections.Generic;

namespace WebApp2.Models;

public partial class Administrator
{
    public int BrosShopAdministratorId { get; set; }

    public string BrosShopLogin { get; set; } = null!;

    public string BrosShopPassword { get; set; } = null!;

    public ulong? BrosShopIsAdmin { get; set; }
}