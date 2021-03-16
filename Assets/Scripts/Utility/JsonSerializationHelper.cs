using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MageBattle.Utility
{
    public static class JsonSerializationHelper
    {
        private static JsonSerializer _defaultSerializer = JsonSerializer.CreateDefault();
        private static JsonSerializer _defaultSerializerD = JsonSerializer.CreateDefault();
        private static JsonSerializer _defaultSerializerAsync = JsonSerializer.CreateDefault();
        private static StringBuilder _stringBuilder = new StringBuilder();
        private static StringBuilder _stringBuilderAsync = new StringBuilder();

        public static string SerializeObject<T>(T value, Formatting formatting = Formatting.None)
        {
            _stringBuilder.Clear();
            using (StringWriter writer = new StringWriter(_stringBuilder))
            using (JsonWriter jwriter = new JsonTextWriter(writer))
            {
                jwriter.Formatting = formatting;
                _defaultSerializer.Serialize(jwriter, value, typeof(T));
            }
            return _stringBuilder.ToString();
        }

        public static async Task<string> SerializeObjectAsync<T>(T value, Formatting formatting = Formatting.None)
        {
            string result = string.Empty;
            try
            {
                result = await Task.Run<string>(() => {
                    lock (_stringBuilderAsync)
                    {
                        _stringBuilderAsync.Clear();
                        using (StringWriter writer = new StringWriter(_stringBuilderAsync))
                        using (JsonWriter jwriter = new JsonTextWriter(writer))
                        {
                            jwriter.Formatting = formatting;
                            lock (_defaultSerializerAsync)
                            {
                                _defaultSerializerAsync.Serialize(jwriter, value, typeof(T));
                            }
                        }
                        return _stringBuilderAsync.ToString();
                    }
                });
            }
            catch (System.Exception ex)
            {
                result = SerializeObject(value, formatting);
            }

            return result;
        }

        public static T DeserializeObject<T>(string jString)
        {
            T result;
            StringReader reader = new StringReader(jString);
            using (JsonTextReader jReader = new JsonTextReader(reader))
            {
                result = _defaultSerializerD.Deserialize<T>(jReader);
            }
            reader.Close();
            return result;
        }
    }
}