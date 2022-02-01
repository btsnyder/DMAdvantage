using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;

namespace DMAdvantage.Data
{
    public interface IRepository
    { 
        public T? GetEntityById<T>(Guid id, string username) where T : BaseEntity;
        public T? GetEntityByIdWithoutUser<T>(Guid id) where T : BaseEntity;
        public Character? GetCharacterByPlayerNameWithoutUser(string name);
        public IEnumerable<T> GetEntitiesByIdsWithoutUser<T>(Guid[] ids) where T : BaseEntity;
        public IEnumerable<T> GetAllEntities<T>(string username, ISearchParameters<T>? searching = null) where T : BaseEntity;
        public PagedList<T> GetAllEntities<T>(string username, PagingParameters search, ISearchParameters<T>? searching) where T : BaseEntity;

        void AddEntity(object entity);
        void RemoveEntity(object entity);
        bool SaveAll();
    }
}
    