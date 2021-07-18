using System;
using System.Collections.ObjectModel;
using EmbedIO;
using Xamarin.Forms;

namespace linkopener.tv
{
    public class App : Application
    {
        // IP of smart TV
        private const string UrlFormat = "http://{0}:9696/";
        // server to listen put commands
        private WebServer server;

        // Link holder
        private ObservableCollection<string> linkCollection = new ObservableCollection<string>
        {
            "https://ypyl.github.io/",
        };
        public App()
        {
            var listView = new ListView { ItemsSource = linkCollection };
            listView.ItemTapped += (sender, e) => OpenInBrowser(e.Item.ToString());
            MainPage = new ContentPage
            {
                Content = listView
            };
        }

        // Open selected link in the integrated browser on Smart TV
        private static void OpenInBrowser(string link) =>
            Tizen.Applications.AppControl.SendLaunchRequest(new Tizen.Applications.AppControl
            {
                Operation = Tizen.Applications.AppControlOperations.View,
                Uri = link,
                LaunchMode = Tizen.Applications.AppControlLaunchMode.Single
            });

        // Create web server to listen PUT with links that we want to open
        private static WebServer CreateWebServer(string url, ObservableCollection<string> linkCollection) =>
            new WebServer(o => o
                .WithUrlPrefix(string.Format(UrlFormat, url))
                .WithMode(HttpListenerMode.EmbedIO))
                .OnPut("/", async ctx =>
                {
                    var httpLink = await ctx.GetRequestBodyAsStringAsync();
                    if (Uri.TryCreate(httpLink, UriKind.Absolute, out _))
                    {
                        linkCollection.Insert(0, httpLink);
                        await ctx.SendDataAsync(null);
                    }
                    else
                    {
                        await ctx.SendDataAsync(new { Error = "Incorrect link." });
                    }
                });

        protected override void OnStart() => RunServer();

        private void RunServer()
        {
            var currentIP = Tizen.Network.Connection.ConnectionManager.GetIPAddress(Tizen.Network.Connection.AddressFamily.IPv4);
            this.server = this.server ?? CreateWebServer(currentIP.ToString(), this.linkCollection);
            server.RunAsync();
        }

        protected override void OnSleep() => this.server?.Dispose();

        protected override void OnResume() => RunServer();
    }
}
