using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Text.Json;
using DMAdvantage.Shared.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        public DbSet<Ability> Abilities => Set<Ability>();
        public DbSet<DMClass> DMClasses => Set<DMClass>();


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

            var converter = new EnumToStringConverter<DamageType>();

            modelBuilder.Entity<Creature>()
                .Property(e => e.Vulnerabilities)
                .HasConversion(
                    v => string.Join(",", v.Select(e => e.ToString("D")).ToArray()),
                    v => v.Split(new[] { ',' })
                        .Select(e => Enum.Parse(typeof(DamageType), e))
                        .Cast<DamageType>()
                        .ToList()
            );

            modelBuilder.Ignore<BaseAction>();
            modelBuilder.Ignore<InitativeData>();
            modelBuilder.Ignore<Weapon>();

            AddPropertyList(modelBuilder, (Creature c) => c.Vulnerabilities);
            AddPropertyList(modelBuilder, (Creature c) => c.Immunities);
            AddPropertyList(modelBuilder, (Creature c) => c.Resistances);
        }

        private static void AddPropertyList<T1, T2>(ModelBuilder modelBuilder, Expression<Func<T1, List<T2>>> expression) where T1 : BaseEntity
        {
            modelBuilder.Entity<T1>()
              .Property(expression)
              .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<T2>>(v, (JsonSerializerOptions?)null) ?? new List<T2>(),
                 new ValueComparer<List<T2>>(
                    (c1, c2) => c1 == null ? c2 == null : c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
                    c => c.ToList()));
        }
    }
}
