using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

namespace GraphFileTransferSdk.Services
{
    public static class GraphClientFactory
    {
        public static ClientSecretCredential Credential { get; private set; }

        // Creates a GraphServiceClient using Azure AD app credentials
        public static GraphServiceClient Create(IConfiguration config)
        {
            Credential = new ClientSecretCredential(
                config["AzureAd:TenantId"],     // Azure AD tenant ID
                config["AzureAd:ClientId"],     // App registration client ID
                config["AzureAd:ClientSecret"]  // App registration client secret
            );

            return new GraphServiceClient(Credential);
        }
    }
}
