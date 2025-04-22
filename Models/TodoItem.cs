namespace TodoApi.Models;

using MongoDB.Bson;
using  MongoDB.Bson.Serialization.Attributes;
    public class TodoItem{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id{set; get;}

        [BsonElement("title")]
        public string Title{get; set;} = string.Empty;

        [BsonElement("description")]
        public string Description{get; set;} = string.Empty;

        [BsonElement("iscompleted")]
        public string IsCompleted{get; set;} = string.Empty;
    }
