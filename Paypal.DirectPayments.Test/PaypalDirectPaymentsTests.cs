using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Domain.Commerce.Model;
using Xunit;
using Xunit.Abstractions;

namespace Paypal.DirectPayments.Test
{
    [Trait("Category", "CI")]
    public class PaypalDirectPaymentsTests
    {
        private const string Mode = "Sandbox",
            APIUsername = "em_api1.virtoway.com",
            APIPassword = "T69S32TJRQRZ3B99",
            APISignature = "An5ns1Kso7MWUdW4ErQKJJJ4qi4-AC8NsVtsG6F2RMTHYE-jlx5jBi0m"
            ;

        private readonly ITestOutputHelper output;

        public PaypalDirectPaymentsTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ProcessPayment_successfull()
        {
            //arrange
            var doDirectPaymentRequest = GetDoDirectPaymentRequest();
            var service = GetService();


            //act
            var response = service.DoDirectPayment(doDirectPaymentRequest);

            //assert
            Assert.Equal("", GetErrors(response.Errors));
            //Assert.NotNull(response.getTransactionId());
            //Assert.Equal("000", response.getErrorCode());
        }


        private PayPalAPIInterfaceServiceService GetService()
        {
            var config = new Dictionary<string, string>();

            var isSandbox = Mode.ToLower().Equals("sandbox");
            var url = isSandbox ? "https://api-3t.sandbox.paypal.com/2.0" : "https://api-3t.paypal.com/2.0";

            config.Add("PayPalAPI", url);
            config.Add("mode", Mode);
            config.Add("account0.apiUsername", APIUsername);
            config.Add("account0.apiPassword", APIPassword);
            config.Add("account0.apiSignature", APISignature);

            return new PayPalAPIInterfaceServiceService(config);
        }

        private DoDirectPaymentReq GetDoDirectPaymentRequest()
        {
            var retVal = new DoDirectPaymentReq();

            retVal.DoDirectPaymentRequest = new DoDirectPaymentRequestType();
            retVal.DoDirectPaymentRequest.Version = "117";
            var details = new DoDirectPaymentRequestDetailsType();
            retVal.DoDirectPaymentRequest.DoDirectPaymentRequestDetails = details;
            details.PaymentAction = PaymentActionCodeType.SALE;

            //credit card
            details.CreditCard = GetCreditCardDetails();


            //order totals
            details.PaymentDetails = new PaymentDetailsType();
            details.PaymentDetails.OrderTotal = new BasicAmountType();
            details.PaymentDetails.OrderTotal.value = "845.24";
            details.PaymentDetails.OrderTotal.currencyID = (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), "USD");
            details.PaymentDetails.Custom = "context.Order.Id";
            details.PaymentDetails.ButtonSource = "Virto_SP";

            return retVal;
        }

        private CreditCardDetailsType GetCreditCardDetails()
        {
            var retVal = new CreditCardDetailsType();

            retVal.CreditCardNumber = "4539715785713186";
            retVal.CreditCardType = CreditCardTypeType.VISA;
            retVal.ExpMonth = 11;
            retVal.ExpYear = 2022;
            retVal.CVV2 = "111";
            retVal.CardOwner = new PayerInfoType();

            var billingAddress = GetValidAddress();

            retVal.CardOwner.Address = new PayPal.PayPalAPIInterfaceService.Model.AddressType();
            retVal.CardOwner.Address.Street1 = billingAddress.Line1;
            retVal.CardOwner.Address.Street2 = billingAddress.Line2;
            retVal.CardOwner.Address.CityName = billingAddress.City;
            retVal.CardOwner.Address.StateOrProvince = billingAddress.RegionName;

            retVal.CardOwner.Address.Country = CountryCodeType.US;
            retVal.CardOwner.Address.PostalCode = billingAddress.Zip;
            retVal.CardOwner.Payer = billingAddress.Email;
            retVal.CardOwner.PayerName = new PersonNameType();
            retVal.CardOwner.PayerName.FirstName = billingAddress.FirstName;
            retVal.CardOwner.PayerName.LastName = billingAddress.LastName;
            retVal.CardOwner.PayerCountry = CountryCodeType.US;

            return retVal;
        }

        private static Address GetValidAddress()
        {
            return new Address
            {
                AddressType = VirtoCommerce.Domain.Commerce.Model.AddressType.BillingAndShipping,
                Phone = "+68787687",
                PostalCode = "19142",
                CountryCode = "US",
                CountryName = "United states",
                Email = "user@mail.com",
                FirstName = "first name",
                LastName = "last name",
                Line1 = "6025 Greenway Ave",
                City = "Philadelphia",
                RegionId = "PA",
                RegionName = "Pennsylvania",
                Organization = "org1"
            };
        }

        private string GetErrors(List<ErrorType> errors)
        {
            var sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.AppendLine(error.ErrorCode + ": " + error.LongMessage);
            }
            var retVal = sb.ToString();

            if (!string.IsNullOrEmpty(retVal))
                output.WriteLine(retVal);

            return retVal;
        }
    }
}
