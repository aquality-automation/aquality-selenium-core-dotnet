﻿using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using System;
using System.IO;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    public class AqualityServices : AqualityServices<WindowsApplication>
    {
        private const string SupportedApplication = "./Resources/WindowsApp/Day Maxi Calc.exe";
        private const string DefaultDriverServer = "http://127.0.0.1:4723/";

        public new static bool IsApplicationStarted => IsApplicationStarted();

        public static WindowsApplication Application => GetApplication(service => StartApplication(service));

        public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application);

        private static WindowsApplication StartApplication(IServiceProvider services)
        {
            var isRemoteFromEnv = EnvironmentConfiguration.GetVariable("isRemote");
            var isRemote = bool.Parse(isRemoteFromEnv);
            Uri driverServer;
            AppiumOptions options = new();
            if (!isRemote)
            {
                var driverService = AppiumLocalService.BuildDefaultService();
                driverService.Start();
                driverServer = driverService.ServiceUrl;
                options.PlatformVersion = "10";
                options.PlatformName = "Windows";
                options.DeviceName = "WindowsPC";
            }
            else
            {
                driverServer = new Uri(EnvironmentConfiguration.GetVariable("driverServer") ?? DefaultDriverServer);
            }            
            return new WindowsApplication(Path.GetFullPath(SupportedApplication), driverServer, services.GetRequiredService<ITimeoutConfiguration>(), options);
        }
    }
}
