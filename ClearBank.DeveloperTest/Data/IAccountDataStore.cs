using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public interface IAccountDataStore
    {
        /// <summary>
        /// Get an account by account number. Returns `null` if no match is found.
        /// </summary>
        Account GetAccount(string accountNumber);
        void UpdateAccount(Account account);
    }
}
