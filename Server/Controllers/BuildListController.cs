using DbConnector;
using Microsoft.AspNetCore.Mvc;

namespace Yrhacks2023.Server.Controllers;

[ApiController]
[Route("api/buildList")]
public class BuildListController : ControllerBase
{
    [HttpGet]
    [Route("getById")]
    public async Task<ActionResult> GetBuildList()
    {
        IConnector connector = new Connector();
        return Ok("TODO...");
    }
}