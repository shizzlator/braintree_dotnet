using Braintree.Test;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Braintree.Tests.Integration
{
    [TestFixture()]
    public class TestTransactionIntegrationTest {

        private BraintreeGateway gateway;

        [SetUp()]
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
        public void Settle()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = gateway.Transaction.Sale(request);
            Transaction transaction = gateway.TestTransaction.Settle(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLED, transaction.Status);
        }

        [Test]
        public async Task SettleAsync()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = await gateway.Transaction.SaleAsync(request);
            Transaction transaction = await gateway.TestTransaction.SettleAsync(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLED, transaction.Status);
        }

        [Test]
        public void SettlementConfirm()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = gateway.Transaction.Sale(request);
            Transaction transaction = gateway.TestTransaction.SettlementConfirm(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLEMENT_CONFIRMED, transaction.Status);
        }

        [Test]
        public async Task SettlementConfirmAsync()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = await gateway.Transaction.SaleAsync(request);
            Transaction transaction = await gateway.TestTransaction.SettlementConfirmAsync(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLEMENT_CONFIRMED, transaction.Status);
        }

        [Test]
        public void SettlementPending()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = gateway.Transaction.Sale(request);
            Transaction transaction = gateway.TestTransaction.SettlementPending(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLEMENT_PENDING, transaction.Status);
        }

        [Test]
        public void SettlementDecline()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = gateway.Transaction.Sale(request);
            Transaction transaction = gateway.TestTransaction.SettlementDecline(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLEMENT_DECLINED, transaction.Status);
        }

        [Test]
        public async Task SettlementDeclineAsync()
        {
            var request = new TransactionRequest
            {
                Amount = 100M,
                PaymentMethodNonce = Nonce.Transactable,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var transactionResult = await gateway.Transaction.SaleAsync(request);
            Transaction transaction = await gateway.TestTransaction.SettlementDeclineAsync(transactionResult.Target.Id);

            Assert.AreEqual(TransactionStatus.SETTLEMENT_DECLINED, transaction.Status);
        }
    }
}
