using McKIESales.WEB.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace McKIESales.WEB.Models {
    /// <summary>
    /// This class serves as a wrapper for the MongoDB connection. 
    /// It initializes a connection to the MongoDB server using settings from 
    /// the configuration, such as the connection string and database name, which 
    /// are provided via `IOptions<MongoDbSettings>`. It creates a MongoDB client, 
    /// connects to the specified database, and provides access to the `Users` 
    /// collection by exposing it as a property. This class essentially abstracts 
    /// the interaction with the MongoDB database, particularly focusing on the 
    /// `Users` collection in this case.
    /// </summary>
    public class MongoDbContext {
        private readonly IMongoDatabase _database;

        public MongoDbContext (IOptions<MongoDbSettings> settings){
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        //  That line is basically a shortcut to interact with the "Users"
        //  collection in your MongoDB database. It pulls the collection for `User`
        //  documents, so you can easily use it throughout your `MongoDbContext`
        //  class for tasks like querying, adding, or modifying user data.
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}