using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace DbConnector;

public class Connector : IConnector
{
    public const string ConnStr = "Server=172.105.103.136;Port=3306;Database=Pcpp;Uid=PcppUser;Pwd=PcppPassword";
    
    public async Task<List<T>> QueryAsync<T, TU>(string sql, TU parameters, string connectionString)
    {
        using IDbConnection connection = new MySqlConnection(connectionString);
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        IEnumerable<T> rows = await connection.QueryAsync<T>(sql, parameters);
        return rows.ToList();
    }
    
    public Task ExecuteAsync<T>(string sql, T parameters, string connectionString)
    {
        using IDbConnection connection = new MySqlConnection(connectionString);
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        return connection.ExecuteAsync(sql, parameters);
    }
}