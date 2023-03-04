using System.Security.Cryptography;
using DbConnector;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Yrhacks2023.Shared;

namespace Yrhacks2023.Server;

public static class Utils
{
    public static async Task<(bool, Account?)> Login(LoginRequest r)
    {
        // Check for empty input
        if (r.Username is null or "") return (false, null);
        if (r.Password.Length == 0 || r.Password is null) return (false, null);

        // Get account from database if exists
        IConnector access = new Connector();
        const string sql = "SELECT * FROM accounts WHERE Username = @LoginUsername LIMIT 1";
        List<Account> accounts = await access.QueryAsync<Account, dynamic>(sql, new
        {
            LoginUsername = r.Username
        }, Connector.ConnStr);
        
        // Check if account exists
        Account selected;
        if (accounts.Count == 1) selected = accounts.First();
        else return (false, null);

        byte[] hash = KeyDerivation.Pbkdf2(
            password: Convert.ToBase64String(r.Password),
            salt: selected.PasswordSalt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 128933,
            numBytesRequested: 64);
        
        return hash.SequenceEqual(selected.PasswordHash) ? (true, selected) : (false, null);
    }

    public static async Task<string> NewToken(string username)
    {
        IConnector access = new Connector();
        string token = GenToken();
        const string query = "INSERT INTO tokens (Token, Username) VALUES (@Token, @Username)";
        await access.ExecuteAsync(query, new
        {
            Token = token,
            Username = username
        }, Connector.ConnStr);
        return token;
    }

    public static async Task<(bool, string)> GetUnameFromToken(string token)
    {
        // Get token from database if exists
        IConnector access = new Connector();
        const string sql = "SELECT * FROM tokens WHERE Token = @Token LIMIT 1";
        List<TokenDbObject> results = await access.QueryAsync<TokenDbObject, dynamic>(sql, new
        {
            Token = token
        }, Connector.ConnStr);
        
        // Check if token exists
        return results.Count == 1 ? (true, results.First().Username) : (false, string.Empty);
    }

    private static string GenToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        string result = Convert.ToHexString(bytes);
        return result.ToLowerInvariant();
    }
}
