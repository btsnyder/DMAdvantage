using DMAdvantage.Shared.Entities;

namespace TestEngineering
{
    public class ControllerUnitTestData<TEntity> where TEntity : BaseEntity
    {
        public ControllerUnitTestData(List<TEntity> entities)
        {
            RepositoryEntities = entities;
            Entity = entities.PickRandom();
        }

        public TEntity? Expected { get; set; }
        public List<TEntity>? ExpectedList { get; set; }
        public TEntity Entity { get; set; }
        public List<TEntity> RepositoryEntities { get; set; }
    }
}
