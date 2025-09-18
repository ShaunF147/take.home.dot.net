using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using NUnit.Framework;

namespace ClearBank.DeveloperTest
{
    [TestFixture]
    public class PaymentServiceTests
    {
        [TestFixture]
        public class ValidPaymentRequests
        {
            [TestFixture]
            public class Bacs
            {
                public class GivenAValidRequest
                {
                    private MakePaymentRequest makePaymentRequest;
                    private MakePaymentResult result;
                    private Mock<IAccountDataStore> dataStore;
                    private Account account;

                    [SetUp]
                    public void WhenMakingASuccessfulPayment()
                    {
                        dataStore = new Mock<IAccountDataStore>();
                        account = FixtureGenerator.Account();
                        dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(
                            account
                        );

                        var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                        makePaymentRequest = new MakePaymentRequest() { PaymentScheme = PaymentScheme.Bacs, Amount = 100m, DebtorAccountNumber = "98765432" };

                        result = subject.MakePayment(makePaymentRequest);
                    }

                    [Test]
                    public void ThenTheResultIsSuccessful()
                    {
                        Assert.That(result.Success, Is.True);
                    }

                    [Test]
                    public void ThenTheAccountDataStoreIsUpdated()
                    {
                        dataStore.Verify(x => x.UpdateAccount(account));
                        dataStore.Verify(x => x.UpdateAccount(It.Is<Account>(a => a.Balance == 900m)));
                    }
                }

                public class GivenAnInvalidRequest
                {
                    [Test]
                    public void WhenMakingAPaymentThenTheAccountIsNotUpdated()
                    {
                        var dataStore = new Mock<IAccountDataStore>();
                        dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(
                            (Account)null
                        );
                        var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                        var result = subject.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 100m, DebtorAccountNumber = "98765432" });

                        dataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
                    }
                }
            }

            [TestFixture]
            public class FasterPayments
            {
                public class GivenAValidRequest
                {
                    private MakePaymentResult result;
                    private Mock<IAccountDataStore> dataStore;
                    private Account account;

                    [SetUp]
                    public void WhenMakingASuccessfulPayment()
                    {
                        dataStore = new Mock<IAccountDataStore>();
                        account = FixtureGenerator.Account();
                        dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);

                        var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                        result = subject.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 100m, DebtorAccountNumber = "98765432" });
                    }

                    [Test]
                    public void ThenTheResultIsSuccessful()
                    {
                        Assert.That(result.Success, Is.True);
                    }

                    [Test]
                    public void ThenTheAccountDataStoreIsUpdated()
                    {
                        dataStore.Verify(x => x.UpdateAccount(account));
                        dataStore.Verify(x => x.UpdateAccount(It.Is<Account>(a => a.Balance == 900m)));
                    }
                }

                public class GivenAnInvalidRequest
                {
                    [Test]
                    public void WhenMakingAPaymentThenTheAccountIsNotUpdated()
                    {
                        var dataStore = new Mock<IAccountDataStore>();
                        dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(
                            (Account)null
                        );
                        var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                        var result = subject.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 100m, DebtorAccountNumber = "98765432" });

                        dataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
                    }
                }

            }

            [TestFixture]
            public class Chaps
            {
                [TestFixture]
                public class GivenAValidRequest
                {
                    private Mock<IAccountDataStore> dataStore;
                    private Account account;
                    private MakePaymentResult result;

                    [SetUp]
                    public void WhenMakingASuccessfulPayment()
                    {
                        dataStore = new Mock<IAccountDataStore>();
                        account = FixtureGenerator.Account();
                        dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(
                            account
                        );
                        var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                        result = subject.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps, Amount = 100m, DebtorAccountNumber = "98765432" });

                    }

                    [Test]
                    public void ThenTheResultIsSuccessful()
                    {
                        Assert.That(result.Success, Is.True);
                    }

                    [Test]
                    public void ThenTheAccountDataStoreIsUpdated()
                    {
                        dataStore.Verify(x => x.UpdateAccount(account));
                        dataStore.Verify(x => x.UpdateAccount(It.Is<Account>(a => a.Balance == 900m)));
                    }
                }

                public class GivenAnInvalidRequest
                {
                    [Test]
                    public void WhenMakingAPaymentThenTheAccountIsNotUpdated()
                    {
                        var dataStore = new Mock<IAccountDataStore>();
                        dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(
                            (Account)null
                        );
                        var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                        var result = subject.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps, Amount = 100m, DebtorAccountNumber = "98765432" });

                        dataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
                    }
                }
            }
        }

        [TestFixture]
        public class InvalidPaymentRequests
        {
            [Test]
            public void GivenNoAccountIsFoundThenTheResultIsUnsuccessful()
            {
                var dataStore = new Mock<IAccountDataStore>();
                var account = FixtureGenerator.Account();
                dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(
                       (Account)null
                   );

                var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                var makePaymentRequest = new MakePaymentRequest() { PaymentScheme = PaymentScheme.Bacs, Amount = 100m, DebtorAccountNumber = "98765432" };

                var result = subject.MakePayment(makePaymentRequest);

                Assert.That(result.Success, Is.False);
            }

            [Test]
            public void GivenAnUnhandledExceptionIsThrownThenTheResultIsUnsuccessful()
            {
                var dataStore = new Mock<IAccountDataStore>();
                var account = FixtureGenerator.Account();
                dataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Throws(new Exception());

                var subject = new PaymentService(dataStore.Object, new PaymentRequestValidator());

                var makePaymentRequest = new MakePaymentRequest() { PaymentScheme = PaymentScheme.Bacs, Amount = 100m, DebtorAccountNumber = "98765432" };

                var result = subject.MakePayment(makePaymentRequest);

                Assert.That(result.Success, Is.False);
            }
        }
    }
}
