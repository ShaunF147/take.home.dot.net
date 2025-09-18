using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest
{
    public static class FixtureGenerator
    {
        public static Account Account()
        {
            return new AccountBuilder().Build();
        }
    }
}
