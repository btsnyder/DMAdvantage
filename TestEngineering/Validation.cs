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
        public static void CompareEntities<T>(T? expected, T? actual) where T : BaseEntity
        {
            CompareData(expected, actual);
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
                if (expectedValue is User && actualValue == null)
                    continue;
                else if (expectedValue is ICollection)
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
                    var parsedResult = (T?)createdResult.Subject.Value;
                    testData.Expected ??= testData.Entity;
                    parsedResult.Should().NotBeNull();
                    testData.RepositoryEntities.Should().Contain(x => x.Id == parsedResult!.Id);
                    CompareData(testData.Expected, parsedResult);
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
            if (result.Value is IList<T> list)
            {
                testData.ExpectedList ??= testData.RepositoryEntities;
                testData.ExpectedList.Should().HaveCount(list.Count);
                for (var i = 0; i < list.Count; i++)
                {
                    list[i].User = testData.ExpectedList[i].User;
                    CompareData(testData.ExpectedList[i], list[i]);
                }
            }
            else
            {
                testData.Expected.Should().NotBeNull();
                var parsedAssumption = result.Value.Should().BeOfType<T>();
                parsedAssumption.Subject.User = testData.Expected!.User;
                CompareData(testData.Expected, parsedAssumption.Subject);
            }
        }
    }
}
