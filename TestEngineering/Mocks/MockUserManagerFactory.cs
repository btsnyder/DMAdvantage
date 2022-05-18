using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace TestEngineering.Mocks
{
    public static class MockUserManagerFactory
    {
        public static UserManager<User> Create()
        {
            Mock<IUserStore<User>> store = new();
            Mock<IOptions<IdentityOptions>> optionsAccessor = new();
            Mock<IPasswordHasher<User>> passwordHasher = new();
            Mock<IUserValidator<User>> userValidator = new();
            Mock<IPasswordValidator<User>> passwordValidator = new();
            Mock<ILookupNormalizer> keyNormalizer = new();
            Mock<IdentityErrorDescriber> errors = new();
            Mock<IServiceProvider> services = new();
            List<IUserValidator<User>> _userValidators = new() { userValidator.Object };
            List<IPasswordValidator<User>> _passwordValidators = new() { passwordValidator.Object };

            var manager = new Mock<UserManager<User>>(store.Object, optionsAccessor.Object, passwordHasher.Object,
                _userValidators, _passwordValidators, keyNormalizer.Object, errors.Object, services.Object,
                new MockLogger<UserManager<User>>());

            manager.Setup(x => x.FindByNameAsync(MockHttpContext.CurrentUser))
                .Returns(Task.FromResult(new User { UserName = MockHttpContext.CurrentUser, Email = MockHttpContext.CurrentUser }));

            return manager.Object;
        }
    }
}
