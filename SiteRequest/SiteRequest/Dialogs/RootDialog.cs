using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace SiteRequest.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>

    {
        public static string schemaVersion = "1.2.4";
       
        public async Task StartAsync(IDialogContext context)

        {
            context.Wait(MessageReceivedAsync);
        }

        [Obsolete]
        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var replyMessage = context.MakeMessage();
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            dynamic value = activity.Value;
            string _btnValue = string.Empty;
            if (activity.Text == null)

            {
                activity.Text = string.Empty;
            }
            if (activity.Value != null)
            {
                _btnValue = value["button"];
            }

            var message = Microsoft.Bot.Connector.Teams.ActivityExtensions.GetTextWithoutMentions(activity).ToLowerInvariant().Trim();

            if (message.Equals("help") || message.Equals("hi") || message.Equals("hello"))
            {
                await this.WelcomeDialogAsync(context);
            }
            else if (string.IsNullOrEmpty(message) && activity.Value != null)
            {
                if (string.IsNullOrEmpty(message) && _btnValue == "create site")
                {
                    await this.SiteRequestDialogAsync(context);
                }
                else if (string.IsNullOrEmpty(message) && _btnValue.ToString().ToLower().Contains("submit"))
                {
                    await this.validateInputAsync(context, result);
                }
                else if (string.IsNullOrEmpty(message) && _btnValue.ToString().ToLower().Contains("cancel"))
                {
                    await context.PostAsync("Cancelled team request.");
                }
                else
                {
                }
            }
            else
            {


                //Activity isTypingActivity = activity.CreateReply();
                //isTypingActivity.Type = ActivityTypes.Handoff;
                //await context.PostAsync((Activity)isTypingActivity);
                //reply.Attachments.Add(EchoBot.CreateDynamicSiteRequest());
                //await context.PostAsync(reply);
                await this.WelcomeDialogAsync(context);

                //// await HandleActions(context, activity);
            }
        }
        private async Task HandleActions(IDialogContext context, Activity activity)
        {
            var reply = activity.CreateReply();
            reply.Attachments.Add(EchoBot.CreateSiteRequest());
            await context.PostAsync(reply);
        }

        public void DisplayOptionsAsync(IDialogContext context)
        {
            //PromptDialog.Choice<string>(context,this.SelectedOptionAsync,this.options.Keys,"What Demo / Sample option would you like to see?","Please select Valid option 1 to 6",
            //    6,
            //    PromptStyle.PerLine,
            //    this.options.Values);
        }

        public async Task WelcomeDialogAsync(IDialogContext context)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = null;
            attachment = WelcomeAdapativecard();
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);

        }

        public async Task SiteRequestDialogAsync(IDialogContext context)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = null;
            attachment = EchoBot.CreateDynamicSiteRequest();
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
            this.DisplayOptionsAsync(context);
        }
        public async Task validateInputAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            Teams team = new Teams();
            if (message.Value != null)
            {
                // Got an Action Submit
                dynamic formvalue = message.Value;

                if (formvalue != null)
                {

                    team.TeamName = formvalue["Teamname"];
                    team.Description = formvalue["Description"];
                    team.TeamMailNickname = formvalue["TeamMailNickname"];
                    team.TeamOwners = formvalue["TeamOwners"];
                    team.Type = formvalue["Type"];
                    team.Classification = formvalue["Classification"];
                    var error = GetErrorMessage(team); // Validation
                    IMessageActivity replyMessage = context.MakeMessage();
                    if (!string.IsNullOrEmpty(error))
                    {
                        replyMessage.Text = error;
                        await context.PostAsync(replyMessage);
                    }
                    else
                    {

                        //Insert data into database here.



                        // Save Information in service bus
                        replyMessage.Text = "We have submiited your site request";
                        await context.PostAsync(replyMessage);
                        context.Done(true);
                    }

                }
            }
        }

        public async Task SelectedOptionAsync(IDialogContext context)
        {
            var message = "1";

            var replyMessage = context.MakeMessage();

            Attachment attachment = null;

            switch (message)
            {
                case "1":
                    attachment = WelcomeAdapativecard();
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    break;
                case "2":
                    attachment = WelcomeAdapativecard();
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    break;
                case "3":
                    //replyMessage.Attachments = new List<Attachment> { CreateAdapativecardWithColumn(), CreateAdaptiveCardwithEntry() };
                    break;

            }


            await context.PostAsync(replyMessage);

            this.DisplayOptionsAsync(context);
        }
        public Attachment WelcomeAdapativecard()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Cards\WelcomeCard.json");
            var card = File.ReadAllText(path);
            var parsedResult = AdaptiveCard.FromJson(card);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = parsedResult.Card
            };
            return attachment;
        }

        private string GetErrorMessage(Teams team)
        {

            if (string.IsNullOrWhiteSpace(team.TeamName) && string.IsNullOrWhiteSpace(team.TeamOwners) && string.IsNullOrWhiteSpace(team.Description) && string.IsNullOrWhiteSpace(team.TeamMailNickname))
            {
                return "Please fill out all the fields";
            }
            else if (string.IsNullOrWhiteSpace(team.TeamName))
            {
                return "Please fill out Team Name";
            }
            else if (string.IsNullOrWhiteSpace(team.Description))
            {
                return "Please fill out Team Description";
            }
            else if (string.IsNullOrWhiteSpace(team.TeamMailNickname))
            {
                return "Please fill out Team MailNickname";
            }
            else if (string.IsNullOrWhiteSpace(team.TeamOwners))
            {
                return "Please select Team Owner";
            }
            else
            {
                return string.Empty;
            }
        }


    }
}