using ClearBank.DeveloperTest.Services;
using NUnit.Framework;
using ClearBank.DeveloperTest;

namespace ClearBank.DeveloperTest.Tests.Validators;

[TestFixture]
public class FasterPaymentsPaymentRequestValidatorTest
{
    public class GivenAValidRequest
    {
        [Test]
        public void WhenValidatingThenTheResultIsSuccessful()
        {
            var subject = new FasterPaymentsRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder().WithPaymentSchemes(Types.AllowedPaymentSchemes.FasterPayments).Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321" });

            Assert.That(result.Success, Is.True);
        }
    }

    public class GivenAnInvalidRequest
    {
        [Test]
        public void WhenValidatingWithAnInvalidPaymentSchemeThenTheResultIsUnsuccessful()
        {
            var subject = new FasterPaymentsRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder()
                .WithPaymentSchemes(Types.AllowedPaymentSchemes.Chaps)
                .Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321" });

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void WhenValidatingWithPaymentAmountThatIsLessThanTheAccountBalanceThenTheResultIsUnsuccessful()
        {
            var subject = new FasterPaymentsRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder()
                .WithPaymentSchemes(Types.AllowedPaymentSchemes.FasterPayments)
                .WithBalance(0m)
                .Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321", Amount = 100m });

            Assert.That(result.Success, Is.False);
        }
    }
}
