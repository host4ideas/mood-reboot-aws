using NugetMoodReboot.Helpers;

namespace MoodReboot.Extensions
{
    public static class SessionExtension
    {
        // Queremos un metodo GetObject<T>(KEY)
        public static T? GetObject<T>(this ISession session, string key)
        {
            string json = session.GetString(key)!;
            if (json == null)
            {
                return default;
            }
            return HelperJsonSession.DeserializeObject<T>(json);
        }
        // Queremos un metodo SetObject(KEY, OBJETO)
        public static void SetObject(this ISession session, string key, object value)
        {
            string data = HelperJsonSession.SerializeObject(value);
            session.SetString(key, data);
        }
    }
}
