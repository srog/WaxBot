using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;

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

            if (message.Text.Contains("report"))
            {
                PromptDialog.Choice(context, AfterReportAsync, new string[] { "Sales", "Marketing", "Dev" }, 
                    "Please Select A Report To Run",null, 3, PromptStyle.Auto, null);
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

        public async Task AfterReportAsync(IDialogContext context, IAwaitable<string> result)
        {
            var confirm = await result;
            if (confirm == "Sales")
            {            
                await context.PostAsync("Running sales report....");
            }
            if (confirm == "Marketing")
            {
                await context.PostAsync("Running Marketing report....");
            }
            if (confirm == "Dev")
            {
                await context.PostAsync("Running dev report....");
            }

            context.Wait(MessageReceivedAsync);
        }

    }
}