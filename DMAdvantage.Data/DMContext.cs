using DMAdvantage.Shared.Entities;
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
    public class DMContext : IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;

        public DMContext(DbContextOptions<DMContext> options, IConfiguration configuration) : base(options)
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
        public DbSet<Weapon> Weapons => Set<Weapon>();
        public DbSet<WeaponProperty> WeaponProperties => Set<WeaponProperty>();
        public DbSet<BaseAction> Actions => Set<BaseAction>();
        public DbSet<InitativeData> InitativeData => Set<InitativeData>();
        public DbSet<EnemyShip> EnemyShips => Set<EnemyShip>();
        public DbSet<ShipWeapon> ShipWeapons => Set<ShipWeapon>();
        public DbSet<ShipWeaponProperty> ShipWeaponProperties => Set<ShipWeaponProperty>();
        public DbSet<PlayerShip> PlayerShips => Set<PlayerShip>();
        public DbSet<ShipEncounter> ShipEncounters => Set<ShipEncounter>();
        public DbSet<ShipInitativeData> ShipInitativeData => Set<ShipInitativeData>();
        public DbSet<Equipment> Equipments => Set<Equipment>();

        public bool SaveAll()
        {
            return SaveChanges() > 0;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder bldr)
        {
            base.OnConfiguring(bldr);



            if (!bldr.IsConfigured)
            {
                var dbKey = _configuration["ConnectionVariable:Key"];
                var connectionString = Environment.GetEnvironmentVariable(dbKey) ?? string.Empty;
                bldr.UseSqlServer(connectionString,
                        b =>
                        {
                            b.MigrationsAssembly("DMAdvantage.Server");
                            b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            AddPropertyList(modelBuilder, (Creature c) => c.Vulnerabilities);
            AddPropertyList(modelBuilder, (Creature c) => c.Immunities);
            AddPropertyList(modelBuilder, (Creature c) => c.Resistances);

            modelBuilder.Entity<InitativeEquipmentQuantity>()
                .HasKey(t => new { t.InitativeDataId, t.EquipmentId });

            modelBuilder.Entity<InitativeEquipmentQuantity>()
                .HasOne(pt => pt.InitativeData)
                .WithMany(p => p.EquipmentQuantities)
                .HasForeignKey(pt => pt.InitativeDataId);

            modelBuilder.Entity<InitativeEquipmentQuantity>()
                .HasOne(pt => pt.Equipment)
                .WithMany(t => t.InitativeData)
                .HasForeignKey(pt => pt.EquipmentId);
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

        public IQueryable<T> GetQueryable<T>(bool include = true) where T : BaseEntity
        {
            if (!include)
                return Set<T>();
            return typeof(T).Name switch
            {
                nameof(Character) => (IQueryable<T>)Characters
                    .Include(c => c.Abilities)
                    .Include(c => c.Class)
                    .Include(c => c.ForcePowers)
                    .Include(c => c.TechPowers)
                    .Include(c => c.Weapons).ThenInclude(w => w.Properties)
                    .Include(c => c.Equipments),
                nameof(Creature) => (IQueryable<T>)Creatures
                    .Include(c => c.ForcePowers)
                    .Include(c => c.TechPowers)
                    .Include(c => c.Actions),
                nameof(Encounter) => (IQueryable<T>)Encounters
                    .Include(e => e.InitativeData).ThenInclude(i => i.Character)
                    .Include(e => e.InitativeData).ThenInclude(i => i.Creature)
                    .Include(e => e.InitativeData).ThenInclude(i => i.EquipmentQuantities).ThenInclude(e => e.Equipment),
                nameof(Weapon) => (IQueryable<T>)Weapons
                    .Include(w => w.Properties),
                nameof(EnemyShip) => (IQueryable<T>)EnemyShips
                    .Include(w => w.Weapons).ThenInclude(w => w.Properties),
                nameof(ShipWeapon) => (IQueryable<T>)ShipWeapons
                    .Include(w => w.Properties),
                nameof(PlayerShip) => (IQueryable<T>)PlayerShips
                    .Include(w => w.Weapons).ThenInclude(w => w.Properties),
                _ => Set<T>()
            };
        }
    }
}
