using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest
{
    public class StubAccountDataStore : IAccountDataStore
    {
        public Account GetAccount(string accountNumber)
        {
            return FixtureGenerator.Account();
        }

        public void UpdateAccount(Account account)
        {
            // no op.
        }
    }
}
