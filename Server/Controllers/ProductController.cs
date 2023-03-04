using DbConnector;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Yrhacks2023.Shared;

namespace Yrhacks2023.Server.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    [HttpPost]
    [Route("listProducts")]
    public async Task<ActionResult> ListProduct([FromBody] Product product)
    {
        IConnector connector = new Connector();
        const string command =
            "INSERT INTO products (Id, Name, TypeId, Description, Image, Seller, Price) VALUES (@Id, @Name, @TypeId, @Description, @Image, @Seller, @Price)";
        await connector.ExecuteAsync(command, new
        {
            product.Id,
            product.Name,
            product.TypeId,
            product.Description,
            product.Image,
            product.Seller,
            product.Price
        }, Connector.ConnStr);
        return Ok();
    }
}