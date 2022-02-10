using DMAdvantage.Shared.Entities;
using FluentAssertions;
using System.Collections;
using AutoMapper;
using DMAdvantage.Data;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using TestEngineering.Enums;
using TestEngineering.Mocks;

namespace TestEngineering
{
    public static class Validation
    {
        private static readonly Mapper _mapper;

        static Validation()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = new Mapper(config);
        }

        public static void CompareRequests<T>(T? expected, T? actual) where T : IEntityRequest
        {
            CompareData(expected, actual);
        }

        public static void CompareResponses<T>(T? expected, T? actual) where T : IEntityResponse
        {
            CompareData(expected, actual);
        }

        public static void CompareEntities<T>(T? expected, T? actual) where T : BaseEntity
        {
            CompareData(expected, actual);
            expected!.User?.UserName.Should().Be(MockHttpContext.CurrentUser.UserName);
            actual!.User?.UserName.Should().Be(MockHttpContext.CurrentUser.UserName);
        }


        private static void CompareData<T>(T? expected, T? actual)
        {
            if (expected == null)
                throw new ArgumentNullException(nameof(expected));
            if (actual == null)
                throw new ArgumentNullException(nameof(actual));

            var properties = expected.GetType().GetProperties().Where(x => x.CanWrite && x.CanRead);
            foreach (var prop in properties)
            {
                var expectedValue = prop.GetValue(expected);
                var actualProp = actual.GetType().GetProperty(prop.Name);
                if (actualProp == null)
                    continue;
                var actualValue = actualProp.GetValue(actual);
                if (expectedValue is ICollection)
                    actualValue.Should().BeEquivalentTo(expectedValue, $"{prop.Name} is equal");
                else
                    actualValue.Should().Be(expectedValue, $"{prop.Name} is equal");
            }
        }


        public static void ValidateResponse<T>(TestAction action, IActionResult result, ControllerUnitTestData<T> testData) where T: BaseEntity
        {
            switch (action)
            {
                case TestAction.Get:
                    var okResult = result.Should().BeOfType<OkObjectResult>();
                    testData.Expected ??= testData.Entity;
                    ValidateGetResponse(okResult.Subject, testData);
                    break;
                case TestAction.Create:
                    var createdResult = result.Should().BeOfType<CreatedResult>();
                    var mappedResult = _mapper.Map<T>(createdResult.Subject.Value);
                    testData.Expected ??= testData.Entity;
                    testData.RepositoryEntities.Should().Contain(x => x.Id == mappedResult.Id);
                    CompareData(testData.Expected, mappedResult);
                    break;
                case TestAction.Update:
                    result.Should().BeOfType<NoContentResult>();
                    testData.Expected.Should().NotBeNull();
                    testData.Entity.Should().NotBeNull();
                    testData.Expected!.Id = testData.Entity.Id;
                    testData.Expected!.User = testData.Entity.User;
                    CompareData(testData.Expected, testData.Entity);
                    break;
                case TestAction.Delete:
                    result.Should().BeOfType<NoContentResult>();
                    testData.RepositoryEntities.Should().NotContain(testData.Entity);
                    break;
                case TestAction.Missing:
                    result.Should().BeOfType<NotFoundResult>();
                    break;
                case TestAction.Error:
                    result.Should().BeOfType<BadRequestObjectResult>();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void ValidateGetResponse<T>(OkObjectResult result, ControllerUnitTestData<T> testData) where T : BaseEntity
        {
            if (result.Value is IList list)
            {
                testData.ExpectedList ??= testData.RepositoryEntities;
                testData.ExpectedList.Should().HaveCount(list.Count);
                for (var i = 0; i < list.Count; i++)
                {
                    var mappedResult = _mapper.Map<T>(list[i]);
                    mappedResult.User = testData.ExpectedList[i].User;
                    CompareData(testData.ExpectedList[i], mappedResult);
                }
            }
            else
            {
                testData.Expected.Should().NotBeNull();
                var mappedResult = _mapper.Map<T>(result.Value);
                mappedResult.User = testData.Expected!.User;
                CompareData(testData.Expected, mappedResult);
            }
        }
    }
}
