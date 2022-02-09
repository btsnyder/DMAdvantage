namespace TestEngineering
{
    public static class EnumerableExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            var enumerable = source.ToArray();
            var index = Faker.RandomNumber.Next(0, enumerable.Length - 1);
            return enumerable[index];
        }
    }
}
