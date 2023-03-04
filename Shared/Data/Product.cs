using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Yrhacks2023.Shared.Data;

public class Product
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Seller { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    [NotMapped]
    [JsonIgnore]
    public ItemType Type
    {
        get => (ItemType) TypeId;
        set => TypeId = (int) value;
    }
}