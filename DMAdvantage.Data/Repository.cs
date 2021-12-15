using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.Extensions.Logging;

namespace DMAdvantage.Data
{
    public class Repository : IRepository
    {
        private readonly Context _ctx;
        private readonly ILogger<Repository> _logger;

        public Repository(Context ctx, ILogger<Repository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        private IList<T> GetDbSet<T>(string username) where T : BaseEntity
        {
            return typeof(T).Name switch
            {
                nameof(Character) => (IList<T>)_ctx.Characters.Where(c => c.User != null && c.User.UserName == username).ToList(),
                nameof(Creature) => (IList<T>)_ctx.Creatures.Where(c => c.User != null && c.User.UserName == username).ToList(),
                nameof(Encounter) => (IList<T>)_ctx.Encounters.Where(c => c.User != null && c.User.UserName == username).ToList(),
                nameof(ForcePower) => (IList<T>)_ctx.ForcePowers.Where(c => c.User != null && c.User.UserName == username).ToList(),
                nameof(TechPower) => (IList<T>)_ctx.TechPowers.Where(c => c.User != null && c.User.UserName == username).ToList(),
                nameof(DamageType) => (IList<T>)_ctx.DamageTypes.Where(c => c.User != null && c.User.UserName == username).ToList(),
                _ => throw new NotImplementedException(),
            };
        }

        public IEnumerable<T> GetAllEntities<T>(string username) where T : BaseEntity
        {
            return GetDbSet<T>(username)
                    .OrderBy(c => c.OrderBy())
                    .ToArray();
        }

        public PagedList<T> GetAllEntities<T>(string username, PagingParameters paging) where T : BaseEntity
        {
            return PagedList<T>.ToPagedList(GetDbSet<T>(username).OrderBy(c => c.OrderBy()), paging);
        }

        public T? GetEntityById<T>(Guid id, string username) where T : BaseEntity
        {
            if (id == Guid.Empty)
                return null;
            return GetDbSet<T>(username).FirstOrDefault(c => c.Id == id);
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
    }
}
