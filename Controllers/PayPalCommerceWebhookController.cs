using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Nop.Plugin.Payments.PayPalCommerceSSI.Services;
using Nop.Plugin.Payments.PayPalCommerceSSI.Settings;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Controllers
{
    public class PayPalCommerceSSIWebhookController : Controller
    {
        #region Fields

        private readonly PayPalCommerceSettingsSSI _settings;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PayPalCommerceSSIWebhookController(PayPalCommerceSettingsSSI settings,
            ServiceManager serviceManager)
        {
            _settings = settings;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> WebhookHandler()
        {
            await _serviceManager.HandleWebhookAsync(_settings, Request);
            return Ok();
        }

        #endregion
    }
}