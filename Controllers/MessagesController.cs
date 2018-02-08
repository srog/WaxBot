using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        //await Conversation.SendAsync(activity, () => new EchoDialog());
                        //await Conversation.SendAsync(activity, () => EchoCommandDialog.dialog);
                        //await Conversation.SendAsync(activity, () => new EchoAttachmentDialog());
                        await Conversation.SendAsync(activity, MakeRoot);
                        break;

                    case ActivityTypes.ConversationUpdate:
                        IConversationUpdateActivity update = activity;
                        using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                        {
                            var client = scope.Resolve<IConnectorClient>();
                            if (update.MembersAdded.Any())
                            {
                                var reply = activity.CreateReply();
                                foreach (var newMember in update.MembersAdded)
                                {
                                    if (newMember.Id != activity.Recipient.Id)
                                    {
                                        reply.Text = $"Welcome to the Wax Bot, {newMember.Name}!";
                                    }
                                    //else
                                    //{
                                    //    reply.Text = $"Welcome to the Wax Bot, {activity.From.Name}";
                                    //}

                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                        }

                        break;
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    case ActivityTypes.Ping:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);


            // check if activity is of type message
            //if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            //{
            //    await Conversation.SendAsync(activity, () => new EchoDialog());
            //}
            //else
            //{
            //    HandleSystemMessage(activity);
            //}

        }

        private static IDialog<object> MakeRoot()
        {
            return Chain.From(() => new RootDialog());
        }

        //private Activity HandleSystemMessage(Activity message)
        //{
        //    if (message.Type == ActivityTypes.DeleteUserData)
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (message.Type == ActivityTypes.ConversationUpdate)
        //    {
        //        // Handle conversation state changes, like members being added and removed
        //        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //        // Not available in all channels
        //    }
        //    else if (message.Type == ActivityTypes.ContactRelationUpdate)
        //    {
        //        // Handle add/remove from contact lists
        //        // Activity.From + Activity.Action represent what happened
        //    }
        //    else if (message.Type == ActivityTypes.Typing)
        //    {
        //        // Handle knowing tha the user is typing
        //    }
        //    else if (message.Type == ActivityTypes.Ping)
        //    {
        //    }

        //    return null;
        //}
    }
}