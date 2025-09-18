using ClearBank.DeveloperTest.Services;
using NUnit.Framework;
using ClearBank.DeveloperTest;

namespace ClearBank.DeveloperTest.Tests.Validators;

[TestFixture]
public class ChapsPaymentRequestValidatorTest
{
    public class GivenAValidRequest
    {
        [Test]
        public void WhenValidatingThenTheResultIsSuccessful()
        {
            var subject = new ChapsRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder().WithPaymentSchemes(Types.AllowedPaymentSchemes.Chaps).Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321" });

            Assert.That(result.Success, Is.True);
        }
    }

    public class GivenAnInvalidRequest
    {
        [Test]
        public void WhenValidatingWithAnInvalidPaymentSchemeThenTheResultIsUnsuccessful()
        {
            var subject = new ChapsRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder()
                .WithPaymentSchemes(Types.AllowedPaymentSchemes.FasterPayments)
                .Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321" });

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void WhenValidatingWithAnAccountThatIsNotLiveThenTheResultIsUnsuccessful()
        {
            var subject = new ChapsRequestValidator();

            var result = subject.ValidateRequest(
                new AccountBuilder()
                .WithPaymentSchemes(Types.AllowedPaymentSchemes.Chaps)
                .WithStatus(Types.AccountStatus.Disabled)
                .Build(),
                new Types.MakePaymentRequest() { DebtorAccountNumber = "987654321", Amount = 100m });

            Assert.That(result.Success, Is.False);
        }
    }
}
