using Newtonsoft.Json.Linq;

namespace TrelloSpc.UnitTest.Helpers
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            return JObject.FromObject(obj).ToString();
        }
    }
}