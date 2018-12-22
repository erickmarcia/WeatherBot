using WeatherBot.Helpers;
using WeatherBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Core.Extensions;
using WeatherBot.Services;
using System.Linq;

namespace WeatherBot.Containers
{
    public class WeatherDialogContainer : DialogContainer
    {
        public static WeatherDialogContainer Instance { get; } = new WeatherDialogContainer();

        private WeatherDialogContainer() : base(nameof(WeatherDialogContainer))
        {
            this.Dialogs.Add(nameof(WeatherDialogContainer), new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
                    // Initialize state.
                    if(args!=null && args.ContainsKey(Constantes.LuisArgs))
                    {
                        // Add any LUIS entities to the active dialog state. Remove any values that don't validate, and convert the remainder to a dictionary.
                        var entities = (WeatherLuisModel._Entities)args[Constantes.LuisArgs];
                        dc.ActiveDialog.State = LuisValidators.Validate(entities);
                    }
                    else
                    {
                        // Begin without any information collected.
                        dc.ActiveDialog.State = new Dictionary<string,object>();
                    }

                    // Display user's choice
                    await dc.Context.SendActivity("OK, we're going to get the weather.");
                    await next();
                },
                async (dc, args, next) =>
                {
                    var location = "Zlín";
                    var weather = "N/A";

                    if (dc.ActiveDialog.State.ContainsKey(Constantes.LocationLabel))
                    {
                        location = dc.ActiveDialog.State[Constantes.LocationLabel].ToString();
                    }

                    var ro = await WeatherService.GetWeather(location);
                    weather = $"{ro.weather.First().main} ({ro.main.temp.ToString("N2")} °C)";

                    //await dc.Context.SendActivity($"Weather of {location} is: {weather}");

                    
                    var typing = Activity.CreateTypingActivity();
                        var delay = new Activity { Type = "delay", Value = 5000 };
                        await dc.Context.SendActivities(
                            new IActivity[]
                            {
                                typing, delay,
                                MessageFactory.Text($"Weather of {location} is: {weather}")
                            });
                            
                },
                async (dc, args, next) =>
                {
                    // Prompt the user to do something else
                    await dc.Context.SendActivity("OK, we're done here. What is next?");
                },
                async (dc, args, next) =>
                {
                    await dc.End();
                }
            });

            // Add the prompts and child dialogs
            this.Dialogs.Add(Constantes.LocationLabel, new Microsoft.Bot.Builder.Dialogs.TextPrompt());
        }
    }
}
