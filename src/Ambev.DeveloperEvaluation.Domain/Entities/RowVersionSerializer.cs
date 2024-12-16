using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class RowVersionSerializer : SerializerBase<byte[]>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, byte[] value)
        {
            if (value == null)
                context.Writer.WriteNull();
            else
                context.Writer.WriteString(Convert.ToBase64String(value));
        }

        public override byte[] Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            switch (bsonReader.CurrentBsonType)
            {
                case BsonType.String:
                    return Convert.FromBase64String(bsonReader.ReadString());
                case BsonType.Binary:
                    return bsonReader.ReadBytes();
                default:
                    throw new BsonSerializationException("Invalid RowVersion type");
            }
        }
    }
}
