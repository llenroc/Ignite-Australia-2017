using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TemperatureApp_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool SendDataToAzureIoTHub = false;

        public MainPage()
        {
            this.InitializeComponent();

            Task.Run(
             async () =>
             {
                 while (true)
                 {
                     var message = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
                     await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                     {
                         textBox.Text += Environment.NewLine + message;
                         if (message.ToLower().Contains("alert"))
                             textBox1.IsEnabled = true;
                         else
                             textBox1.IsEnabled = false;
                     });
                 }
             }
             );
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => { await AzureIoTHub.SendDeviceToCloudMessageAsync(); });
        }

        private async Task SendDataToAzure()
        {
            while (true)
            {
                if (SendDataToAzureIoTHub)
                {
                    await AzureIoTHub.SendDeviceToCloudMessageAsync();
                }
                await Task.Delay(1000);
            }
        }

        private void toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            toggleButton.Content = "Sending Data to IoT Suite";
            SendDataToAzureIoTHub = true;
            SendDataToAzure();
        }

        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            toggleButton.Content = "Send Continuous D2C";
            SendDataToAzureIoTHub = false;
        }
    }
}
