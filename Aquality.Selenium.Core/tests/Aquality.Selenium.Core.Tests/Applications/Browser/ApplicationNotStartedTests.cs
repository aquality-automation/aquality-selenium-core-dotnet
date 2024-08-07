﻿using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Waitings;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Applications.Browser
{
    public class ApplicationNotStartedTests : TestWithBrowser
    {
        private static readonly Type[] TypesNotRequireApplication = 
        [ 
            typeof(IElementActionRetrier),
            typeof(IConditionalWait),
            typeof(Logger),
            typeof(ILocalizationManager),
            typeof(ILocalizedLogger),
            typeof(IElementFinder),
            typeof(IElementFactory),
            typeof(ITimeoutConfiguration),
            typeof(ILoggerConfiguration),
            typeof(IRetryConfiguration)
        ];

        [TestCaseSource(nameof(TypesNotRequireApplication))]
        public void Should_NotStartApplication_ForServiceResolving(Type type)
        {
            Assert.That(ServiceProvider.GetRequiredService(type), Is.Not.Null);
            Assert.That(AqualityServices.IsApplicationStarted, Is.False);
        }
    }
}
