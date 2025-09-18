using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest
{
    public class AccountBuilder
    {
        private AllowedPaymentSchemes schemes = AllowedPaymentSchemes.Bacs
                                              | AllowedPaymentSchemes.FasterPayments
                                              | AllowedPaymentSchemes.Chaps;
        private decimal balance = 1000m;
        private string accountNumber = "987654321";
        private AccountStatus status = AccountStatus.Live;

        public AccountBuilder WithBalance(decimal balance)
        {
            this.balance = balance;
            return this;
        }

        public AccountBuilder WithStatus(AccountStatus status)
        {
            this.status = status;
            return this;
        }

        public AccountBuilder WithPaymentSchemes(AllowedPaymentSchemes schemes)
        {
            this.schemes = schemes;
            return this;
        }

        public Account Build() =>
            new()
            {
                AllowedPaymentSchemes = this.schemes,
                Balance = this.balance,
                AccountNumber = this.accountNumber,
                Status = this.status
            };
    }
}
