using System.Text.Json;
using System.Text.Json.Serialization;
using CQRSCore.Events;
using PostCommon.Events;

namespace PostQuery.Infrastructure.Converters;

public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {

        return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
    }
    public override BaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var document))
        {
            throw new JsonException($"Ошибка парсинга {nameof(JsonDocument)}");
        }
        if (!document.RootElement.TryGetProperty("Type", out var typeProperty))
        {
            throw new JsonException($"В Json-документе нет 'Type' свойства");
        }
        var _type = typeProperty.GetString();
        var json = document.RootElement.GetRawText();
        return _type switch
        {
            nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
            nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json, options),
            nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
            nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
            nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
            nameof(CommentDeletedEvent) => JsonSerializer.Deserialize<CommentDeletedEvent>(json, options),
            nameof(PostDeletedEvent) => JsonSerializer.Deserialize<PostDeletedEvent>(json, options),
            _ => throw new JsonException($"{_type} не поддерживается")
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}