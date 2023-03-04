using System.Security.Cryptography;
using DbConnector;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Yrhacks2023.Shared;

namespace Yrhacks2023.Server.Controllers;

[ApiController]
[Route("AccountApi")]
public class AccountController : ControllerBase
{
    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register([FromBody] RegistrationRequest r)
    {
        if (r.Username is null or "") return BadRequest();
        if (r.Email is null or "") return BadRequest();
        if (r.PasswordHash.Length == 0 || r.PasswordHash is null) return BadRequest();
        if (r.DisplayName is null or "") r.DisplayName = r.Username;

        byte[] salt = RandomNumberGenerator.GetBytes(128);
        byte[] hash = KeyDerivation.Pbkdf2(
            password: Convert.ToBase64String(r.PasswordHash),
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 128933,
            numBytesRequested: 64);

        const string command =
            "INSERT INTO accounts (Id, Username, PasswordHash, PasswordSalt, DisplayName, Email, Joined)" +
            "VALUES (@Id, @Username, @PasswordHash, @PasswordSalt, @DisplayName, @Email, @Joined)";
        
        var idArr = new byte[8];
        RandomNumberGenerator.Fill(idArr);
        var id = BitConverter.ToUInt64(idArr, 0);

        IConnector connector = new Connector();
        await connector.ExecuteAsync(command, new
        {
            Id = id,
            r.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            r.DisplayName,
            r.Email,
            Joined = DateTime.UtcNow
        }, Connector.ConnStr);

        string token = await Utils.NewToken(r.Username);
        return Ok(token);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        (bool result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        string token = await Utils.NewToken(selected!.Username);
        return Ok(token);
    }
}