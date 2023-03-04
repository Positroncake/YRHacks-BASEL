namespace Database;

public interface IConnector
{
    Task<List<T>> QueryAsync<T, TU>(string sql, TU parameters, string connectionString);
    Task ExecuteAsync<T>(string sql, T parameters, string connectionString);
}