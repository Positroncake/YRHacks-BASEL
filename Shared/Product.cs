namespace Yrhacks2023.Shared;

public class Product
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Seller { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    // [NotMapped]
    // [JsonIgnore]
    // public ItemType Type
    // {
    //     get => (ItemType) TypeId;
    //     set => TypeId = (int) value;
    // }
}