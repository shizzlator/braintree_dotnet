using Braintree.Exceptions;
using Braintree.Test;
using Braintree.TestUtil;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Braintree.Tests.Integration
{
    [TestFixture]
    public class PaymentMethodNonceTest
    {
        private BraintreeGateway gateway;

        [SetUp]
        public void Setup()
        {
            gateway = new BraintreeGateway
            {
                Environment = Environment.DEVELOPMENT,
                MerchantId = "integration_merchant_id",
                PublicKey = "integration_public_key",
                PrivateKey = "integration_private_key"
            };
        }
        
        [Test]
        public void Create_CreatesPaymentMethodNonce()
        { 
            string nonce = TestHelper.GenerateUnlockedNonce(gateway);
            Result<Customer> customerResult = gateway.Customer.Create(new CustomerRequest());

            Result<PaymentMethod> paymentMethodResult = gateway.PaymentMethod.Create(new PaymentMethodRequest
            {
                CustomerId = customerResult.Target.Id,
                PaymentMethodNonce = nonce
            });

            Result<PaymentMethodNonce> result = gateway.PaymentMethodNonce.Create(paymentMethodResult.Target.Token);
            Assert.IsTrue(result.IsSuccess());
            Assert.IsNotNull(result.Target);
            Assert.IsNotNull(result.Target.Nonce);
        }
        
        [Test]
        public async Task CreateAsync_CreatesPaymentMethodNonce()
        {
            string nonce = TestHelper.GenerateUnlockedNonce(gateway);
            Result<Customer> customerResult = await gateway.Customer.CreateAsync(new CustomerRequest());

            Result<PaymentMethod> paymentMethodResult = gateway.PaymentMethod.Create(new PaymentMethodRequest
            {
                CustomerId = customerResult.Target.Id,
                PaymentMethodNonce = nonce
            });

            Result<PaymentMethodNonce> result = await gateway.PaymentMethodNonce.CreateAsync(paymentMethodResult.Target.Token);
            Assert.IsTrue(result.IsSuccess());
            Assert.IsNotNull(result.Target);
            Assert.IsNotNull(result.Target.Nonce);
        }

        [Test]
        public void Create_RaisesNotFoundErrorWhenTokenDoesntExist()
        {
            Assert.Throws<NotFoundException>(() => gateway.PaymentMethodNonce.Create("notarealtoken"));
        }

        [Test]
        public void Find_ExposesDetailsForCreditCardNonce()
        {
            string nonce = "fake-valid-nonce";
            PaymentMethodNonce foundNonce = gateway.PaymentMethodNonce.Find(nonce);
            Assert.IsNotNull(foundNonce);
            Assert.AreEqual(foundNonce.Nonce, nonce);
            Assert.AreEqual(foundNonce.Type, "CreditCard");
            Assert.IsNotNull(foundNonce.Details);
            Assert.AreEqual(foundNonce.Details.CardType, "Visa");
            Assert.AreEqual(foundNonce.Details.LastTwo, "81");
            Assert.AreEqual(foundNonce.Details.LastFour, "1881");
        }

        [Test]
        public void Find_ExposesThreeDSecureInfo()
        {
            BraintreeService service = new BraintreeService(gateway.Configuration);
            CreditCardRequest creditCardRequest = new CreditCardRequest
            {
                Number = SandboxValues.CreditCardNumber.VISA,
                ExpirationMonth = "05",
                ExpirationYear = "2020"
            };
            string nonce = TestHelper.Generate3DSNonce(service, creditCardRequest);

            PaymentMethodNonce foundNonce = gateway.PaymentMethodNonce.Find(nonce);
            ThreeDSecureInfo info = foundNonce.ThreeDSecureInfo;

            Assert.AreEqual(foundNonce.Nonce, nonce);
            Assert.AreEqual(foundNonce.Type, "CreditCard");
            Assert.AreEqual(info.Enrolled, "Y");
            Assert.AreEqual(info.Status, "authenticate_successful");
            Assert.AreEqual(info.LiabilityShifted, true);
            Assert.AreEqual(info.LiabilityShiftPossible, true);
        }

        [Test]
        public async Task FindAsync_ExposesBinData()
        {
            string inputNonce = Nonce.TransactableUnknownIndicators;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.Commercial, Braintree.CreditCardCommercial.UNKNOWN);
            Assert.AreEqual(nonce.BinData.Debit, Braintree.CreditCardDebit.UNKNOWN);
            Assert.AreEqual(nonce.BinData.DurbinRegulated, Braintree.CreditCardDurbinRegulated.UNKNOWN);
            Assert.AreEqual(nonce.BinData.Healthcare, Braintree.CreditCardHealthcare.UNKNOWN);
            Assert.AreEqual(nonce.BinData.Payroll, Braintree.CreditCardPayroll.UNKNOWN);
            Assert.AreEqual(nonce.BinData.Prepaid, Braintree.CreditCardPrepaid.UNKNOWN);
            Assert.AreEqual(nonce.BinData.CountryOfIssuance, "Unknown");
            Assert.AreEqual(nonce.BinData.IssuingBank, "Unknown");
            Assert.AreEqual(nonce.BinData.ProductId,"Unknown");
        }

        [Test]
        public async Task FindAsync_ExposesBinDataPrepaidValue()
        {
            string inputNonce = Nonce.TransactablePrepaid;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.Prepaid, Braintree.CreditCardPrepaid.YES);
        }

        [Test]
        public async Task FindAsync_ExposesBinDataCommercial()
        {
            string inputNonce = Nonce.TransactableCommercial;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.Commercial, Braintree.CreditCardCommercial.YES);
        }

        [Test]
        public async Task FindAsync_ExposesBinDataDebit()
        {
            string inputNonce = Nonce.TransactableDebit;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.Debit, Braintree.CreditCardDebit.YES);
        }

        [Test]
        public async Task FindAsync_ExposesBinDataDurbinRegulated()
        {
            string inputNonce = Nonce.TransactableDurbinRegulated;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.DurbinRegulated, Braintree.CreditCardDurbinRegulated.YES);
        }

        [Test]
        public async Task FindAsync_ExposesBinDataHealthcare()
        {
            string inputNonce = Nonce.TransactableHealthcare;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.Healthcare, Braintree.CreditCardHealthcare.YES);
            Assert.AreEqual(nonce.BinData.ProductId, "J3");
        }

        [Test]
        public async Task FindAsync_ExposesBinDataPayroll()
        {
            string inputNonce = Nonce.TransactablePayroll;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.Payroll, Braintree.CreditCardPayroll.YES);
            Assert.AreEqual(nonce.BinData.ProductId, "MSA");
        }

        [Test]
        public async Task FindAsync_ExposesBinDataIssuingBank()
        {
            string inputNonce = Nonce.TransactableIssuingBankNetworkOnly;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.IssuingBank, "NETWORK ONLY");
        }

        [Test]
        public async Task FindAsync_ExposesBinDataCountryOfIssuance()
        {
            string inputNonce = Nonce.TransactableCountryOfIssuanceUSA;
            PaymentMethodNonce nonce = await gateway.PaymentMethodNonce.FindAsync(inputNonce);
            Assert.AreEqual(nonce.Nonce, inputNonce);
            Assert.IsNotNull(nonce.BinData);
            Assert.AreEqual(nonce.BinData.CountryOfIssuance, "USA");
        }

        [Test]
        public async Task FindAsync_ExposesThreeDSecureInfo()
        {
            BraintreeService service = new BraintreeService(gateway.Configuration);
            CreditCardRequest creditCardRequest = new CreditCardRequest
            {
                Number = SandboxValues.CreditCardNumber.VISA,
                ExpirationMonth = "05",
                ExpirationYear = "2020"
            };
            string nonce = TestHelper.Generate3DSNonce(service, creditCardRequest);

            PaymentMethodNonce foundNonce = await gateway.PaymentMethodNonce.FindAsync(nonce);
            ThreeDSecureInfo info = foundNonce.ThreeDSecureInfo;

            Assert.AreEqual(foundNonce.Nonce, nonce);
            Assert.AreEqual(foundNonce.Type, "CreditCard");
            Assert.AreEqual(info.Enrolled, "Y");
            Assert.AreEqual(info.Status, "authenticate_successful");
            Assert.AreEqual(info.LiabilityShifted, true);
            Assert.AreEqual(info.LiabilityShiftPossible, true);
        }

        [Test]
        public void Find_ExposesNullThreeDSecureInfoIfBlank()
        {
            string nonce = TestHelper.GenerateUnlockedNonce(gateway);

            PaymentMethodNonce foundNonce = gateway.PaymentMethodNonce.Find(nonce);

            Assert.IsNull(foundNonce.ThreeDSecureInfo);
        }

        [Test]
        public void Find_RaisesNotFoundErrorWhenTokenDoesntExist()
        {
            Assert.Throws<NotFoundException>(() => gateway.PaymentMethodNonce.Find("notarealnonce"));
        }
    }
}
