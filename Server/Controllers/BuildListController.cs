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
        const string query =
            "SELECT * FROM buildLists WHERE ListId = @ListId LIMIT 1";
        IConnector connector = new Connector();
        List<BuildList> buildList = await connector.QueryAsync<BuildList, dynamic>(query, new
        {
            ListId = listId
        }, Connector.ConnStr);
        return Ok(buildList);
    }

    [HttpPost]
    [Route("newList")]
    public async Task<ActionResult> NewBuildList([FromBody] NewBuildListRequest r)
    {
        if (string.IsNullOrEmpty(r.Token)) return BadRequest();
        (bool exists, string username) = await Utils.GetUnameFromToken(r.Token);
        if (!exists) return Unauthorized();

        const string command =
            "INSERT INTO buildLists (ListId, Name, ProductIds, CreationTime, ModificationTime, OwnerAccount, IsPublic)" +
            "VALUES (@ListId, @Name, @ProductIds, @CreationTime, @ModificationTime, @OwnerAccount, @IsPublic)";
        IConnector connector = new Connector();
        
        var idArr = new byte[8];
        RandomNumberGenerator.Fill(idArr);
        var id = BitConverter.ToUInt64(idArr, 0);

        await connector.ExecuteAsync(command, new
        {
            ListId = id,
            Name = string.IsNullOrEmpty(r.Name) ? $"List {id}" : r.Name,
            ProductIds = string.Empty,
            CreationTime = DateTime.UtcNow,
            ModificationTime = DateTime.UtcNow,
            OwnerAccount = username,
            r.IsPublic
        }, Connector.ConnStr);

        return Ok();
    }

    [HttpPut]
    [Route("append")]
    public async Task<ActionResult> AppendEntry([FromBody] BuildListModificationRequest r)
    {
        // Authentication
        if (string.IsNullOrEmpty(r.Token)) return Unauthorized();
        if (r.ListId is 0 || r.ProductId is 0) return BadRequest();
        (bool exists, string username) = await Utils.GetUnameFromToken(r.Token);
        if (!exists) return Unauthorized();

        IConnector connector = new Connector();
        const string query = "SELECT * FROM buildLists WHERE ListId = @ListId LIMIT 1";
        List<BuildList> result = await connector.QueryAsync<BuildList, dynamic>(query, new
        {
            r.ListId
        }, Connector.ConnStr);
        if (result[0].OwnerAccount != username) return Unauthorized();
        
        // Append
        string oldProductIds = result[0].ProductIds;
        const string command = "UPDATE buildLists SET ProductIds = @ProductIds, ModificationTime = @ModificationTime WHERE ListId = @ListId";
        await connector.ExecuteAsync(command, new
        {
            ProductIds = $"{oldProductIds}\u001f{r.ProductId}",
            ModificationTime = DateTime.UtcNow,
            r.ListId
        }, Connector.ConnStr);

        return Ok();
    }
    
    [HttpPut]
    [Route("remove")]
    public async Task<ActionResult> RemoveEntry([FromBody] BuildListModificationRequest r)
    {
        // Authentication
        if (string.IsNullOrEmpty(r.Token)) return Unauthorized();
        if (r.ListId is 0 || r.ProductId is 0) return BadRequest();
        (bool exists, string username) = await Utils.GetUnameFromToken(r.Token);
        if (!exists) return Unauthorized();

        IConnector connector = new Connector();
        const string query = "SELECT * FROM buildLists WHERE ListId = @ListId LIMIT 1";
        List<BuildList> result = await connector.QueryAsync<BuildList, dynamic>(query, new
        {
            r.ListId
        }, Connector.ConnStr);
        if (result[0].OwnerAccount != username) return Unauthorized();
        
        // Append
        string oldProductIds = result[0].ProductIds;
        const string command = "UPDATE buildLists SET ProductIds = @ProductIds, ModificationTime = @ModificationTime WHERE ListId = @ListId";
        await connector.ExecuteAsync(command, new
        {
            ProductIds = oldProductIds.Replace($"\u001f{r.ProductId}", ""),
            ModificationTime = DateTime.UtcNow,
            r.ListId
        }, Connector.ConnStr);

        return Ok();
    }
    
    [HttpPut]
    [Route("searchPublic")]
    public async Task<ActionResult> SearchLists([FromBody] string name)
    {
        const string query =
            "SELECT * FROM buildLists WHERE Name LIKE @SearchKey AND IsPublic = TRUE";
        IConnector connector = new Connector();
        List<BuildList> lists = await connector.QueryAsync<BuildList, dynamic>(query, new
        {
            SearchKey = $"%{name}%"
        }, Connector.ConnStr);
        return Ok(lists);
    }
    
    [HttpGet]
    [Route("getAll")]
    public async Task<ActionResult> GetAllLists()
    {
        const string query =
            "SELECT * FROM buildLists WHERE IsPublic = TRUE";
        IConnector connector = new Connector();
        List<BuildList> lists = await connector.QueryAsync<BuildList, dynamic>(query, new { }, Connector.ConnStr);
        return Ok(lists);
    }
    
    [HttpPut]
    [Route("getAllPrivate")]
    public async Task<ActionResult> GetAllPrivate([FromBody] string token)
    {
        (bool exists, string username) = await Utils.GetUnameFromToken(token);
        if (!exists) return Unauthorized();
        const string query =
            "SELECT * FROM buildLists WHERE OwnerAccount = @OwnerAccount";
        IConnector connector = new Connector();
        List<BuildList> lists = await connector.QueryAsync<BuildList, dynamic>(query, new
        {
            OwnerAccount = username
        }, Connector.ConnStr);
        return Ok(lists);
    }
    
    [HttpPut]
    [Route("getAllPublicFromUser")]
    public async Task<ActionResult> GetAllPublic([FromBody] string username)
    {
        const string query =
            "SELECT * FROM buildLists WHERE OwnerAccount LIKE @OwnerAccount AND IsPublic = TRUE";
        IConnector connector = new Connector();
        List<BuildList> lists = await connector.QueryAsync<BuildList, dynamic>(query, new
        {
            OwnerAccount = $"%{username}%"
        }, Connector.ConnStr);
        return Ok(lists);
    }
}