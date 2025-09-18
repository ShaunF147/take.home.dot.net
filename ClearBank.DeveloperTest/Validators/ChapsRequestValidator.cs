using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class ChapsRequestValidator : IPaymentValidator
    {
        public MakePaymentResult ValidateRequest(Account account, MakePaymentRequest request)
        {
            var result = new MakePaymentResult() { Success = true };

            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
            {
                result.Success = false;
            }
            else if (account.Status != AccountStatus.Live)
            {
                result.Success = false;
            }

            return result;
        }
    }
}
