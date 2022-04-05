namespace DMAdvantage.Shared.Services
{
    public interface IKeyVaultManager
    {
        public Task<string> GetSecret(string secretName);
    }
}
