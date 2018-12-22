using WeatherBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace WeatherBot.Helpers
{
    public static class LuisValidators
    {
        public static Dictionary<string, object> Validate(WeatherLuisModel._Entities entities)
        {
            var result = new Dictionary<string, object>();

            if (entities?.Location?.Any() is true)
            {
                var label = entities.Location.FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
                if (label != null)
                {
                    result[Constantes.LocationLabel] = label;
                }
            }

            if (entities?.Location_PatternAny.Any() is true)
            {
                var label = entities.Location_PatternAny.FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
                if (label != null)
                {
                    result[Constantes.LocationLabel] = label;
                }
            }

            return result;
        }
    }
}
