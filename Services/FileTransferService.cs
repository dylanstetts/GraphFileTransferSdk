using GraphFileTransferSdk.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Serilog;

namespace GraphFileTransferSdk.Services
{
    public class FileTransferService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _targetUser;

        public FileTransferService(GraphServiceClient graphClient, IConfiguration config)
        {
            _graphClient = graphClient;
            _targetUser = config["Graph:TargetUser"]; // Load target user from config
            Log.Information($"TargetUser from config: {_targetUser}");
        }

        // Uploads a file from the local "files" folder to the target user's OneDrive
        public async Task<FileMetadata> UploadFileAsync(string fileName)
        {
            var filePath = Path.Combine("files", fileName);

            // Check if the file exists locally
            if (!File.Exists(filePath))
            {
                Log.Error($"File not found: {filePath}");
                return null;
            }

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Resolve the target user's object ID
            var userId = await ResolveUserIdAsync(_targetUser);
            if (userId == null) return null;

            // Get the user's default drive
            var drive = await _graphClient.Users[userId].Drive.GetAsync();

            // Upload the file to the root directory of the drive
            var uploadedItem = await _graphClient.Drives[drive.Id]
                .Items["root"]
                .ItemWithPath(fileName)
                .Content
                .PutAsync(fileStream);

            Log.Information($"Uploaded file: {uploadedItem?.Name}");

            // Return metadata about the uploaded file
            return new FileMetadata
            {
                Name = uploadedItem?.Name,
                Id = uploadedItem?.Id,
                Size = uploadedItem?.Size ?? 0
            };
        }

        // Downloads a file from OneDrive using its item ID and saves it locally
        public async Task<bool> DownloadFileAsync(string itemId, string saveAs)
        {
            var userId = await ResolveUserIdAsync(_targetUser);
            if (userId == null) return false;

            var drive = await _graphClient.Users[userId].Drive.GetAsync();

            // Get the file stream from OneDrive
            var stream = await _graphClient.Drives[drive.Id]
                .Items[itemId]
                .Content
                .GetAsync();

            // Save the stream to a local file
            using var fileStream = new FileStream(saveAs, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);

            Log.Information($"Downloaded file as: {saveAs}");
            return true;
        }

        // Resolves a user's object ID from their UPN (email address)
        public async Task<string> ResolveUserIdAsync(string upn)
        {
            try
            {
                var user = await _graphClient.Users[upn]
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Select = new[] { "id" };
                    });

                if (user?.Id != null)
                {
                    Log.Information($"Resolved UPN '{upn}' to object ID '{user.Id}'");
                    return user.Id;
                }

                Log.Error($"Failed to resolve user ID for UPN: {upn}");
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error resolving user ID for UPN: {upn}");
                return null;
            }
        }
    }
}
