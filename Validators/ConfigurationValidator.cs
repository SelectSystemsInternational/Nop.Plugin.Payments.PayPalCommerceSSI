using FluentValidation;
using Nop.Plugin.Payments.PayPalCommerceSSI.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.ClientId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerceSSI.Fields.ClientId.Required"))
                .When(model => !model.UseSandbox && model.SetCredentialsManually);

            RuleFor(model => model.SecretKey)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerceSSI.Fields.SecretKey.Required"))
                .When(model => !model.UseSandbox && model.SetCredentialsManually);
        }

        #endregion
    }
}