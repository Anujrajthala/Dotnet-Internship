namespace TodoApi.Models;

using MongoDB.Bson;
using  MongoDB.Bson.Serialization.Attributes;
    public class TodoItem{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id{set; get;} = null!;

        [BsonElement("title")]
        public string Title{get; set;} = string.Empty;

        [BsonElement("description")]
        public string Description{get; set;} = string.Empty;

        [BsonElement("addedat")]
        public DateTime AddedAt{get; set;}
        public DateTime ExpiryDate{get; set;}

        [BsonElement("iscompleted")]
        public bool IsCompleted{get; set;} = false;
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId{get; set;}= string.Empty;
    }
