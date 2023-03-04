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
            "INSERT INTO products (Id, Name, TypeId, Description, Seller, Price, Created, Modified)" +
            "VALUES (@Id, @Name, @TypeId, @Description, @Seller, @Price, @Created, @Modified)";
        await connector.ExecuteAsync(command, new
        {
            Id = id,
            product.Name,
            product.TypeId,
            product.Description,
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
        const string query = "SELECT (Id, Name, TypeId, Description, Seller, Price) FROM products";
        List<Product> products = await connector.QueryAsync<Product, dynamic>(query, new { }, Connector.ConnStr);
        return products.Count == 0 ? NotFound() : Ok(products);
    }

    [HttpGet]
    [Route("getAll/{i:int}")]
    public async Task<ActionResult> GetAllByType(int i)
    {
        Console.WriteLine(i);
        IConnector connector = new Connector();
        const string query = "SELECT (Id, Name, TypeId, Description, Seller, Price) FROM products WHERE TypeId = @TypeId";
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
            "SELECT (Id, Name, TypeId, Description, Seller, Price) FROM products WHERE Name LIKE @SearchKey";
        List<Product> products = await connector.QueryAsync<Product, dynamic>(query, new
        {
            SearchKey = $"%{key}%"
        }, Connector.ConnStr);
        return Ok(products);
    }

    [HttpGet]
    [Route("image")]
    public async Task<IActionResult> GetImage(ulong id)
    {
        IConnector connector = new Connector();
        const string query = "SELECT Picture FROM products WHERE Id=@Id";
        using var reader = connector.GetBlob(query, Connector.ConnStr);

        if (!await reader.ReadAsync())
            return NotFound();

        int len = (int) reader.GetBytes(0, 0, null, 0, 0);
        byte[] data = new byte[len];
        reader.GetBytes(0, 0, data, 0, len);

        return File(data, "image/png");
    }

    [HttpPost]
    [Route("imageUpload")]
    public async Task<IActionResult> UploadImage(ulong id, [FromBody] IFormFile file)
    {
        // 8 MB max upload
        const int maxLength = (1 << 20) * 8;
        if (file.ContentType != "image/png")
            return StatusCode(415);
        if (file.Length > maxLength)
            return BadRequest();

        using (MemoryStream tempBuf = new MemoryStream())
        {
            await file.CopyToAsync(tempBuf);
            IConnector connector = new Connector();
            const string query = "UPDATE products SET Image=@Data WHERE Id=@Id";
            await connector.ExecuteAsync(query, new
            {
                Data = tempBuf.GetBuffer(),
                Id = id
            }, Connector.ConnStr);
        }

        return Ok();
    }
}