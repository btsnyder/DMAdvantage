using Microsoft.AspNetCore.Identity;

namespace DMAdvantage.Shared.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override bool Equals(object o)
        {
            var other = o as User;
            return other?.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Email;
    }
}
