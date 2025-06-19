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
namespace CNetworking
{
    using System;
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

    public class IntPtrConverter : JsonConverter<IntPtr>
    {
        public override IntPtr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Expecting a number (long) and converting it back to IntPtr
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (IntPtr)reader.GetInt64();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                // Optionally, handle string representation if you might serialize it as a string (e.g., "0x...")
                // For simplicity, we'll stick to number, but you could parse hex here if needed.
                if (long.TryParse(reader.GetString(), out long longValue))
                {
                    return (IntPtr)longValue;
                }
            }
            throw new JsonException($"Cannot convert {reader.TokenType} to IntPtr.");
        }

        public override void Write(Utf8JsonWriter writer, IntPtr value, JsonSerializerOptions options)
        {
            // Write the IntPtr as its underlying long value
            writer.WriteNumberValue(value.ToInt64());
        }
    }


    public class TupleConverter<T1, T2> : JsonConverter<(T1, T2)>
    {
        private readonly JsonConverter<T1>? _item1Converter;
        private readonly JsonConverter<T2>? _item2Converter;

        public TupleConverter(JsonConverter<T1>? item1Converter, JsonConverter<T2>? item2Converter)
        {
            _item1Converter = item1Converter;
            _item2Converter = item2Converter;
        }

        public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of array for tuple.");

            reader.Read();

            if (reader.TokenType == JsonTokenType.EndArray)
                throw new JsonException("Tuple array cannot be empty.");

            T1 item1;
            if (_item1Converter != null)
                item1 = _item1Converter.Read(ref reader, typeof(T1), options);
            else
                item1 = JsonSerializer.Deserialize<T1>(ref reader, options);

            reader.Read();

            if (reader.TokenType == JsonTokenType.EndArray)
                throw new JsonException("Tuple array missing second element.");

            T2 item2;
            if (_item2Converter != null)
                item2 = _item2Converter.Read(ref reader, typeof(T2), options);
            else
                item2 = JsonSerializer.Deserialize<T2>(ref reader, options);

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException("Expected end of array for tuple.");

            return (item1, item2);
        }

        public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (_item1Converter != null)
                _item1Converter.Write(writer, value.Item1, options);
            else
                JsonSerializer.Serialize(writer, value.Item1, options);

            if (_item2Converter != null)
                _item2Converter.Write(writer, value.Item2, options);
            else
                JsonSerializer.Serialize(writer, value.Item2, options);

            writer.WriteEndArray();
        }
    }



    public class TupleConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type[] genericArguments = typeToConvert.GetGenericArguments();
            Type type1 = genericArguments[0];
            Type type2 = genericArguments[1];

            JsonConverter converter1 = options.GetConverter(type1);
            JsonConverter converter2 = options.GetConverter(type2);

            Type converterType = typeof(TupleConverter<,>).MakeGenericType(new Type[] { type1, type2 });

            return (JsonConverter)Activator.CreateInstance(converterType, new object[] { converter1, converter2 });
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
            Converters = { new IntPtrConverter() , new Int2DArrayConverter(), new TupleConverterFactory(),}
        };

        public static byte[] Serialize<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj, options);
            return Encoding.UTF8.GetBytes(json);
        }

        public static async Task SendObjectAsync<T>(T obj, NetworkStream stream)
        {
            string kys = JsonSerializer.Serialize(obj, options);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(kys);
            int length = payload.Length;
            byte[] lengthPrefix = BitConverter.GetBytes(length);

            if (stream != null)
            { 
                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

                await stream.WriteAsync(payload, 0, payload.Length);
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
