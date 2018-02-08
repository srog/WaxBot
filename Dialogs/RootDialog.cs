using System;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;


namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "hi" || message.Text == "hello")
            {
                await context.PostAsync("Hello to you too!");
                return;
            }

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!");

                return;

            }

            if (message.Text == "bye")
            {
                
                return;
            }


            if (message.Text == "status")
            {
                await context.PostAsync("Status is Good!!!");
                return;
            }

            if (message.Text == "weather")
            {
                await context.PostAsync("It's raining. Just a guess!");
                return;
            }

            await context.PostAsync($"{this.count++}: You said: '{message.Text}'");
            context.Wait(MessageReceivedAsync);
         
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}