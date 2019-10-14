using Aquality.Selenium.Core.Logging;
using System;

namespace Aquality.Selenium.Core.Localization
{
    public class LocalizationLogger : ILocalizationLogger
    {
        private readonly ILocalizationManager localizationManager;
        private readonly Logger logger;

        public LocalizationLogger(ILocalizationManager localizationManager, Logger logger)
        {
            this.localizationManager = localizationManager;
            this.logger = logger;
        }

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
