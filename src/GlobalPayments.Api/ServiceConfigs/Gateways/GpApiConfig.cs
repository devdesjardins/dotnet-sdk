﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Gateways;
using GlobalPayments.Api.Utils;
using System;
using System.Globalization;

namespace GlobalPayments.Api {
    public class GpApiConfig : GatewayConfig {
        /// <summary>
        /// GP API app id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// GP API app key
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// The time left in seconds before the token expires
        /// </summary>
        public int? SecondsToExpire { get; set; }
        /// <summary>
        /// The time interval set for when the token will expire
        /// </summary>
        public IntervalToExpire? IntervalToExpire { get; set; }
        /// <summary>
        /// Channel
        /// </summary>
        public Channel Channel { get; set; } = Channel.CardNotPresent;
        /// <summary>
        /// Gloabl API language configuration
        /// </summary>
        public Language Language { get; set; } = Language.English;
        /// <summary>
        /// Two letter ISO 3166 country
        /// </summary>
        public string Country { get; set; } = "US";
        /// <summary>
        /// Access token information
        /// </summary>
        public AccessTokenInfo AccessTokenInfo { get; set; }

        public GpApiConfig() : base(GatewayProvider.GP_API) { }

        internal override void ConfigureContainer(ConfiguredServices services) {
            if (string.IsNullOrEmpty(ServiceUrl)) {
                if (Environment.Equals(Entities.Environment.TEST))
                    ServiceUrl = ServiceEndpoints.GP_API_TEST;
                else
                    ServiceUrl = ServiceEndpoints.GP_API_PRODUCTION;
            }

            var gateway = new GpApiConnector {
                AppId = AppId,
                AppKey = AppKey,
                SecondsToExpire = SecondsToExpire,
                IntervalToExpire = IntervalToExpire,
                Channel = Channel,
                Language = Language,
                Country = Country,
                ServiceUrl = ServiceUrl,
                Timeout = Timeout,
                AccessToken = AccessTokenInfo?.Token,
                DataAccountName = AccessTokenInfo?.DataAccountName,
                DisputeManagementAccountName = AccessTokenInfo?.DisputeManagementAccountName,
                TokenizationAccountName = AccessTokenInfo?.TokenizationAccountName,
                TransactionProcessingAccountName = AccessTokenInfo?.TransactionProcessingAccountName,
                RequestLogger = RequestLogger,
                WebProxy = WebProxy,
            };

            services.GatewayConnector = gateway;

            services.ReportingService = gateway;

            services.SetSecure3dProvider(Secure3dVersion.Any, gateway);
        }

        internal override void Validate() {
            base.Validate();

            if (AccessTokenInfo == null && (AppId == null || AppKey == null))
                throw new ConfigurationException("AccessTokenInfo or AppId and AppKey cannot be null.");
        }
    }
}
