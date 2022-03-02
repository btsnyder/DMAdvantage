using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;

namespace DMAdvantage.Client.Services
{
    public interface IApiService
    {
        Task AddEntity<T>(T model);
        Task<List<T>?> GetAllEntities<T>(ISearchQuery? searching = null) where T : class;
        Task<PagedList<T>?> GetAllPagedEntities<T>(PagingParameters paging, ISearchQuery? searching = null, CancellationToken? token = null) where T : class;
        Task<T?> GetEntityById<T>(Guid id) where T : class;
        Task UpdateEntity<T>(Guid id, T model);
        Task RemoveEntity<T>(Guid id);
        Task<EncounterResponse?> GetEncounterView(Guid id);
        Task<List<T>?> GetViews<T>(IEnumerable<Guid>? ids = null);
        Task<CharacterResponse?> GetCharacterViewFromPlayerName(string name);
    }
}
