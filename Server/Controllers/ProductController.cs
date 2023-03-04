using System.Security.Cryptography;
using DbConnector;
using Microsoft.AspNetCore.Mvc;
using Yrhacks2023.Shared.Data;

namespace Yrhacks2023.Server.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    [HttpPost]
    [Route("addProduct")]
    public async Task<ActionResult> AddProduct([FromBody] Product product)
    {
        var idArr = new byte[8];
        RandomNumberGenerator.Fill(idArr);
        var id = BitConverter.ToUInt64(idArr, 0);

        IConnector connector = new Connector();
        const string command =
            "INSERT INTO products (Id, Name, TypeId, Description, Image, Seller, Price, Created, Modified)" +
            "VALUES (@Id, @Name, @TypeId, @Description, @Image, @Seller, @Price, @Created, @Modified)";
        await connector.ExecuteAsync(command, new
        {
            Id = id,
            product.Name,
            product.TypeId,
            product.Description,
            product.Image,
            product.Seller,
            product.Price,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        }, Connector.ConnStr);
        return Ok();
    }

    [HttpGet]
    [Route("getAll")]
    public async Task<ActionResult> GetAll()
    {
        IConnector connector = new Connector();
        const string query = "SELECT * FROM products";
        List<Product> products = await connector.QueryAsync<Product, dynamic>(query, new { }, Connector.ConnStr);
        return products.Count == 0 ? NotFound() : Ok(products);
    }

    [HttpGet]
    [Route("getAll/{i:int}")]
    public async Task<ActionResult> GetAllByType(int i)
    {
        Console.WriteLine(i);
        IConnector connector = new Connector();
        const string query = "SELECT * FROM products WHERE TypeId = @TypeId";
        List<Product> products = await connector.QueryAsync<Product, dynamic>(query, new
        {
            TypeId = i
        }, Connector.ConnStr);
        return products.Count == 0 ? NotFound() : Ok(products);
    }

    [HttpPut]
    [Route("search")]
    public async Task<ActionResult> Search([FromBody] string key)
    {
        IConnector connector = new Connector();
        const string query =
            "SELECT * FROM products WHERE Name LIKE @SearchKey";
        List<Product> products = await connector.QueryAsync<Product, dynamic>(query, new
        {
            SearchKey = $"%{key}%"
        }, Connector.ConnStr);
        return Ok(products);
    }
}