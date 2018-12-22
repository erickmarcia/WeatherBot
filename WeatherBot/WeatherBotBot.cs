using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using WeatherBot.Helpers;
using WeatherBot.Models;
using WeatherBot.Containers;
using Microsoft.Extensions.Configuration;

namespace WeatherBot
{
    public class WeatherBotBot : IBot
    {
        private LuisRecognizer LuisRecognizer { get; } = null;
        private DialogSet _dialogs { get; } = null;

        //private readonly WeatherBotAccessors _accessors;
        //private readonly ILogger _logger;

        public WeatherBotBot(IConfiguration c)
        {
            _dialogs = ComposeRootDialog();

            var options = new LuisRecognizerOptions { Verbose = true };
            var luisModel = new LuisModel(Constantes.LuisApplicationID,
                Constantes.LuisAuthorizationKey, 
                new Uri(Constantes.LuisEndpoint));
            LuisRecognizer = new LuisRecognizer(luisModel, options, null);
        }

        private DialogSet ComposeRootDialog()
        {
            var dialogs = new DialogSet();

            try
            {
                dialogs.Add(nameof(WeatherBot), new WaterfallStep[]
                {
                async (dc, args, next) =>
                {
                    try
                    {
                        var utterance = dc.Context.Activity.Text?.Trim().ToLowerInvariant();

                        if (!string.IsNullOrEmpty(utterance))
                        {
                            // Decide which dialog to start based on top scoring Luis intent
                            var luisResult = await LuisRecognizer.Recognize<WeatherLuisModel>(utterance, new CancellationToken());

                            // Decide which dialog to start.
                            switch (luisResult.TopIntent().intent)
                            {
                                case WeatherLuisModel.Intent.Get_Weather_Condition:
                                    var dialogArgs = new Dictionary<string, object>();
                                    dialogArgs.Add(Constantes.LuisArgs, luisResult.Entities);
                                    await dc.Begin(nameof(WeatherDialogContainer), dialogArgs);
                                    break;
                            }
                        }
                        else
                        {
                            await dc.End();
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                }
                });

            }
            catch(Exception ex)
            {

            }
            // Add our child dialogs.
            dialogs.Add(nameof(WeatherDialogContainer), WeatherDialogContainer.Instance);
            return dialogs;
        }

        public async Task OnTurn(ITurnContext context)
        {
            var conversationInfo = ConversationState<ConversationInfo>.Get(context);

            // Establish dialog state from the conversation state.
            var dc = _dialogs.CreateContext(context, conversationInfo);

            // This bot handles only messages for simplicity. Ideally it should handle new conversation joins and other non message based activities. See the CafeBot examples
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Continue any current dialog.
                await dc.Continue();

                // If this is not a repsonse, start the main root dialog
                if (!context.Responded)
                {
                    await dc.Begin(nameof(WeatherBot));
                }
            }
        }
    }
}
