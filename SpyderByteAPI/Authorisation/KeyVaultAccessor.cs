using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using SpyderByteAPI.Authorisation.Abstract;

namespace SpyderByteAPI.Authorisation
{
    public class KeyVaultAccessor : ISecretAccessor
    {
        public string ApiKey { get; }

        public KeyVaultAccessor()
        {
            var client = new SecretClient(new Uri("https://spyderbyteglobalkeyvault.vault.azure.net/"), new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret("SBAPIKEY");
            ApiKey = secret.Value;
        }
    }
}
