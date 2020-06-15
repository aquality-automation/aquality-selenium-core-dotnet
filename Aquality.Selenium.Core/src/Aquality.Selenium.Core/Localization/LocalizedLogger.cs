using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Logging;
using System;

namespace Aquality.Selenium.Core.Localization
{
    public class LocalizedLogger : ILocalizedLogger
    {
        private readonly ILocalizationManager localizationManager;
        private readonly Logger logger;

        public LocalizedLogger(ILocalizationManager localizationManager, Logger logger, ILoggerConfiguration configuration)
        {
            this.localizationManager = localizationManager;
            this.logger = logger;
            Configuration = configuration;
        }

        public ILoggerConfiguration Configuration { get; }

        public void InfoElementAction(string elementType, string elementName, string messageKey, params object[] args)
        {
            logger.Info($"{elementType} '{elementName}' :: {localizationManager.GetLocalizedMessage(messageKey, args)}");
        }

        public void Info(string messageKey, params object[] args)
        {
            logger.Info(localizationManager.GetLocalizedMessage(messageKey, args));
        }

        public void Debug(string messageKey, Exception exception = null, params object[] args)
        {
            logger.Debug(localizationManager.GetLocalizedMessage(messageKey, args), exception);
        }

        public void Warn(string messageKey, params object[] args)
        {
            logger.Warn(localizationManager.GetLocalizedMessage(messageKey, args));
        }

        public void Error(string messageKey, params object[] args)
        {
            logger.Error(localizationManager.GetLocalizedMessage(messageKey, args));
        }

        public void Fatal(string messageKey, Exception exception = null, params object[] args)
        {
            logger.Fatal(localizationManager.GetLocalizedMessage(messageKey, args), exception);
        }
    }
}
