using MySql.Data.MySqlClient;

using MySQLApp.Entity;

namespace MySQLApp.Services
{
    public class MySQLService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public MySQLService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config["MySQL:ConnectionString"];
        }

        public void PersistMessage(PersistRequest request)
        {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Messages (Timestamp, ID, Message) VALUES (@Timestamp, @ID, @Message)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Timestamp", request.Timestamp);
                        command.Parameters.AddWithValue("@ID", request.ID);
                        command.Parameters.AddWithValue("@Message", request.Message);
                        command.ExecuteNonQuery();
                    }
                }
        }
    }
}