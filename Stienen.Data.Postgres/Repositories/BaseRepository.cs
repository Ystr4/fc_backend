using System.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Data.Postgres.Repositories
{
    public class BaseRepository {
        private string _connectionString;
        public BaseRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }
        private NpgsqlConnection _connection = null;

        protected NpgsqlConnection Connection => _connection ?? (_connection = new NpgsqlConnection(_connectionString));
    }
}