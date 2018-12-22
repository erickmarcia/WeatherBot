namespace WeatherBot.Helpers
{
    public static class Constantes
    {
        public readonly static string LuisApplicationID = "luis-app-id";
        public readonly static string LuisAuthorizationKey = "luis-auth-key";
        public readonly static string LuisEndpoint = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/";

        public readonly static string LuisArgs = "LuisEntities";
        public readonly static string LocationLabel = "Location";

        public static string OpenWeatherMapURL = $"http://api.openweathermap.org/data/2.5/weather";
        public readonly static string OpenWeatherMapKey = "llave-open-weather-map";
    }
}