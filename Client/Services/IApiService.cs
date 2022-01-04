using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;

namespace DMAdvantage.Client.Services
{
    public interface IApiService
    {
        Task AddEntity<T>(T model);
        Task<List<T>?> GetAllEntities<T>(ISearchQuery? searching = null);
        Task<PagedList<T>?> GetAllPagedEntities<T>(PagingParameters paging, ISearchQuery? searching = null, CancellationToken? token = null) where T : class;
        Task<T?> GetEntityById<T>(Guid id);
        Task UpdateEntity<T>(Guid id, T model);
        Task RemoveEntity<T>(Guid id);
        Task<EncounterResponse?> GetEncounterView(Guid id);
        Task<List<CharacterResponse>> GetCharacterViews(IEnumerable<Guid> ids);
        Task<List<CreatureResponse>> GetCreatureViews(IEnumerable<Guid> ids);
    }
}
