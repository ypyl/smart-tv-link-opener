using System;
using System.Collections.ObjectModel;
using EmbedIO;
using Xamarin.Forms;

namespace linkopener.tv
{
    public class App : Application
    {
        // IP of smart TV
        private const string Url = "http://192.168.0.106:9696/";
        // server to listen put commands
        private WebServer server;

        // Link holder
        private ObservableCollection<string> linkCollection = new ObservableCollection<string>
        {
            "https://ypyl.github.io/",
        };
        public App()
        {
            var listView = new ListView();
            listView.ItemsSource = linkCollection;
            listView.ItemTapped += (sender, e) =>
            {
                // open selected link in the integrated browser on Smart TV
                var app = new Tizen.Applications.AppControl();
                app.Operation = Tizen.Applications.AppControlOperations.View;
                app.Uri = e.Item.ToString();
                app.LaunchMode = Tizen.Applications.AppControlLaunchMode.Single;
                Tizen.Applications.AppControl.SendLaunchRequest(app);
            };
            // The root page of your application
            MainPage = new ContentPage
            {
                Content = listView
            };
        }

        // Create web server to listen PUT with links that we want to open
        private static WebServer CreateWebServer(ObservableCollection<string> linkCollection)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(Url)
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
            // server.StateChanged += (s, e) => linkCollection.Insert(0, $"WebServer New State - {e.NewState}");
            // server.OnHttpException += (s, e) =>
            // {
            //     linkCollection.Insert(0, $"HttpEx - {e.StatusCode}");
            //     return Task.CompletedTask;
            // };
            // server.OnUnhandledException += (s, e) =>
            // {
            //     linkCollection.Insert(0, $"Exception - {e.Message}");
            //     return Task.CompletedTask;
            // };

            return server;
        }

        protected override void OnStart()
        {
            this.server = this.server ?? CreateWebServer(this.linkCollection);
            server.RunAsync();
            // catch (Exception ex)
            // {
            //     this.linkCollection.Insert(0, ex.Message);
            // }
            // finally
            // {
            //     this.linkCollection.Insert(0, "Created server");
            // }
        }

        protected override void OnSleep()
        {
            this.server?.Dispose();
            // }
            // catch (Exception ex)
            // {
            //     this.linkCollection.Insert(0, ex.Message);
            // }
            // finally
            // {
            //     this.linkCollection.Insert(0, "Disposed");
            // }

        }

        protected override void OnResume()
        {
            this.server = this.server ?? CreateWebServer(this.linkCollection);
            server.RunAsync();
        }
    }
}
