namespace DMAdvantage.Client.Services
{
    public interface IApiService
    {
        Task AddEntity<T>(T model);
        Task<List<T>?> GetAllEntities<T>();
        Task<T?> GetEntityById<T>(Guid id);
        Task UpdateEntity<T>(Guid id, T model);
        Task RemoveEntity<T>(Guid id);
    }
}
