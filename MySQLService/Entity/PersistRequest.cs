
namespace MySQLApp.Entity
{

    public class PersistRequest
    {
        public required long Timestamp { get; set; }
        public required int ID { get; set; }
        public required string Message { get; set; }
    }

}
