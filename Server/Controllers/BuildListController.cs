using System.Security.Cryptography;
using DbConnector;
using Microsoft.AspNetCore.Mvc;
using Yrhacks2023.Shared.Data;
using Yrhacks2023.Shared.Requests;

namespace Yrhacks2023.Server.Controllers;

[ApiController]
[Route("api/buildList")]
public class BuildListController : ControllerBase
{
    [HttpGet]
    [Route("getById")]
    public async Task<ActionResult> GetBuildList([FromBody] ulong listId)
    {
        const string buildListQuery =
            "SELECT * FROM buildLists WHERE ListId = @ListId";
        IConnector connector = new Connector();
        List<BuildListEntry> result = await connector.QueryAsync<BuildListEntry, dynamic>(buildListQuery, new
        {
            ListId = listId
        }, Connector.ConnStr);

        if (result.Count == 0) return NotFound();

        var reply = new BuildList
        {
            ListId = listId,
            ProductIds = new List<ulong>()
        };
        foreach (BuildListEntry entry in result)
        {
            if (entry.ProductId == 0)
            {
                reply.Name = entry.Name;
                reply.CreationTime = entry.CreationTime;
                reply.ModificationTime = entry.ModificationTime;
                reply.OwnerAccount = entry.OwnerAccount;
                reply.IsPublic = entry.IsPublic;
            }
            else
            {
                reply.ProductIds.Add(entry.ProductId);
            }
        }
        return Ok(reply);
    }

    [HttpPost]
    [Route("newList")]
    public async Task<ActionResult> NewBuildList([FromBody] NewBuildListRequest r)
    {
        if (string.IsNullOrEmpty(r.Token)) return BadRequest();
        (bool exists, string username) = await Utils.GetUnameFromToken(r.Token);
        if (!exists) return Unauthorized();

        const string command =
            "INSERT INTO buildLists (ListId, Name, ProductId, CreationTime, ModificationTime, OwnerAccount, IsPublic)" +
            "VALUES (@ListId, @Name, @ProductId, @CreationTime, @ModificationTime, @OwnerAccount, @IsPublic)";
        IConnector connector = new Connector();
        
        var idArr = new byte[8];
        RandomNumberGenerator.Fill(idArr);
        var id = BitConverter.ToUInt64(idArr, 0);

        await connector.ExecuteAsync(command, new
        {
            ListId = id,
            Name = string.IsNullOrEmpty(r.Name) ? $"List {id}" : r.Name,
            ProductId = 0,
            CreationTime = DateTime.UtcNow,
            ModificationTime = DateTime.UtcNow,
            OwnerAccount = username,
            r.IsPublic
        }, Connector.ConnStr);

        return Ok();
    }

    [HttpPost]
    [Route("append")]
    public async Task<ActionResult> AppendEntry([FromBody] BuildListModificationRequest r)
    {
        // Authentication
        if (string.IsNullOrEmpty(r.Token) || r.ListId is 0 || r.ProductId is 0) return BadRequest();
        (bool exists, string username) = await Utils.GetUnameFromToken(r.Token);
        if (!exists) return Unauthorized();

        IConnector connector = new Connector();
        const string query = "SELECT * FROM buildLists WHERE ListId = @ListId AND ProductId = 0 LIMIT 1";
        List<BuildListEntry> result = await connector.QueryAsync<BuildListEntry, dynamic>(query, new
        {
            ListId = r.ListId
        }, Connector.ConnStr);
        if (result[0].OwnerAccount != username) return Unauthorized();
        string name = result[0].Name;
        DateTime creationTime = result[0].CreationTime;
        string ownerAccount = result[0].OwnerAccount;
        bool isPublic = result[0].IsPublic;

        // Append supplied value
        string command =
            "INSERT INTO buildLists (ListId, Name, ProductId, CreationTime, ModificationTime, OwnerAccount, IsPublic)" +
            "VALUES (@ListId, @Name, @ProductId, @CreationTime, @ModificationTime, @OwnerAccount, @IsPublic)";
        await connector.ExecuteAsync(command, new
        {
            ListId = r.ListId,
            ProductId = r.ProductId
        }, Connector.ConnStr);
        
        // Delete header entry
        command = "DELETE FROM buildLists WHERE ListId = @ListId AND ProductId = 0";
        await connector.ExecuteAsync(command, new
        {
            r.ListId
        }, Connector.ConnStr);
        
        // Add new header entry
        command =
            "INSERT INTO buildLists (ListId, Name, ProductId, CreationTime, ModificationTime, OwnerAccount, IsPublic)" +
            "VALUES (@ListId, @Name, @ProductId, @CreationTime, @ModificationTime, @OwnerAccount, @IsPublic)";
        await connector.ExecuteAsync(command, new
        {
            r.ListId,
            Name = name,
            ProductId = 0,
            CreationTime = creationTime,
            ModificationTime = DateTime.UtcNow,
            OwnerAccount = ownerAccount,
            IsPublic = isPublic
        }, Connector.ConnStr);

        return Ok();
    }
    
    [HttpPost]
    [Route("remove")]
    public async Task<ActionResult> RemoveEntry([FromBody] BuildListModificationRequest r)
    {
        // Authentication
        if (string.IsNullOrEmpty(r.Token) || r.ListId is 0 || r.ProductId is 0) return BadRequest();
        (bool exists, string username) = await Utils.GetUnameFromToken(r.Token);
        if (!exists) return Unauthorized();

        IConnector connector = new Connector();
        const string query = "SELECT * FROM buildLists WHERE ListId = @ListId AND ProductId = 0 LIMIT 1";
        List<BuildListEntry> result = await connector.QueryAsync<BuildListEntry, dynamic>(query, new
        {
            ListId = r.ListId
        }, Connector.ConnStr);
        if (result[0].OwnerAccount != username) return Unauthorized();
        string name = result[0].Name;
        DateTime creationTime = result[0].CreationTime;
        string ownerAccount = result[0].OwnerAccount;
        bool isPublic = result[0].IsPublic;
        
        // Remove supplied value
        var command =
            "DELETE FROM buildLists WHERE ListId = @ListId AND ProductId = @ProductId LIMIT 1";
        await connector.ExecuteAsync(command, new
        {
            ListId = r.ListId,
            ProductId = r.ProductId
        }, Connector.ConnStr);
        
        // Delete header entry
        command = "DELETE FROM buildLists WHERE ListId = @ListId AND ProductId = 0";
        await connector.ExecuteAsync(command, new
        {
            r.ListId
        }, Connector.ConnStr);
        
        // Add new header entry
        command =
            "INSERT INTO buildLists (ListId, Name, ProductId, CreationTime, ModificationTime, OwnerAccount, IsPublic)" +
            "VALUES (@ListId, @Name, @ProductId, @CreationTime, @ModificationTime, @OwnerAccount, @IsPublic)";
        await connector.ExecuteAsync(command, new
        {
            r.ListId,
            Name = name,
            ProductId = 0,
            CreationTime = creationTime,
            ModificationTime = DateTime.UtcNow,
            OwnerAccount = ownerAccount,
            IsPublic = isPublic
        }, Connector.ConnStr);

        return Ok();
    }
    
    [HttpPut]
    [Route("search")]
    public async Task<ActionResult> SearchLists([FromBody] ulong productId)
    {
        // TODO: not implemented
        return StatusCode(501);
    }
}