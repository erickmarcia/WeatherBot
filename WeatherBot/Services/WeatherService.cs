using System.Threading.Tasks;
using WeatherBot.Helpers;
using WeatherBot.Models;

using Newtonsoft.Json;
using System.Net.Http;

namespace WeatherBot.Services
{
    public class WeatherService
    {
        public static async Task<RootObject> GetWeather(string city)
        {
            var query = $"{Constantes.OpenWeatherMapURL}?q={city}&appid={Constantes.OpenWeatherMapKey}";

            using (var client = new HttpClient())
            {
                var getWeather = await client.GetAsync(query);

                if (getWeather != null)
                {
                    var json = getWeather.Content.ReadAsStringAsync().Result;
                    var weather = JsonConvert.DeserializeObject<RootObject>(json);

                    weather.main.temp = weather.main.temp - 273.15;
                    return weather;
                }
            }
            return default(RootObject);
        }
    }
}
