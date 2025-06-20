using CNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using Catan;

namespace CNetworking
{
    using System;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class Int2DArrayConverter : JsonConverter<int[,]>
    {
        public override int[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jagged = JsonSerializer.Deserialize<int[][]>(ref reader, options);
            if (jagged == null || jagged.Length == 0) return new int[0, 0];

            int rows = jagged.Length;
            int cols = jagged[0].Length;
            var result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                if (jagged[i].Length != cols)
                    throw new JsonException("Inconsistent column size in jagged array");

                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = jagged[i][j];
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, int[,] value, JsonSerializerOptions options)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            writer.WriteStartArray();
            for (int i = 0; i < rows; i++)
            {
                writer.WriteStartArray();
                for (int j = 0; j < cols; j++)
                {
                    writer.WriteNumberValue(value[i, j]);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }

    public class IntPtrConverter : JsonConverter<System.IntPtr>
    {
        public override System.IntPtr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Expecting a number (long) and converting it back to IntPtr
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (System.IntPtr)reader.GetInt64();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                // Optionally, handle string representation if you might serialize it as a string (e.g., "0x...")
                // For simplicity, we'll stick to number, but you could parse hex here if needed.
                if (long.TryParse(reader.GetString(), out long longValue))
                {
                    return (System.IntPtr)longValue;
                }
            }
            throw new JsonException($"Cannot convert {reader.TokenType} to IntPtr.");
        }

        public override void Write(Utf8JsonWriter writer, System.IntPtr value, JsonSerializerOptions options)
        {
            // Write the IntPtr as its underlying long value
            writer.WriteNumberValue(value.ToInt64());
        }
    }

    public class MoveConverter : JsonConverter<Move>
    {
        public override Move Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("MoveType", out var typeProperty))
                throw new JsonException("Missing MoveType discriminator");

            string moveType = typeProperty.GetString();

            Type actualType = moveType switch
            {
                nameof(HouseMove) => typeof(HouseMove),
                nameof(RoadMove) => typeof(RoadMove),
                nameof(FirstMove) => typeof(FirstMove),
                nameof(TradeMove) => typeof(TradeMove),
                nameof(RobberMove) => typeof(RobberMove),
                nameof(UpgradeMove) => typeof(UpgradeMove),
                nameof(EndMove) => typeof(EndMove),
                nameof(DiscardMove) => typeof(DiscardMove),
                _ => throw new JsonException($"Unknown MoveType: {moveType}")
            };

            var json = root.GetRawText();
            return (Move)JsonSerializer.Deserialize(json, actualType, options);
        }

        public override void Write(Utf8JsonWriter writer, Move value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }


    public class TupleConverter<T1, T2> : JsonConverter<(T1, T2)>
    {
        public TupleConverter() { }
        public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();
            reader.Read();
            T1 item1 = JsonSerializer.Deserialize<T1>(ref reader, options)!;
            reader.Read();
            T2 item2 = JsonSerializer.Deserialize<T2>(ref reader, options)!;
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray) throw new JsonException();
            return (item1, item2);
        }

        public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            JsonSerializer.Serialize(writer, value.Item1, value.Item1?.GetType() ?? typeof(T1), options);
            JsonSerializer.Serialize(writer, value.Item2, value.Item2?.GetType() ?? typeof(T2), options);
            writer.WriteEndArray();
        }
    }




    public class TupleConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType
                && typeToConvert.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type[] args = typeToConvert.GetGenericArguments();
            Type converterType = typeof(TupleConverter<,>).MakeGenericType(args);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }


    public class CMessageConverter : JsonConverter<CMessage>
    {
        public override void Write(Utf8JsonWriter writer, CMessage msg, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(msg.Type), msg.Type);
            writer.WriteString(nameof(msg.PayloadType), msg.PayloadType);
            writer.WritePropertyName(nameof(msg.Payload));

            if (msg.Payload == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                JsonSerializer.Serialize(
                    writer,
                    msg.Payload,
                    msg.Payload.GetType(),
                    options
                );
            }

            writer.WriteEndObject();
        }

        public override CMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var type = root.GetProperty(nameof(CMessage.Type)).GetString()!;
            var payloadTypeName = root.GetProperty(nameof(CMessage.PayloadType)).GetString();
            var payloadElement = root.GetProperty(nameof(CMessage.Payload));

            object? payload = null;
            if (payloadElement.ValueKind != JsonValueKind.Null && !string.IsNullOrEmpty(payloadTypeName))
            {
                var actualType = Type.GetType(payloadTypeName);
                if (actualType != null)
                    payload = JsonSerializer.Deserialize(
                        payloadElement.GetRawText(),
                        actualType,
                        options
                    );
            }

            return new CMessage(type, payload);
        }
    }


    public interface IMsgTransceiver
    { 
        Task<CMessage> Proccess(CMessage msg);

    }
    public class CMessage
    {
        public string Type;
        public object? Payload;
        public string PayloadType;
        public CMessage(string type, object? payload)
        {
            Type = type;
            Payload = payload;
            if (Payload != null) PayloadType = payload.GetType().AssemblyQualifiedName;
            else PayloadType = "null";
        }
    }
    public class NetworkUtils
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            IncludeFields = true,
            Converters =
            {
                new IntPtrConverter(),
                new Int2DArrayConverter(),
                new CMessageConverter(),
                new TupleConverterFactory(),
                new MoveConverter()
            }
        };

        public static byte[] Serialize<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj, obj.GetType(), options);
            return Encoding.UTF8.GetBytes(json);
        }

        public static async Task SendObjectAsync<T>(T obj, NetworkStream stream)
        {
            /*string kys = JsonSerializer.Serialize(obj, obj.GetType(), options);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(kys);
            int length = payload.Length;
            byte[] lengthPrefix = BitConverter.GetBytes(length);

            if (stream != null)
            { 
                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

                await stream.WriteAsync(payload, 0, payload.Length);
            }*/
                    string json = JsonSerializer.Serialize(obj, obj.GetType(), options);
            byte[] payload = Encoding.UTF8.GetBytes(json);

            var lengthPrefix = BitConverter.GetBytes(payload.Length);
            await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
            await stream.WriteAsync(payload, 0, payload.Length);
        }


        public static class PlayerTypeRegistry
        {
            private static readonly Dictionary<string, Type> _typeMap = new();

            public static void RegisterPlayerType<T>() where T : IPlayer
            {
                _typeMap[typeof(T).Name] = typeof(T);
            }

            public static Type? Resolve(string typeName)
            {
                return _typeMap.TryGetValue(typeName, out var type) ? type : null;
            }

            public static string GetTypeName(IPlayer player) => player.GetType().Name;
        }

        public class IPlayerConverter : JsonConverter<IPlayer>
        {
            public override IPlayer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using JsonDocument doc = JsonDocument.ParseValue(ref reader);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("$type", out JsonElement typeProp))
                    throw new JsonException("Missing $type discriminator");

                string typeName = typeProp.GetString();
                Type? targetType = PlayerTypeRegistry.Resolve(typeName);

                if (targetType == null)
                    throw new JsonException($"Unrecognized player type: {typeName}");

                return (IPlayer?)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
            }

            public override void Write(Utf8JsonWriter writer, IPlayer value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("$type", PlayerTypeRegistry.GetTypeName(value));

                var json = JsonSerializer.SerializeToElement(value, value.GetType(), options);
                foreach (var prop in json.EnumerateObject())
                {
                    prop.WriteTo(writer);
                }

                writer.WriteEndObject();
            }
        }
        public static async Task<T> ReceiveObjectAsync<T>(NetworkStream stream)
        {
            byte[] lengthPrefix = new byte[4];
            int read = await stream.ReadAsync(lengthPrefix, 0, 4);
            if (read < 4) throw new Exception("Failed to read message length");

            int length = BitConverter.ToInt32(lengthPrefix, 0);
            byte[] buffer = new byte[length];

            int bytesRead = 0;
            while (bytesRead < length)
            {
                int chunkSize = await stream.ReadAsync(buffer, bytesRead, length - bytesRead);
                if (chunkSize == 0) throw new Exception("Disconnected while reading message");
                bytesRead += chunkSize;
            }

            string jsonString = Encoding.UTF8.GetString(buffer);
            T result = JsonSerializer.Deserialize<T>(jsonString, options);
            
            if (result == null) throw new Exception("Failed to deserialize the object.");

            if (result is CMessage message)
            {
                if (message.Payload is JsonElement payloadJson && !string.IsNullOrEmpty(message.PayloadType))
                {
                    if(message.PayloadType == "null")
                    {
                        message.Payload = null;

                    }
                    else 
                    {
                        Type actualPayloadType = Type.GetType(message.PayloadType);
                        if (actualPayloadType != null)
                        {
                            try
                            {
                                message.Payload = JsonSerializer.Deserialize(payloadJson.GetRawText(), actualPayloadType, options);
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine($"Deserialization failed for {actualPayloadType}: {ex.Message}");
                                //Debug.WriteLine($"Deserialization failed for {actualPayloadType}: {ex.Message}");
                                throw;
                            }

                        }
                        else
                        {
                            // Fallback if the type can't be found (e.g., assembly not loaded)
                            // It will remain a JsonElement or be deserialized to object
                            //Console.WriteLine($"Warning: Could not find type {message.PayloadType}. Payload will remain as JsonElement or 'object'.");
                            message.Payload = JsonSerializer.Deserialize<object>(payloadJson.GetRawText(), options);
                        }
                    }
                   
                }
            }

            return result;
        }


    }
}
