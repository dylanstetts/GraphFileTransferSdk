using GraphFileTransferSdk.Services;
using Microsoft.Extensions.Configuration;
using Serilog;

class Program
{
    static async Task Main(string[] args)
    {
        // Load configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            // Create Graph client and file transfer service
            var graphClient = GraphClientFactory.Create(config);
            var fileService = new FileTransferService(graphClient, config);

            var fileName = "example.txt"; // File to upload from "files" folder
            var metadata = await fileService.UploadFileAsync(fileName);

            // If upload succeeded, download the file with a new name
            if (metadata?.Id != null)
            {
                var downloadedName = $"downloaded_{metadata.Name}";
                await fileService.DownloadFileAsync(metadata.Id, downloadedName);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during file transfer.");
        }
    }
}
