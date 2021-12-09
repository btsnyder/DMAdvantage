using DMAdvantage.Shared.Entities;
using FluentAssertions;
using System.Collections;
using TestEngineering.Mocks;

namespace TestEngineering
{
    public static class Validation
    {
        public static void CompareData<T1, T2>(T1 expected, T2 actual)
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
                if (expectedValue is IList)
                    actualValue.Should().BeEquivalentTo(expectedValue, $"{prop.Name} is equal");
                else
                    actualValue.Should().Be(expectedValue, $"{prop.Name} is equal");
            }
            if (expected is BaseEntity expectedEntity && expectedEntity.User != null)
                expectedEntity.User.UserName.Should().Be(MockHttpContext.CurrentUser.UserName);
            if (actual is BaseEntity actualEntity && actualEntity.User != null)
                actualEntity.User.UserName.Should().Be(MockHttpContext.CurrentUser.UserName);
        }
    }
}
