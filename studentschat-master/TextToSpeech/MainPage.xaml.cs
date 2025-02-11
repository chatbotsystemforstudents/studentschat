using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextToSpeech
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //this.Loaded += MainPage_Loaded;
            WriteToLog($"Log path={App.LogPath}");
        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            WriteToLog($"Log path={App.LogPath}");
        }
        private void WriteToLog(string message)
        {
            LogTextBox.Text += $"{DateTime.Now:HH:mm:ss} - {message}\n";

            // Scroll to the bottom
            LogTextBox.Select(LogTextBox.Text.Length, 0);            
        }
    }
}
