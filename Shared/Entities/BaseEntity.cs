using System.ComponentModel.DataAnnotations;

namespace DMAdvantage.Shared.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsFound(string search)
        {
            if (Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
        public override bool Equals(object o)
        {
            var other = o as BaseEntity;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
