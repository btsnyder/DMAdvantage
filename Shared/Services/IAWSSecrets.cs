namespace DMAdvantage.Shared.Services
{
    public interface IAWSSecrets
    {
        public string GetSecret(string secretName);
    }
}
