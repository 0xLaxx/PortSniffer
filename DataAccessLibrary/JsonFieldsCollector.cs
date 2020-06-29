using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DatabaseAccessLibrary
{
    //SOURCE: https://riptutorial.com/de/csharp/example/32164/sammeln-sie-alle-felder-des-json-objekts
    public class JsonFieldsCollector
    {
        private readonly Dictionary<string, JValue> fields;
        public IEnumerable<KeyValuePair<string, JValue>> GetAllFields() => fields;

        public JsonFieldsCollector(JToken token)
        {
            fields = new Dictionary<string, JValue>();
            CollectFields(token);
        }

        private void CollectFields(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (var child in jToken.Children<JProperty>())
                        CollectFields(child);
                    break;
                case JTokenType.Array:
                    foreach (var child in jToken.Children())
                        CollectFields(child);
                    break;
                case JTokenType.Property:
                    CollectFields(((JProperty)jToken).Value);
                    break;
                default:
                    fields.Add(jToken.Path, (JValue)jToken);
                    break;
            }
        }

    }
}
