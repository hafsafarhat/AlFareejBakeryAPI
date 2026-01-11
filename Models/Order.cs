using System;
using System.Collections.Generic;

namespace AlFareejBakeryAPI.Models;

public partial class Order
{
    public int TransactionId { get; set; }

    public int? CustomerId { get; set; }

    public int? ProductId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public TimeOnly? OrderTime { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Channel { get; set; }

    public int? StoreId { get; set; }

    public int? PromotionId { get; set; }

    public string? Status { get; set; }

    public decimal? DiscountAmount { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
