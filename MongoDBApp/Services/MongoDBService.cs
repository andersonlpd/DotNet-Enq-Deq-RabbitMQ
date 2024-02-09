using MongoDB.Driver;

using MongoDBApp.Entity;

namespace MongoDBApp.Services
{
    public class MongoDBService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<PersistRequest> _collection;

        public MongoDBService(IConfiguration config)
        {
            _config = config;

            var connectionString = _config["MongoDB:ConnectionString"];
            var databaseName = _config["MongoDB:DatabaseName"];
            var collectionName = _config["MongoDB:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<PersistRequest>(collectionName);
        }

        public void PersistMessage(PersistRequest request)
        {
                _collection.InsertOne(request);
        }
    }
}