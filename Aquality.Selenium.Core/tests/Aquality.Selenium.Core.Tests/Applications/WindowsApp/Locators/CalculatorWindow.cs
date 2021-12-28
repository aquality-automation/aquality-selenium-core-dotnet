﻿using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp.Locators
{
    public static class CalculatorWindow
    { 
        public static By WindowLocator => MobileBy.ClassName("ThunderRT6FormDC");

        public static By OneButton => MobileBy.Name("1");

        public static By TwoButton => MobileBy.Name("2");

        public static By ThreeButton => MobileBy.Name("3");

        public static By PlusButton => MobileBy.Name("+");

        public static By EqualsButton => MobileBy.Name("=");

        public static By ResultsLabel => MobileBy.AccessibilityId("48");

        public static By EmptyButton => MobileBy.AccessibilityId("7");

        public static By AbsentElement => MobileBy.Name("Absent element");
    }
}
