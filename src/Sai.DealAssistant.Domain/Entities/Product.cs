using System;

namespace Sai.DealAssistant.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }

    private Product() { /* EF */ }

    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetPrice(price);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        Name = name;
    }

    public void SetPrice(decimal price)
    {
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
        Price = price;
    }
}