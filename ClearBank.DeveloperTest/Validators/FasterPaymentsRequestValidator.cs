using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class FasterPaymentsRequestValidator : IPaymentValidator
    {
        public MakePaymentResult ValidateRequest(Account account, MakePaymentRequest request)
        {
            var result = new MakePaymentResult() { Success = true };

            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
            {
                result.Success = false;
            }
            else if (account.Balance < request.Amount)
            {
                result.Success = false;
            }

            return result;
        }
    }
}
