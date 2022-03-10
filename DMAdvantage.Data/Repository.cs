using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace DMAdvantage.Data
{
    public class Repository : IRepository
    {
        public Context Context { get; }

        public Repository(Context context)
        {
            Context = context;
        }

        public IEnumerable<T> GetAllEntities<T>(string username, ISearchParameters<T>? searching = null) where T : BaseEntity
        {
            var query = GetFromDatabase<T>(username);
            return query.ToList()
                .OrderBy(c => c.OrderBy())
                .ToArray();
        }

        public PagedList<T> GetAllEntities<T>(string username, PagingParameters paging, ISearchParameters<T>? searching = null) where T : BaseEntity
        {
            var query = GetFromDatabase<T>(username);

            var data = query.ToList().OrderBy(c => c.OrderBy());
            return PagedList<T>.ToPagedList(data.ToList(), paging);
        }

        public T? GetEntityById<T>(Guid id, string username) where T : BaseEntity
        {
            if (id == Guid.Empty)
                return null;
            return GetFromDatabase<T>(username).FirstOrDefault(c => c.Id == id);
        }

        public T? GetEntityByIdV2<T>(Guid id, string username, bool include, bool tracking) where T : BaseEntity
        {
            if (id == Guid.Empty)
                return null;
            return GetFromDatabase<T>(username, include, tracking).FirstOrDefault(c => c.Id == id);
        }

        public T? GetEntityByIdWithoutUser<T>(Guid id) where T : BaseEntity
        {
            return id == Guid.Empty ? null : GetFromDatabaseWithoutUser<T>().FirstOrDefault(c => c.Id == id);
        }

        public Character? GetCharacterByPlayerNameWithoutUser(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return Context.Characters.FirstOrDefault(c => c.PlayerName == name);
        }

        public IEnumerable<T> GetEntitiesByIdsWithoutUser<T>(Guid[] ids) where T : BaseEntity
        {
            return ids.Any() ? GetFromDatabaseWithoutUser<T>().Where(c => ids.Contains(c.Id)) : GetFromDatabaseWithoutUser<T>();
        }

        public void AddEntity(object entity)
        {
            Context.Add(entity);
        }

        public void RemoveEntity(object entity)
        {
            Context.Remove(entity);
        }

        public bool SaveAll()
        {
            return Context.SaveChanges() > 0;
        }

        public void DetachAllEntities()
        {
            Context.ChangeTracker.Clear();
        }

        private IQueryable<T> GetFromDatabase<T>(string username, bool include = true, bool tracking = true) where T : BaseEntity
        {
            try
            {
                DbSet<T> dbSet = Context.Set<T>();
                if (dbSet is DbSet<Character> characters)
                {
                    if (tracking && include)
                    {
                        return (IQueryable<T>)characters.Include(x => x.Abilities).Where(c => c.User != null && c.User.UserName == username);
                    }
                    else if (include)
                    {
                        return (IQueryable<T>)characters.Include(x => x.Abilities).Where(c => c.User != null && c.User.UserName == username).AsNoTracking();
                    }
                    else if (tracking)
                    {
                        return (IQueryable<T>)characters.Where(c => c.User != null && c.User.UserName == username);
                    }
                    else
                    {
                        return (IQueryable<T>)characters.Where(c => c.User != null && c.User.UserName == username).AsNoTracking();
                    }
                }
                return tracking ?
                    dbSet.Where(c => c.User != null && c.User.UserName == username) :
                    dbSet.Where(c => c.User != null && c.User.UserName == username).AsNoTracking();
            }
            catch (Exception ex)
            {
                // Invalid type was provided (i.e. table does not exist in database)
                throw new ArgumentException("Invalid Entity", ex);
            }
        }

        private IQueryable<T> GetFromDatabaseWithoutUser<T>() where T : BaseEntity
        {
            try
            {
                DbSet<T> dbSet = Context.Set<T>();
                if (dbSet is DbSet<Character> characters)
                {
                    return (IQueryable<T>)characters.Include(x => x.Abilities).AsNoTracking();
                }
                return dbSet.AsNoTracking();
            }
            catch (Exception ex)
            {
                // Invalid type was provided (i.e. table does not exist in database)
                throw new ArgumentException("Invalid Entity", ex);
            }
        }
    }
}
