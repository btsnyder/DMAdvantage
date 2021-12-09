using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Text.Json;

namespace DMAdvantage.Data
{
    public class Context: IdentityDbContext<User>
    {
        private readonly IConfiguration? _configuration;

        public Context(DbContextOptions<Context> options, IConfiguration? configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Creature> Creatures => Set<Creature>();
        public DbSet<Encounter> Encounters => Set<Encounter>();
        public DbSet<ForcePower> ForcePowers => Set<ForcePower>();
        public DbSet<TechPower> TechPowers => Set<TechPower>();
        public DbSet<DamageType> DamageTypes => Set<DamageType>();

        protected override void OnConfiguring(DbContextOptionsBuilder bldr)
        {
            base.OnConfiguring(bldr);

            if (!bldr.IsConfigured)
                bldr.UseSqlServer(_configuration.GetConnectionString("DbConnectionString"),
                     b => b.MigrationsAssembly("DMAdvantage.Server"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseAction>();

            AddGuidList(modelBuilder, (Encounter e) => e.CharacterIds);
            AddGuidList(modelBuilder, (Encounter e) => e.CreatureIds);
            AddGuidList(modelBuilder, (Character c) => c.ForcePowerIds);
            AddGuidList(modelBuilder, (Character c) => c.TechPowerIds);
            AddGuidList(modelBuilder, (Creature c) => c.ForcePowerIds);
            AddGuidList(modelBuilder, (Creature c) => c.TechPowerIds);
            AddGuidList(modelBuilder, (Creature c) => c.VulnerabilityIds);
            AddGuidList(modelBuilder, (Creature c) => c.ImmunityIds);
            AddGuidList(modelBuilder, (Creature c) => c.ResistanceIds);
        }

        private static void AddGuidList<T>(ModelBuilder modelBuilder, Expression<Func<T, List<Guid>>> expression) where T : BaseEntity
        {
            modelBuilder.Entity<T>()
              .Property(expression)
              .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>(),
                 new ValueComparer<List<Guid>>(
                    (c1, c2) => c1 == null ? c2 == null : c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        }
    }
}
