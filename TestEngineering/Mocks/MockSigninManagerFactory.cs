using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace TestEngineering.Mocks
{
    public static class MockSigninManagerFactory
    {
        public static readonly string CurrentPassword = "P@ssw0rd";
   
        public static SignInManager<User> Create()
        {
            Mock<IHttpContextAccessor> contextAccessor = new();
            Mock<IUserClaimsPrincipalFactory<User>> claimsFactory = new();
            Mock<IOptions<IdentityOptions>> optionsAccessor = new();
            Mock<IAuthenticationSchemeProvider> schemes = new();
            Mock<IUserConfirmation<User>> confirmation = new();

            var manager = new Mock<SignInManager<User>>(MockUserManagerFactory.Create(), contextAccessor.Object, claimsFactory.Object,
                optionsAccessor.Object, new MockLogger<SignInManager<User>>(), schemes.Object, confirmation.Object);

            manager.Setup(x => x.CheckPasswordSignInAsync(MockHttpContext.CurrentUser, CurrentPassword, false))
                .Returns(Task.FromResult(SignInResult.Success));

            return manager.Object;
        }
    }
}
