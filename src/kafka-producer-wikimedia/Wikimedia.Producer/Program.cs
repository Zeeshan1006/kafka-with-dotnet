// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Wikimedia.Producer.Service;
using Wikimedia.Producer.Service.Configuration;

var services = new ServiceCollection()
    .AddLogging(logging => logging.AddConsole());

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSetting.json", optional: false, reloadOnChange: true);

IConfiguration configuartion = builder.Build();
services.AddSingleton(configuartion);

services.AddWikimediaProducerService(configuartion);

var serviceProvider = services.BuildServiceProvider();

var producerService = serviceProvider.GetRequiredService<ISendMessage>();

string url = configuartion.GetSection("SOURCE_URL").Value;

try
{
    using var httpClient = new HttpClient();
    using (var stream = await httpClient.GetStreamAsync(url))
    {
        using (var reader = new StreamReader(stream))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (!line.StartsWith("data:"))
                {
                    continue;
                }

                int openBraceIndex = line.IndexOf('{');
                string data = line[openBraceIndex..];
                Console.WriteLine($"Data string: { data }");

                var doc = JsonDocument.Parse(data);
                var metaEelement = doc.RootElement.GetProperty("meta");
                var uriElement = metaEelement.GetProperty("uri");
                var key = uriElement.GetString();

                await producerService.SendMessageRequest(key, data);
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}