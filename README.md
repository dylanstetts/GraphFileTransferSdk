
# GraphFileTransferSdk

**GraphFileTransferSdk** is a simple .NET SDK that demonstrates how to upload and download files using the Microsoft Graph API. It supports file transfers to and from a user's OneDrive or SharePoint drive using delegated or app-only permissions.

## Features

- Upload files from a local directory to a user's OneDrive
- Download files from a user's OneDrive to a local directory
- Resolve user object ID from UPN
- Uses `Microsoft.Graph` SDK and `Azure.Identity` for authentication
- Configurable via `appsettings.json`
- Logging via Serilog

## Project Structure

```
GraphFileTransferSdk/
│
├── Models/
│   └── FileMetadata.cs         # Represents metadata for uploaded/downloaded files
│
├── Services/
│   ├── FileTransferService.cs  # Core logic for file upload/download
│   └── GraphClientFactory.cs   # Creates authenticated GraphServiceClient
│
├── Program.cs                  # Entry point for running the file transfer
└── appsettings.json            # Configuration file (not included in repo)
```

## Configuration

Create an `appsettings.json` file in the root directory with the following structure:

```json
{
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  },
  "Graph": {
    "TargetUser": "user@domain.com"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" }
    ]
  }
}
```

## Usage

1. Place the file you want to upload in the `files/` directory.
2. Update `Program.cs` with the correct file name.
3. Run the project:

```bash
dotnet run
```

The program will:
- Upload the file to the target user's OneDrive root directory.
- Download the same file and save it locally with a `downloaded_` prefix.

## FileMetadata

The `FileMetadata` class contains:

```csharp
public class FileMetadata
{
    public string Name { get; set; }
    public string Id { get; set; }
    public long Size { get; set; }
    public string DownloadedAs { get; set; }
}
```

## Dependencies

- [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph)
- [Azure.Identity](https://www.nuget.org/packages/Azure.Identity)
- [Serilog](https://www.nuget.org/packages/Serilog)

## Authentication

This SDK uses **client credentials flow** via `ClientSecretCredential`. Ensure the app registration has the necessary Graph API permissions (e.g., `Files.ReadWrite.All`, `User.Read.All`) and admin consent is granted.

