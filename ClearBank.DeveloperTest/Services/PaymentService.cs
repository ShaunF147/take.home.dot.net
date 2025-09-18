using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore accountDataStore;
        private readonly IPaymentValidator paymentValidator;

        public PaymentService(IAccountDataStore accountDataStore, IPaymentValidator paymentValidator)
        {
            this.accountDataStore = accountDataStore;
            this.paymentValidator = paymentValidator;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            try
            {
                Account account = this.accountDataStore.GetAccount(request.DebtorAccountNumber);

                if (account == null)
                {
                    return new MakePaymentResult();
                }

                var result = this.paymentValidator.ValidateRequest(account, request);

                if (result.Success)
                {
                    account.Balance -= request.Amount;
                    this.accountDataStore.UpdateAccount(account);
                }

                return result;
            }
            catch (Exception)
            {
                // log exception here with more time.

                // we're catching all exceptions to ensure the client always gets
                // a valid response. with more time we'd be more specific on the
                // exceptions caught or alternatively use a Result pattern. see
                // notes for more.
                return new MakePaymentResult();
            }
        }
    }
}
