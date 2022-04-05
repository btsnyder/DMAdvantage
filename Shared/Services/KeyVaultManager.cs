using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;

namespace DMAdvantage.Shared.Services
{
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient _secretClient;

        public KeyVaultManager(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task<string> GetSecret(string secretName)
        {
            try
            {
                KeyVaultSecret keyValueSecret = await
                _secretClient.GetSecretAsync(secretName);
                return keyValueSecret.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
