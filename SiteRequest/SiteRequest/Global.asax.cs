using Autofac;
using SiteRequest.Helpers;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace SiteRequest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            Conversation.UpdateContainer(builder =>
            {

                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
                //     var store = new TableBotDataStore(ConfigurationManager.AppSettings["BotStateEndpoint"]);
                var store = new InMemoryDataStore();
                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();


                //builder.Register(c => new CachingBotDataStore(store,
                //          CachingBotDataStoreConsistencyPolicy
                //          .ETagBasedConsistency))
                //          .As<IBotDataStore<BotData>>()
                //          .AsSelf()
                //          .InstancePerLifetimeScope();
            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        //in global.asax or global.asax.cs
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            ErrorLogService.LogError(ex);
        }
    }
}
