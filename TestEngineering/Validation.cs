using DMAdvantage.Shared.Entities;
using FluentAssertions;
using System.Collections;

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
    }
}
