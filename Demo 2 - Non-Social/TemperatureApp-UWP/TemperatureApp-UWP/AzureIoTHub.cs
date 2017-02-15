using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using TemperatureApp_UWP.Model;
using Newtonsoft.Json;

static class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "TemperatureUWP". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "<Insert Connection String>";

    //
    // To monitor messages sent to device "TemperatureUWP" use iothub-explorer as follows:
    //    iothub-explorer HostName=ignite-demo-hub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=pPjN8l/DC76LcrTT186a59/vGrBoHxkblEjZ9OUMSp0= monitor-events "TemperatureUWP"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    public static async Task SendDeviceToCloudMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        Random rnd = new Random();
        var tempObj = new TemperatureObject()
        {
            temp = rnd.Next(20,45) + rnd.NextDouble(),
            datetime = DateTime.Now
        };

        var str = JsonConvert.SerializeObject(tempObj);

        var message = new Message(Encoding.ASCII.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
