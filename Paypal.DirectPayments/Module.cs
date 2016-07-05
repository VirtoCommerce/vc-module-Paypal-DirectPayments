using System;
using Microsoft.Practices.Unity;
using Paypal.DirectPayments.Managers;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace Paypal.DirectPayments
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void PostInitialize()
        {
            var settings = _container.Resolve<ISettingsManager>().GetModuleSettings("Paypal.DirectPayment");

            Func<PaypalDirectPaymentsPaymentMethod> paypalBankCardsExpressCheckoutPaymentMethodFactory = () => new PaypalDirectPaymentsPaymentMethod
            {
                Name = "Credit Card (Paypal)",
                Description = "Paypal Direct Payment",
                LogoUrl = "https://raw.githubusercontent.com/VirtoCommerce/vc-module-Paypal-DirectPayments/master/Paypal.DirectPayments/Content/paypal_2014_logo.png",
                Settings = settings
            };

            _container.Resolve<IPaymentMethodsService>().RegisterPaymentMethod(paypalBankCardsExpressCheckoutPaymentMethodFactory);
        }

        #endregion
    }
}
