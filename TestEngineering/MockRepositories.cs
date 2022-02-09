using DMAdvantage.Data;
using DMAdvantage.Shared.Entities;
using FluentAssertions;
using Moq;
using TestEngineering.Mocks;

namespace TestEngineering
{
    public static class MockRepositories
    {
        public static IRepository GetAllEntities<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<T>(MockHttpContext.CurrentUser.UserName, null))
                .Returns(testData.RepositoryEntities);
            return mockRepo.Object;
        }

        public static IRepository GetEntityById<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<T>(testData.Entity.Id, It.IsAny<string>()))
                .Returns(testData.Entity);
            return mockRepo.Object;
        }

        public static IRepository CreateNewEntity<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object obj) => testData.RepositoryEntities.Add((T)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            return mockRepo.Object;
        }

        public static IRepository UpdateEntity<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<T>(testData.Entity.Id, It.IsAny<string>()))
                .Returns(testData.Entity);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            return mockRepo.Object;
        }

        public static IRepository Failure<T>() where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<T>(MockHttpContext.CurrentUser.UserName, null))
                .Throws(new Exception());
            mockRepo.Setup(x => x.GetEntityById<T>(It.IsAny<Guid>(), MockHttpContext.CurrentUser.UserName))
                .Throws(new Exception());
            mockRepo.Setup(x => x.SaveAll()).Returns(false);
            mockRepo.Setup(x => x.AddEntity(It.IsAny<object>()))
                .Callback((object _) => throw new Exception());

            return mockRepo.Object;
        }

        public static IRepository GetEmptyEntities<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllEntities<T>(It.IsAny<string>(), null))
                .Returns(testData.RepositoryEntities);
            mockRepo.Setup(x => x.GetAllEntities<T>(MockHttpContext.CurrentUser.UserName, null))
                .Returns(new List<T>());
            return mockRepo.Object;
        }

        public static IRepository DeleteEntity<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityById<T>(testData.Entity.Id, It.IsAny<string>()))
                .Returns(testData.Entity);
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            mockRepo.Setup(x => x.RemoveEntity(It.IsAny<object>()))
                .Callback((object obj) => testData.RepositoryEntities.Remove((T)obj));
            mockRepo.Setup(x => x.SaveAll()).Returns(true);
            return mockRepo.Object;
        }

        public static IRepository GetCharacterByPlayerName(ControllerUnitTestData<Character> characters)
        {
            characters.Entity.PlayerName.Should()
                .NotBeNullOrWhiteSpace(" we must have a player name to test searching");
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetCharacterByPlayerNameWithoutUser(characters.Entity.PlayerName!))
                .Returns(characters.Entity);
            return mockRepo.Object;
        }

        public static IRepository GetEntityByIdWithoutUser<T>(ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntityByIdWithoutUser<T>(testData.Entity.Id))
                .Returns(testData.Entity);
            return mockRepo.Object;
        }

        public static IRepository GetEntitiesByIdsWithoutUser<T>(ControllerUnitTestData<T> testData, Guid[] ids) where T : BaseEntity
        {
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetEntitiesByIdsWithoutUser<T>(ids))
                .Returns(testData.RepositoryEntities);
            return mockRepo.Object;
        }
    }
}
