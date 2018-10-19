using Braintree.TestUtil;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Braintree.Tests
{
    [TestFixture]
    public class RequestTest
    {
        [Test]
        public void ToXML_EnsuresUSLocaleForDecimals()
        {
            CultureInfo existingCulture = CultureInfo.CurrentCulture;

            try
            {
#if netcore
                CultureInfo.CurrentCulture = new CultureInfo("it-IT");
#else
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");
#endif

                TransactionRequest transactionRequest = new TransactionRequest
                {
                    Amount = 100.0M,
                    TaxAmount = 10.0M,
                };

                SubscriptionRequest subscriptionRequest = new SubscriptionRequest
                {
                    Price = 200.0M,
                };

                SubscriptionTransactionRequest subscriptionTransactionRequest = new SubscriptionTransactionRequest
                {
                    Amount = 300.0M
                };

                ModificationRequest modificationRequest = new ModificationRequest
                {
                    Amount = 400.0M
                };

                TestHelper.AssertIncludes("<amount>100.00</amount>", transactionRequest.ToXml());
                TestHelper.AssertIncludes("<tax-amount>10.00</tax-amount>", transactionRequest.ToXml());
                TestHelper.AssertIncludes("<price>200.00</price>", subscriptionRequest.ToXml());
                TestHelper.AssertIncludes("<amount>300.00</amount>", subscriptionTransactionRequest.ToXml());
                TestHelper.AssertIncludes("<amount>400.00</amount>", modificationRequest.ToXml("root"));
            }
            finally
            {
#if netcore
                CultureInfo.CurrentCulture = existingCulture;
#else
                Thread.CurrentThread.CurrentCulture = existingCulture;
#endif
            }
        }

        [Test]
        public void ToXML_EscapesGeneratedXMLForString()
        {
            TransactionRequest request = new TransactionRequest
            {
                OrderId = "<>&\"'"
            };

            TestHelper.AssertIncludes("<order-id>&lt;&gt;&amp;&quot;&apos;</order-id>", request.ToXml());
        }

        [Test]
        public void ToXML_EscapesGeneratedXMLForNestedSearchCriteria()
        {
            TransactionRequest request = new TransactionRequest
            {
                Customer = new CustomerRequest
                {
                    FirstName = "<John>"
                }
            };

            TestHelper.AssertIncludes("<first-name>&lt;John&gt;</first-name>", request.ToXml());
        }

        [Test]
        public void ToXML_EscapesGeneratedXMLForArrayElements()
        {
            SubscriptionSearchRequest request = new SubscriptionSearchRequest().
                Ids.IncludedIn("<active");

            TestHelper.AssertIncludes("<ids type=\"array\"><item>&lt;active</item></ids>", request.ToXml());
        }

        [Test]
        public void ToXML_EscapesGeneratedXMLForSearchCriteria()
        {
            SubscriptionSearchRequest request = new SubscriptionSearchRequest().
                PlanId.Is("<my-id");

            TestHelper.AssertIncludes("<plan-id><is>&lt;my-id</is></plan-id>", request.ToXml());
        }

        [Test]
        public void ToXML_DefaultsNullCriteriaToEmptyString()
        {
            SubscriptionSearchRequest request = new SubscriptionSearchRequest().
                PlanId.Is(null);

            TestHelper.AssertIncludes("<plan-id><is></is></plan-id>", request.ToXml());
        }

        [Test]
        public void ToXML_EscapesGeneratedXMLForCustomFields()
        {
            TransactionRequest request = new TransactionRequest
            {
                CustomFields = new Dictionary<string, string>
                {
                    { "<key>", "val&ue" }
                }
            };

            TestHelper.AssertIncludes("<custom-fields><&lt;key&gt;>val&amp;ue</&lt;key&gt;></custom-fields>", request.ToXml());
        }

        [Test]
        public void ToXML_EscapesParentElementName()
        {
            var requestBuilder = new RequestBuilder("<&\"\'>");

            Assert.AreEqual("<&lt;&amp;&quot;&apos;&gt;></&lt;&amp;&quot;&apos;&gt;>", requestBuilder.ToXml());
        }

        [Test]
        public void ToXML_EscapesParentElementNameWithDictionaryValue()
        {
            var customFields = new Dictionary<string, string>();
            Assert.AreEqual("<&lt;&amp;&quot;&apos;&gt;></&lt;&amp;&quot;&apos;&gt;>", RequestBuilder.BuildXMLElement("<&\"\'>", customFields));
        }
    }
}
