using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tcp;
using TextToSpeech.POCO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TextToSpeech
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static Tcp.Server _server = null;
        public static string LogPath { get; private set; }
        public static ServiceProvider ServiceProvider { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

        }
        private async Task ConfigureServices()
        {
            try
            {
                var serviceCollection = new ServiceCollection();
                var Config = await AppConfiguration.LoadAsync<AppConfig>("appsettings.json");

                var logPath = LogPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs", "SpeechToText.log");

                // Register your services
                Log.Logger = new LoggerConfiguration()
                                    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                                    .CreateLogger();
                //Log.Logger = new LoggerConfiguration()
                //    .WriteTo.Sink(new UwpFileSink("app.log"))
                //    .CreateLogger();
                serviceCollection.AddScoped(x => new ServerConfiguration(Config.SpeechServer.Ip, Config.SpeechServer.Port));
                serviceCollection.AddScoped<Tcp.Server>();
                serviceCollection.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog(); // Add Serilog as the logging provider
                });

                ServiceProvider = serviceCollection.BuildServiceProvider();
                _server = ServiceProvider.GetService<Tcp.Server>();
                Log.Information("Services configured successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ConfigureServices");
            }
        }
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await ConfigureServices();
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();

                EndPoints.Register(_server);                
                Task.Run(() =>
                {
                    _server.StartListening();
                });
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
