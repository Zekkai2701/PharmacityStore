using System;
using System.Collections.Generic;

namespace PharmacityStore.Models;

public partial class Product
{
    public string? ProductName { get; set; }

    public double? Price { get; set; }

    public int? CateId { get; set; }

    public string? Color { get; set; }

    public int ProductId { get; set; }

    public string? Brand { get; set; }

    public string? Description { get; set; }

    public string? ProductImage { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Cate { get; set; }
}
