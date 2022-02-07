using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace DMAdvantage.Data
{
    public class Repository : IRepository
    {
        private readonly Context _ctx;

        public Repository(Context ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<T> GetAllEntities<T>(string username, ISearchParameters<T>? searching = null) where T : BaseEntity
        {
            var query = GetFromDatabase<T>(username);
            if (searching != null)
                query = searching.AddToQuery(query);
            return query.ToList()
                .OrderBy(c => c.OrderBy())
                .ToArray();
        }

        public PagedList<T> GetAllEntities<T>(string username, PagingParameters paging, ISearchParameters<T>? searching = null) where T : BaseEntity
        {
            var query = GetFromDatabase<T>(username);
            if (searching != null)
                query = searching.AddToQuery(query);

            var data = query.ToList().OrderBy(c => c.OrderBy());
            return PagedList<T>.ToPagedList(data.ToList(), paging);
        }

        public T? GetEntityById<T>(Guid id, string username) where T : BaseEntity
        {
            if (id == Guid.Empty)
                return null;
            return GetFromDatabase<T>(username).FirstOrDefault(c => c.Id == id);
        }

        public T? GetEntityByIdWithoutUser<T>(Guid id) where T : BaseEntity
        {
            return id == Guid.Empty ? null : GetFromDatabase<T>().FirstOrDefault(c => c.Id == id);
        }

        public Character? GetCharacterByPlayerNameWithoutUser(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return _ctx.Characters.FirstOrDefault(c => c.PlayerName == name);
        }

        public IEnumerable<T> GetEntitiesByIdsWithoutUser<T>(Guid[] ids) where T : BaseEntity
        {
            if (ids.Any())
                return GetFromDatabase<T>().Where(c => ids.Contains(c.Id));
            else
                return GetFromDatabase<T>();
        }

        public void AddEntity(object entity)
        {
            _ctx.Add(entity);
        }

        public void RemoveEntity(object entity)
        {
            _ctx.Remove(entity);
        }

        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }

        private IQueryable<T> GetFromDatabase<T>(string? username = null) where T : BaseEntity
        {
            try
            {
                DbSet<T> dbSet = _ctx.Set<T>();
                return username == null ? dbSet : dbSet.Where(c => c.User != null && c.User.UserName == username);
            }
            catch (Exception ex)
            {
                // Invalid type was provided (i.e. table does not exist in database)
                throw new ArgumentException("Invalid Entity", ex);
            }
        }
    }
}
