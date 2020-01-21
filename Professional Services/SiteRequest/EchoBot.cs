using AdaptiveCards;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

using ProfessionalServices.SiteRequest.Helpers;
using ProfessionalServices.SiteRequest.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TaskModule;

namespace ProfessionalServices.SiteRequest
{
    public class EchoBot
    {
        public static string schemaVersion = "1.2.4";
        public static Attachment WelcomeLeaveCard()
        {
            var card = File.ReadAllText(@"D:\ChatBot\Professional Services\SiteRequest\Cards\WelcomeCard.json");
            var parsedResult = AdaptiveCard.FromJson(card);
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = parsedResult.Card
            };
            return attachment;

        }
       
        public static Attachment CreateDynamicSiteRequest()
        {

            List<string> _teamsTemplateType = new List<string>
                    {
                        { "Channels" },
                        {"Tabs" },
                         { "Team Settings" },
                        {"App" },
                          {"Members"}

                    };
            List<string> _teamsType = new List<string>
                    {
                        { "Public" },
                        {"Private" }
                    };
            List<string> _teamsClassification = new List<string>
                    {
                        { "Internal" },
                        {"External" },
                        { "Business" },
                        {"Protected" },
                        { "Important" },
                        {"Personal" }
                    };

            //var choicesTeamOwners = _teamsa.Select(s => new AdaptiveChoice { Title = s.Key, Value = s.Key }).ToList();
            var choicesType = _teamsType.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();
            var choicesClassification = _teamsClassification.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();
            var ChoiceTemplateType = _teamsTemplateType.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();

            var Card = new AdaptiveCard(schemaVersion)
            {


                Body = new List<AdaptiveElement>()
                  {
                    new AdaptiveTextBlock()
                    {
                        Text="Display Name*",
                        Separator=true,
                    },
                    new AdaptiveTextInput()

                    {
                        Id="Teamname",
                        IsMultiline=false,
                        Style = AdaptiveTextInputStyle.Text,
                        Height=AdaptiveHeight.Auto
                    },

                    new AdaptiveTextBlock()
                    {
                        Text="Description*"
                    },
                    new AdaptiveTextInput()

                    {
                        Id="Description*",
                        IsMultiline=false,
                        Style = AdaptiveTextInputStyle.Text,
                    },

                    new AdaptiveTextBlock()
                    {
                        Text="Classifiction"
                    },
                    new AdaptiveChoiceSetInput()
                    {
                        Choices = choicesClassification,
                        Id = "Classifiction",
                        Style = AdaptiveChoiceInputStyle.Compact,
                        IsMultiSelect = false
                    },

                    new AdaptiveTextBlock()
                    {
                        Text="Site Type"
                    },
                },


                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveShowCardAction()
                    {

                        Title = "Microsoft Teams",
                        Card = new AdaptiveCard(schemaVersion)
                        {
                            Body = new List<AdaptiveElement>()
                            {


                               new AdaptiveTextBlock()
                               {
                                   Text="Team MailNickname*"
                               },

                                new AdaptiveTextInput()

                                {
                                    Id="MicrosoftTeamMailNickname",
                                    IsMultiline=false,
                                    Style = AdaptiveTextInputStyle.Text,
                                },

                                new AdaptiveTextBlock()
                                {
                                    Text="Type"
                                },

                                new AdaptiveChoiceSetInput()
                                {
                                    Choices = choicesType,
                                    Id = "Type",
                                    Style = AdaptiveChoiceInputStyle.Compact,
                                    IsMultiSelect = false
                                },
                                new AdaptiveTextBlock()
                                {
                                    Text="Do you want to create a team using exsting team as a template?"
                                },
                            },

                            Actions = new List<AdaptiveAction>()
                            {
                                new AdaptiveShowCardAction()
                                {
                                    Title = "Yes",
                                    Card = new AdaptiveCard(schemaVersion)
                                    {

                                        Body = new List<AdaptiveElement>()
                                        {
                                        new AdaptiveTextBlock()
                                        {
                                            Text="Select Template"
                                        },
                                        new AdaptiveChoiceSetInput()
                                        {
                                            Choices = choicesType,
                                            Id = "MicrosoftTeamsSelectTemplate",
                                            Style = AdaptiveChoiceInputStyle.Compact,
                                            IsMultiSelect = false
                                        },

                                        new AdaptiveTextBlock()
                                        {
                                            Text="Choose what you'd like to include from the original team Messages, files and content won't be copied. You'll need to set up tabs and connectors again.",
                                            Wrap=true
                                        },

                                        new AdaptiveChoiceSetInput()
                                        {
                                            Choices = ChoiceTemplateType,
                                            Id = "MicrosoftTeamsIncludeTeamMessages",
                                            Style = AdaptiveChoiceInputStyle.Expanded,
                                            IsMultiSelect = true
                                        },

                                    },
                                        Actions = new List<AdaptiveAction>()
                                        {
                                            new AdaptiveSubmitAction()
                                              {
                                                  Title = "Create Team",
                                                  Id="btnCreateMicrosoftTeam",
                                                  Type = "Action.Submit",
                                                  Style="destructive",
                                                  Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "SubmitActionFromMicrosoftTeam" })
                                              },


                                              new AdaptiveSubmitAction()
                                              {
                                                  Title = "Cancel",
                                                  Type = "Action.Submit",
                                                  Style="destructive",
                                                  Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "CancelActionFromMicrosoftTeam" })
                                              }


                                         }
                                    }
                                },
                                 new AdaptiveShowCardAction()
                                {
                                    Title = "No",
                                    Card = new AdaptiveCard(schemaVersion)
                                    {

                                        Body = new List<AdaptiveElement>()
                                        {

                                            new AdaptiveTextBlock()
                                        {
                                            Text="Owner*"

                                        },

                                        new AdaptiveChoiceSetInput()
                                        {
                                            Choices = choicesClassification,
                                            Id = "MicrosoftTeamsOwner",
                                            Style = AdaptiveChoiceInputStyle.Compact,
                                            IsMultiSelect = false
                                        },
                                        new AdaptiveTextBlock()
                                        {
                                            Text="Member*"
                                        },

                                        new AdaptiveChoiceSetInput()
                                        {
                                            Choices = choicesClassification,
                                            Id = "MicrosoftTeamsMember",
                                            Style = AdaptiveChoiceInputStyle.Compact,
                                            IsMultiSelect = false
                                        },

                                    },
                                        Actions = new List<AdaptiveAction>()
                                        {
                                            new AdaptiveSubmitAction()
                                              {
                                                  Title = "Create Team",
                                                  Id="btnCreateMicrosoftTeamWithNoTemplate",
                                                  Type = "Action.Submit",
                                                  Style="destructive",
                                                  Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "SubmitActionFromMicrosoftTeamWithNoTemplate" })
                                              },


                                              new AdaptiveSubmitAction()
                                              {
                                                  Title = "Cancel",
                                                  Type = "Action.Submit",
                                                  Style="destructive",
                                                  Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "CancelActionFromMicrosoftTeamWithNoTemplate" })
                                              }

                                         }
                                    }
                                }

                            }
                        }
                    },


                    new AdaptiveShowCardAction()
                    {
                        Title = "SharePoint Site",
                        Card = new AdaptiveCard(schemaVersion)
                        {
                            Body = new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text="Owner*"
                                },

                                new AdaptiveChoiceSetInput()
                                {
                                    Choices = choicesClassification,
                                    Id = "SharepointOwner",
                                    Style = AdaptiveChoiceInputStyle.Compact,
                                    IsMultiSelect = false
                                },


                                new AdaptiveTextBlock()
                                {
                                    Text="Member*"
                                },

                                new AdaptiveChoiceSetInput()
                                {
                                    Choices = choicesClassification,
                                    Id = "SharepointMember",
                                    Style = AdaptiveChoiceInputStyle.Compact,
                                    IsMultiSelect = false
                                },

                                new AdaptiveTextBlock()
                                {
                                    Text="Choose the type of site you'd like to create."
                                },
                            },
                            Actions = new List<AdaptiveAction>()
                            {
                                new AdaptiveShowCardAction()
                                {
                                    Title = "Team Site",
                                    Card = new AdaptiveCard(schemaVersion)
                                    {
                                        Body = new List<AdaptiveElement>()
                                        {

                                            new AdaptiveTextBlock()
                                            {
                                                Text="Type"
                                            },

                                            new AdaptiveChoiceSetInput()
                                            {
                                                Choices = choicesType,
                                                Id = "teamType",
                                                Style = AdaptiveChoiceInputStyle.Compact,
                                                IsMultiSelect = false
                                           },
                                        },

                                        Actions = new List<AdaptiveAction>()
                                        {
                                            new AdaptiveSubmitAction()
                                            {
                                                Title = "Create Team",
                                                Id="btnCreateTeamSite",
                                                Type = "Action.Submit",
                                                Style="destructive",
                                                Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "SubmitActionfromSharePointSiteamSite" })
                                            },

                                            new AdaptiveSubmitAction()
                                            {
                                               Title = "Cancel",
                                               Type = "Action.Submit",
                                               Style="destructive",
                                               Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "CancelActionfromSharePointSiteamSite" })
                                            }
                                        }
                                     }
                              },

                                new AdaptiveShowCardAction()
                                {
                                    Title = "Communication site",
                                    Card = new AdaptiveCard(schemaVersion)
                                    {
                                        Actions = new List<AdaptiveAction>()
                                        {
                                            new AdaptiveSubmitAction()
                                            {

                                                Title = "Create Team",
                                                Id="btnCreateCommunication site",
                                                Type = "Action.Submit",
                                                Style="destructive",
                                                Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "SubmitActionfromSharePointCommunicationSite" })
                                            },

                                            new AdaptiveSubmitAction()
                                            {
                                                Title = "Cancel",
                                                Type = "Action.Submit",
                                                Style="destructive",
                                                Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "CancelActionfromSharePointCommunicationSite" })
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };


            return new Attachment()
            {

                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };
        }

        public static Attachment CreateSiteRequest()
        {
            List<string> _teamsType = new List<string>
                    {
                        { "Public" },
                        {"Private" }
                    };
            List<string> _teamsClassification = new List<string>
                    {
                        { "Internal" },
                        {"External" },
                        { "Business" },
                        {"Protected" },
                        { "Important" },
                        {"Personal" }
                    };

            //var choicesTeamOwners = _teamsa.Select(s => new AdaptiveChoice { Title = s.Key, Value = s.Key }).ToList();
            var choicesType = _teamsType.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();
            var choicesClassification = _teamsClassification.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();


            var Card = new AdaptiveCard(schemaVersion)
            {

                Body = new List<AdaptiveElement>()
                          {
                              new AdaptiveTextBlock()
                              {
                              Text="Team Display Name*"

                              },
                              new AdaptiveTextInput()
                              {
                               Id="Teamname",
                               IsMultiline=false,
                               Style = AdaptiveTextInputStyle.Text

                              },
                               new AdaptiveTextBlock()
                              {
                              Text="Team Description*"

                              },
                              new AdaptiveTextInput()
                              {
                               Id="Description*",
                               IsMultiline=false,
                               Style = AdaptiveTextInputStyle.Text

                              },
                               new AdaptiveTextBlock()
                              {
                              Text="Team MailNickname*"

                              },
                              new AdaptiveTextInput()
                              {
                               Id="TeamMailNickname",
                               IsMultiline=false,
                               Style = AdaptiveTextInputStyle.Text,

                              },

                               new AdaptiveTextBlock()
                              {
                              Text="Team Owner*"
                              },
                              new AdaptiveChoiceSetInput()
                              {
                               Choices = choicesType,
                                Id = "TeamOwner",
                                Style = AdaptiveChoiceInputStyle.Compact,
                                IsMultiSelect = false

                              },
                                new AdaptiveTextBlock()
                              {
                                 Text="Type"
                              },
                                 new AdaptiveChoiceSetInput()
                              {
                                 Choices = choicesType,
                                Id = "Type",
                                Style = AdaptiveChoiceInputStyle.Compact,
                                IsMultiSelect = false
                              },
                                   new AdaptiveTextBlock()
                              {
                                 Text="Classifiction"
                              },
                                 new AdaptiveChoiceSetInput()
                              {
                                 Choices = choicesClassification,
                                Id = "Classifiction",
                                Style = AdaptiveChoiceInputStyle.Compact,
                                IsMultiSelect = false
                              },
                          },

                Actions = new List<AdaptiveAction>()
                          {
                              new AdaptiveSubmitAction()
                              {
                                  Title = "Create Team",
                                          Id="btnCreateTeam",
                                          Type = "Action.Submit",
                                          Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "submit" })

                              },
                               new AdaptiveSubmitAction()
                              {
                                 Title = "Cancel",
                                 Type = "Action.Submit",
                                 Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "cancel" })

                              }
                          }


            };

            return new Attachment()
            {

                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };
        }

    }

}
