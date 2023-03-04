using Yrhacks2023.Shared.Data;

namespace Yrhacks2023.Shared.Requests;

public class NewProductRequest
{
    public Product Prod { get; set; }
    public string Token { get; set; }
}