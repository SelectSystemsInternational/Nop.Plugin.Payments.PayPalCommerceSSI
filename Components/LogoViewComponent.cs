using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

using Nop.Plugin.Payments.PayPalCommerceSSI.Services;
using Nop.Plugin.Payments.PayPalCommerceSSI.Settings;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Components
{
    /// <summary>
    /// Represents the view component to display logo
    /// </summary>
    [ViewComponent(Name = PayPalCommerceDefaults.LOGO_VIEW_COMPONENT_NAME)]
    public class PayPalCommerceLogoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PayPalCommerceSettingsSSI _settings;

        #endregion

        #region Ctor

        public PayPalCommerceLogoViewComponent(IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext,
            PayPalCommerceSettingsSSI settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(PayPalCommerceDefaults.SystemName, customer, store?.Id ?? 0))
                return Content(string.Empty);

            if (!ServiceManager.IsConfigured(_settings))
                return Content(string.Empty);

            var script = widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) && _settings.DisplayLogoInHeaderLinks
                ? _settings.LogoInHeaderLinks
                : (widgetZone.Equals(PublicWidgetZones.Footer) && _settings.DisplayLogoInFooter
                ? _settings.LogoInFooter
                : null);

            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}