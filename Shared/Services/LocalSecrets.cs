namespace DMAdvantage.Shared.Services
{
    public class LocalSecrets : IAWSSecrets
    {
        public string GetSecret(string secretName)
        {
            var secret = Environment.GetEnvironmentVariable(secretName);
            if (secret == null)
                throw new Exception("Secret not found!");
            return secret;
        }
    }
}
