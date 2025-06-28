using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace OnlineCourses.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;

        protected BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        protected async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.QuerySingleAsync<T>(sql, parameters);
        }
        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.QueryAsync<T>(sql, parameters);
        }

        protected async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.ExecuteAsync(sql, parameters);
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.ExecuteScalarAsync<T>(sql, parameters);
        }

        protected async Task<T> ExecuteWithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await operation(connection);
        }

        protected async Task ExecuteWithConnectionAsync(Func<IDbConnection, Task> operation)
        {
            using var connection = CreateConnection();
            connection.Open();
            await operation(connection);
        }
    }
}
