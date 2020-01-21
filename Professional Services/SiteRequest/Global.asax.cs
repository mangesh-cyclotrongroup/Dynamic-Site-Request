using Autofac;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using ProfessionalServices.SiteRequest;
using ProfessionalServices.SiteRequest.Helpers;

namespace ProfessionalServices.SiteRequest
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
