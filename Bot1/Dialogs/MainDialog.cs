// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;



namespace Bot1.Dialogs
{
    public class MainDialog : ComponentDialog
    {               
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

                       
        public MainDialog(UserState userState)
            : base(nameof(MainDialog))            
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            var waterfallSteps = new WaterfallStep[]
          {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
          };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));



            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Seleccione una opci�n"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2", "3" }),
                }, cancellationToken);
      
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["seleccion"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Por favor, ingresa tu nombre.") }, cancellationToken);
            
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

                stepContext.Values["name"] = (string)stepContext.Result;
            
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                userProfile.Seleccion = (string)stepContext.Values["seleccion"];
                userProfile.Name = (string)stepContext.Values["name"];               

                var msg = $"La selecci�n fue: {userProfile.Seleccion} y tu nombre es: {userProfile.Name}.";
              
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
           

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                       
        }
    }
}
