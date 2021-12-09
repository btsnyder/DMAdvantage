using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Data
{
    public interface IRepository
    {
        public T? GetEntityById<T>(Guid id, string username) where T : BaseEntity;
        public IEnumerable<T> GetAllEntities<T>(string username) where T : BaseEntity;

        void AddEntity(object entity);
        void RemoveEntity(object entity);
        bool SaveAll();
    }
}
    