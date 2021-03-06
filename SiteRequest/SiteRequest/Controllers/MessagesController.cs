﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace SiteRequest
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                await HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task HandleSystemMessage(Activity message)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));

            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    if (message.MembersAdded.Any(m => m.Id == message.Recipient.Id))
                    {
                        //ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                        var welcomeMsg = message.CreateReply("Welcome to the Bot...!");
                        // await connector.Conversations.ReplyToActivityAsync(welcomeMsg);
                        await Conversation.SendAsync(message, () => new Dialogs.RootDialog());
                    }

                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;
                case ActivityTypes.Typing:
                    
                    var reply = message.CreateReply(String.Empty);
                    reply.Type = ActivityTypes.Typing;
                    await connector.Conversations.ReplyToActivityAsync((Activity)reply);

                    break;

            }
        }
    }
}