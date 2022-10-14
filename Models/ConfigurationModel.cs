using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            PaymentTypes = new List<SelectListItem>();
            OnboardingModel = new OnboardingModel();
        }

        #endregion

        #region Properties

        public bool IsConfigured { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.Email")]
        [EmailAddress]
        public string Email { get; set; }
        public bool Email_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.SetCredentialsManually")]
        public bool SetCredentialsManually { get; set; }
        public bool SetCredentialsManually_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.SecretKey")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.PaymentType")]
        public int PaymentTypeId { get; set; }
        public bool PaymentTypeId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentTypes { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.DisplayButtonsOnShoppingCart")]
        public bool DisplayButtonsOnShoppingCart { get; set; }
        public bool DisplayButtonsOnShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.DisplayButtonsOnProductDetails")]
        public bool DisplayButtonsOnProductDetails { get; set; }
        public bool DisplayButtonsOnProductDetails_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.DisplayLogoInHeaderLinks")]
        public bool DisplayLogoInHeaderLinks { get; set; }
        public bool DisplayLogoInHeaderLinks_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.LogoInHeaderLinks")]
        public string LogoInHeaderLinks { get; set; }
        public bool LogoInHeaderLinks_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.DisplayLogoInFooter")]
        public bool DisplayLogoInFooter { get; set; }
        public bool DisplayLogoInFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.LogoInFooter")]
        public string LogoInFooter { get; set; }
        public bool LogoInFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.UseWorkingCurrency")]
        public bool UseWorkingCurrency { get; set; }
        public bool UseWorkingCurrency_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerceSSI.Fields.TestMode")]
        public bool TestMode { get; set; }
        public bool TestMode_OverrideForStore { get; set; }

        public OnboardingModel OnboardingModel { get; set; }

        #endregion
    }
}