using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdaptiveCards;
using BotDialog.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SiteRequest.Controllers;
using SiteRequest.Helpers;
using SiteRequest.Models;

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
                if (string.IsNullOrEmpty(message) && _btnValue == "SubmitSiteProvisioningSettings")
                {
                    await this.SiteProvisioningSettingDialogAsync(context);
                }
                else if (string.IsNullOrEmpty(message) && _btnValue == "create site")
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
                await SendOAuthCardAsync(context, activity);

                //Activity isTypingActivity = activity.CreateReply();
                //isTypingActivity.Type = ActivityTypes.Handoff;
                //await context.PostAsync((Activity)isTypingActivity);
                //reply.Attachments.Add(EchoBot.CreateDynamicSiteRequest());
                //await context.PostAsync(reply);
                //await this.WelcomeDialogAsync(context);

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
            attachment = EchoBot.WelcomeCard();
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);

        }
        public async Task SiteProvisioningSettingDialogAsync(IDialogContext context)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = null;
            attachment = EchoBot.ShowTeamSiteRequestProvisoning();
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
            this.DisplayOptionsAsync(context);
        }
        public async Task SiteRequestDialogAsync(IDialogContext context)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = null;
            DataController dc = new DataController();
            string SiteSettings = dc.getSiteProvisionConfig();
            List<string> SelectedValue = SiteSettings.Split(',').ToList();
            attachment = EchoBot.Test1(SelectedValue);
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
            this.DisplayOptionsAsync(context);

        }
        public async Task validateInputAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            TeamRequest team = new TeamRequest();
            if (message.Value != null)
            {
                // Got an Action Submit
                dynamic formvalue = message.Value;

                if (formvalue != null)
                {

                    team.Name = formvalue["Teamname"];
                    team.Description = formvalue["Description"];
                    team.Alias = formvalue["TeamMailNickname"];
                    team.Owners = formvalue["TeamOwners"];
                    team.SiteType = formvalue["Type"];
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

                        string JSONString = string.Empty;
                        using (var sqLiteDatabase = new SqLiteDatabase())
                        {
                            sqLiteDatabase.OpenConnection();
                            var result1 = sqLiteDatabase.GetDataTable($"SELECT * FROM SiteProvision");
                            string query = string.Empty;
                            if (result1.Rows.Count > 0)
                            {
                                query = $"UPDATE SiteProvision SET ";
                            }
                            else
                            {
                                //query = $"INSERT INTO SiteProvision ('Name','Description', 'Alias', 'SiteType', 'Language','RequestedBy','Owners','Status') VALUES('{name}','{description}','{alias}','{siteType}','{language}','{requestedBy}','{owners}','Requested')";
                            }
                            sqLiteDatabase.ExecuteNonQuery(query);
                            sqLiteDatabase.CloseConnection();
                        }

                        // Save Information in service bus
                        replyMessage.Text = "We have submiited your site request";
                        await context.PostAsync(replyMessage);
                        context.Done(true);
                    }

                }
            }
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
        private string GetErrorMessage(TeamRequest team)
        {

            if (string.IsNullOrWhiteSpace(team.Name) && string.IsNullOrWhiteSpace(team.Owners) && string.IsNullOrWhiteSpace(team.Description) && string.IsNullOrWhiteSpace(team.Alias))
            {
                return "Please fill out all the fields";
            }
            else if (string.IsNullOrWhiteSpace(team.Name))
            {
                return "Please fill out Team Name";
            }
            else if (string.IsNullOrWhiteSpace(team.Description))
            {
                return "Please fill out Team Description";
            }
            else if (string.IsNullOrWhiteSpace(team.Alias))
            {
                return "Please fill out Team MailNickname";
            }
            else if (string.IsNullOrWhiteSpace(team.Owners))
            {
                return "Please select Team Owner";
            }
            else
            {
                return string.Empty;
            }
        }
        private async Task SendOAuthCardAsync(IDialogContext context, Activity activity)
        {
            var reply = await context.Activity.CreateOAuthReplyAsync(ApplicationSettings.ConnectionName, "In order to use Leave Bot we need your basic details, Please sign in", "Sign In").ConfigureAwait(false);
            await context.PostAsync(reply);

            context.Wait(WaitForToken);
        }
        private async Task WaitForToken(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var tokenResponse = activity.ReadTokenResponseContent();
            if (tokenResponse != null)
            {
                await this.SiteRequestDialogAsync(context);
            }
            else
            {
                if (!string.IsNullOrEmpty(activity.Text))
                {
                    tokenResponse = await context.GetUserTokenAsync(ApplicationSettings.ConnectionName, activity.Text);
                    if (tokenResponse != null)
                    {
                        await this.SiteRequestDialogAsync(context);
                    }
                }
                else
                {
                    await context.PostAsync($"Hmm. Something went wrong. Let's try again.");
                    await SendOAuthCardAsync(context, activity);
                }

            }
        }
    }
}