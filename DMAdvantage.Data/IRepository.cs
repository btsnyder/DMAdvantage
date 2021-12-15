using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;

namespace DMAdvantage.Data
{
    public interface IRepository
    {
        public T? GetEntityById<T>(Guid id, string username) where T : BaseEntity;
        public IEnumerable<T> GetAllEntities<T>(string username) where T : BaseEntity;
        public PagedList<T> GetAllEntities<T>(string username, PagingParameters search) where T : BaseEntity;

        void AddEntity(object entity);
        void RemoveEntity(object entity);
        bool SaveAll();
    }
}
    