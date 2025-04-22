namespace TodoApi.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApi.Models;
public class ApplicationDbContext{
    private readonly IMongoDatabase _database;

    public ApplicationDbContext(IOptionsMonitor<MongoDbSettings> optionsMonitor ){
        var settings = optionsMonitor.CurrentValue;
        var client  = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<TodoItem> TodoItems => _database.GetCollection<TodoItem>("TodoItems");
    public IMongoCollection<User> Users => _database.GetCollection<User>("User");
}