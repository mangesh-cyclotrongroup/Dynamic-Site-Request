using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdaptiveCards;
using Microsoft.Bot.Connector;

namespace SiteRequest
{
    public class EchoBot
    {
        public static string schemaVersion = "1.2.4";
        public static Attachment WelcomeCard()
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
        public static Attachment Test1(List<string> SelectedValue)
        {
            bool IsMicrosoftTeam = false;
            bool SharepointSite = false;
            bool YammerGroup = false;
            foreach (var Value in SelectedValue)
            {
                if (Value == "Microsoft Teams")
                {
                    IsMicrosoftTeam = true;
                }
                else if (Value == "Sharepoint Site")
                {
                    SharepointSite = true;
                }
                else if (Value == "Yammer Group")
                {
                    YammerGroup = true;
                }
                else
                {

                }
            }
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

            var card = new AdaptiveCard("1.0");
            List<AdaptiveElement> Conatiner1 = new List<AdaptiveElement>
            {
                new AdaptiveColumnSet()
                {
                    Columns = new List<AdaptiveColumn>()
                    {
                        new AdaptiveColumn()
                        {
                            Width=AdaptiveColumnWidth.Stretch,
                            Items = new List<AdaptiveElement>()
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
                            Separator =true,
                        }
                    }
                }
            };
            if (!IsMicrosoftTeam && !SharepointSite && !YammerGroup)
            {
                AdaptiveContainer Conatainer2 = new AdaptiveContainer();
                Conatainer2.Items = Conatiner1;
                card.Body.Add(Conatainer2);

                card.Body.Add(new AdaptiveTextBlock() { Text = "Leave Request", Color = AdaptiveTextColor.Accent, Size = AdaptiveTextSize.Medium, Spacing = AdaptiveSpacing.None, HorizontalAlignment = AdaptiveHorizontalAlignment.Left });

                card.Body.Add(new AdaptiveTextInput() { Id = "Leave Request" });
                card.Actions.Add(new AdaptiveShowCardAction()
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
                                    Text="Do you want to create a team using exsting team as a template?",
                                    Wrap=true
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

                }
                );
                card.Actions.Add(new AdaptiveShowCardAction()
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

                });
            }
            else if (IsMicrosoftTeam)
            {
                AdaptiveContainer Conatainer2 = new AdaptiveContainer();
                Conatainer2.Items = Conatiner1;
                card.Body.Add(Conatainer2);
                card.Actions.Add(new AdaptiveShowCardAction()
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
                                    Text="Do you want to create a team using exsting team as a template?",
                                    Wrap=true
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

                }
                );
            }
            else if (SharepointSite)
            {
                AdaptiveContainer Conatainer2 = new AdaptiveContainer();
                Conatainer2.Items = Conatiner1;
                card.Body.Add(Conatainer2);
                card.Actions.Add(new AdaptiveShowCardAction()
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
                });
            }

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;

        }
        public static Attachment CreateTest(List<string> SelectedValue)
        {
            bool IsMicrosoftTeam = false;
            bool SharepointSite = false;
            bool YammerGroup = false;
            foreach (var Value in SelectedValue)
            {
                if (Value == "Microsoft Teams")
                {
                    IsMicrosoftTeam = true;
                }
                else if (Value == "Sharepoint Site")
                {
                    SharepointSite = true;
                }
                else if (Value == "Yammer Group")
                {
                    YammerGroup = true;
                }
                else
                {

                }
            }

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
                     new AdaptiveContainer
                    {

                         Items = new List<AdaptiveElement>()
                        {
                             new AdaptiveColumnSet()
                            {
                                 Columns=new List<AdaptiveColumn>()
                                {
                                 new AdaptiveColumn()
                                    {

                                     Items=new List<AdaptiveElement>()
                                         {
                                             new AdaptiveTextBlock(){Text="Test1",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium,HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None }
                                         }
                                     },

                               new AdaptiveColumn()
                               {
                                   Items=new List<AdaptiveElement>()
                                         {
                                             new AdaptiveTextInput(){Id="Test1" }
                                         }
                                },
                                  new AdaptiveColumn()

                                  {
                                      Items=new List<AdaptiveElement>()
                                         {
                                             new AdaptiveTextBlock(){Text="Test2",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium,HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None }
                                         }
                                  }
                                 }
                             }
                         }
                     }
                }
            };

            if (IsMicrosoftTeam)
                (Card.Body[0] as AdaptiveContainer).Items.Insert(0,
                       new AdaptiveColumnSet()
                       {
                           Columns = new List<AdaptiveColumn>()
                           {
                                new AdaptiveColumn()
                                    {
                               Items = new List<AdaptiveElement>()

                           {

                               new AdaptiveTextBlock(){Text="",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.None, HorizontalAlignment=AdaptiveHorizontalAlignment.Center}

                           },SelectAction = new AdaptiveSubmitAction()
                                         {
                                            Title="Pending Approvals"
                                         }
                                },

                           }
                       }
                    );



            return new Attachment()
            {

                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };
        }
        public static Attachment ShowTeamSiteRequestProvisoning()
        {
            List<string> _TeamSiteRequestProvisoning = new List<string>
                    {
                        { "Microsoft Teams" },
                        {"Sharepoint Site" },
                         { "Yammer Group" }


                    };
            var choicesType = _TeamSiteRequestProvisoning.Select(s => new AdaptiveChoice { Title = s, Value = s }).ToList();
            var Card = new AdaptiveCard(schemaVersion)
            {
                Body = new List<AdaptiveElement>()
                {
                       new AdaptiveTextBlock()
                    {
                        Text="Select the type of team you want to create",
                        Wrap=true
                    },
                  new AdaptiveChoiceSetInput()
                    {
                        Choices = choicesType,
                        Id = "SiteProvisioningType",
                        Style = AdaptiveChoiceInputStyle.Expanded,
                        IsMultiSelect = true
                    },
                },
                Actions = new List<AdaptiveAction>()
                {

                    new AdaptiveSubmitAction()
                    {
                      Title = "Submit",
                      Id="btnSubmitSiteProvisioningSettings",
                      Type = "Action.Submit",
                      Data = Newtonsoft.Json.Linq.JObject.FromObject(new { button = "create site" })
                    },
                }


            };

            return new Attachment()
            {

                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };
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
                                    Text="Do you want to create a team using exsting team as a template?",
                                    Wrap=true
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