using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace DMAdvantage.Data
{
    public class Seeder
    {
        private readonly DMContext _ctx;
        private readonly UserManager<User> _userManager;

        public Seeder(DMContext ctx, 
            UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await _ctx.Database.EnsureCreatedAsync();

            var user = await _userManager.FindByEmailAsync("brandon@email.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Brandon",
                    LastName = "Snyder",
                    Email = "brandon@email.com",
                    UserName = "brandon@email.com"
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create new user in seeder"); 
                }
            }

            await _ctx.SaveChangesAsync();
        }
    }
}
