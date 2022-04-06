namespace DMAdvantage.Shared.Services
{
    public class EmptyKeyVaultManager : IKeyVaultManager
    {
        public Task<string> GetSecret(string secretName) => Task.FromResult<string>(null);
    }
}
