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
            "SELECT * FROM buildLists WHERE ListId=@ListId";
        IConnector connector = new Connector();
        List<BuildListEntry> result = await connector.QueryAsync<BuildListEntry, dynamic>(buildListQuery, new
        {
            ListId = listId
        }, Connector.ConnStr);

        if (result.Count == 0)
            return NotFound();

        BuildListReply reply = new();
        reply.ListId = listId;
        reply.ProductIds = new();
        foreach (var entry in result)
        {
            if (entry.ProductId == null)
            {
                reply.CreationTime = entry.CreationTime;
                reply.ModificationTime = entry.ModificationTime;
            }
            else
            {
                reply.ProductIds.Add(entry.ProductId.Value);
            }
        }
        return Ok(reply);
    }
    
    [HttpPost]
    [Route("append")]
    public async Task<ActionResult> AppendEntry([FromBody] AppendBuildListRequest request)
    {
        // TODO: not implemented
        return StatusCode(501);
    }
    
    [HttpPost]
    [Route("remove")]
    public async Task<ActionResult> RemoveEntry([FromBody] RemoveBuildListRequest request)
    {
        // TODO: not implemented
        return StatusCode(501);
    }
    
    [HttpPut]
    [Route("search")]
    public async Task<ActionResult> SearchLists([FromBody] ulong productId)
    {
        // TODO: not implemented
        return StatusCode(501);
    }
}