using System;
using System.Collections.Generic;

namespace AlFareejBakeryAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Category { get; set; }

    public string? Ingredients { get; set; }

    public decimal? Price { get; set; }

    public decimal? Cost { get; set; }

    public bool? Seasonal { get; set; }

    public bool? Active { get; set; }

    public DateOnly? IntroducedDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
