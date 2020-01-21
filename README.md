[![Build Status](https://dev.azure.com/aquality-automation/aquality-automation/_apis/build/status/aquality-automation.aquality-selenium-core-dotnet?branchName=master)](https://dev.azure.com/aquality-automation/aquality-automation/_build/latest?definitionId=3&branchName=master)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=aquality-automation_aquality-selenium-core-dotnet&metric=alert_status)](https://sonarcloud.io/dashboard?id=aquality-automation_aquality-selenium-core-dotnet)
[![NuGet](https://img.shields.io/nuget/v/Aquality.Selenium.Core)](https://www.nuget.org/packages/Aquality.Selenium.Core)

# Aquality Selenium CORE for .NET

### Overview

This package is a library with core functions simplifying work with Selenium-controlled applications.

You've got to use this set of methods, related to most common actions performed with elements.

Most of performed methods are logged using NLog, so you can easily see a history of performed actions in your log. We support different logging languages.

We use interfaces where is possible, so you can implement your own version of target interface with no need to rewrite other classes.

We use Dependency Injection to simplify overriding of implementations.

### Components of solution

1. Applications component provides classes and interfaces which help us work with application and DI container. [AqualityServices](https://github.com/aquality-automation/aquality-selenium-core-dotnet/blob/master/Aquality.Selenium.Core/src/Aquality.Selenium.Core/Applications/AqualityServices.cs) can get\set service provider and application. [Startup](https://github.com/aquality-automation/aquality-selenium-core-dotnet/blob/master/Aquality.Selenium.Core/src/Aquality.Selenium.Core/Applications/Startup.cs) is needed to setup DI container.

2. Configurations component provides classes and interfaces which describe most common configurations of project.

3. Elements component describes classes and interfaces which works with UI elements.

4. Solution contains logger and support several languages, Localization and Logging components helps us to implement this.

5. Resources contains localization and project configuration in json files.

6. Utilities.

7. Waitings component contains classes and interfaces which implement some common waitings, for example, wait till condition is satisfied.

### Quick start

1. To start work with this package, simply add the nuget dependency Aquality.Selenium.Core to your project.

2. Setup DI container using Startup.cs. 

The simpliest way is to create your AqualityServices class extended from abstract AqualityServices with the following simple signature:
```csharp

    public class AqualityServices : AqualityServices<YourApplication>
    {
        public new static bool IsApplicationStarted => IsApplicationStarted();

        public static YourApplication Application => GetApplication(services => StartApplication(services));

        public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application);

        private static IApplication StartApplication(IServiceProvider services)
        {
            your implementation;
        }
    }
```

If you need to register your own services / rewrite the implementation, you need override [Startup](https://github.com/aquality-automation/aquality-selenium-core-dotnet/blob/master/Aquality.Selenium.Core/src/Aquality.Selenium.Core/Applications/Startup.cs) and implement AqualityServices like in example below:

```csharp
        public class AqualityServices : AqualityServices<IApplication>
        {
            private static ThreadLocal<YourStartup> startup = new ThreadLocal<YourStartup>();

            public new static bool IsApplicationStarted => IsApplicationStarted();
            
            public static YourApplication Application => GetApplication(StartApplicationFunction, () => startup.Value.ConfigureServices(new ServiceCollection(), services => Application));

            public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application,
                () => startup.Value.ConfigureServices(new ServiceCollection(), services => Application));

            public static void SetStartup(Startup startup)
            {
                if (startup != null)
                {
                    TestAqualityServices.startup.Value = (TestStartup)startup;
                }
            }

            private static Func<IServiceProvider, YourApplication> StartApplicationFunction => (services) => your implementation;
        }

        public class YourStartup : Startup
        {
            public override IServiceCollection ConfigureServices(IServiceCollection services, Func<IServiceProvider, IApplication> applicationProvider, ISettingsFile settings = null)
            {
                var settingsFile = new JsonSettingsFile($"Resources.settings.{SpecialSettingsFile}.json", Assembly.GetExecutingAssembly());
                base.ConfigureServices(services, applicationProvider, settingsFile);
                //your services like services.AddSingleton<ITimeoutConfiguration>(new TestTimeoutConfiguration(settingsFile));
                return services;
            }
        }
```
3. That's it! Work with Application via AqualityServices or via element services.

All the services could be resolved from the DI container via ServiceProvider.

```csharp
            AqualityServices.Application.Driver.FindElement(CalculatorWindow.OneButton).Click();
            AqualityServices.ServiceProvider.GetService<IConditionalWait>().WaitFor(driver =>
            {
                return driver.FindElements(By.XPath("//*")).Count > 0;
            })
            AqualityServices.ServiceProvider.GetService<IElementFinder>()
                .FindElement(CalculatorWindow.ResultsLabel, timeout: LittleTimeout)
```

4. Extend your elements from Element class:
```csharp
    public abstract class WindowElement : Element
    {
        protected WindowElement(By locator, string name, ElementState state) : base(locator, name, state)
        {
        }

        protected override ElementActionRetrier ActionRetrier => AqualityServices.ServiceProvider.GetService<ElementActionRetrier>();

        protected override IApplication Application => ApplicationManager.Application;

        protected override IConditionalWait ConditionalWait => AqualityServices.ServiceProvider.GetService<IConditionalWait>();

        protected override IElementFactory Factory => AqualityServices.ServiceProvider.GetService<IElementFactory>();

        protected override IElementFinder Finder => AqualityServices.ServiceProvider.GetService<IElementFinder>();

        protected override LocalizationLogger LocalizationLogger => AqualityServices.ServiceProvider.GetService<LocalizationLogger>();
    }
```

```csharp
    public class Label : WindowElement
    {
        public Label(By locator, string name, ElementState state) : base(locator, name, state)
        {
        }

        protected override string ElementType => "Label";
    }
```

5. Extend ElementFactory to get your own elements:
```csharp
    public static class ElementFactoryExtensions
    {
        public static Label GetLabel(this IElementFactory elementFactory, By elementLocator, string elementName)
        {
            return elementFactory.GetCustomElement(GetLabelSupplier(), elementLocator, elementName);
        }

        private static ElementSupplier<Label> GetLabelSupplier()
        {
            return (locator, name, state) => new Label(locator, name, state);
        }
    }
```

Or create your own ElementFactory! You can extend it from Core's ElementFactory or just implement IElementFactory interface.
(Don't forget to register it in the DI container at AqualityServices!).

6. Work with Windows/Pages/Forms according to PageObject pattern.
Create a base Form class with protected access to IApplication instance and IElementFactory (and any other needed service) via ApplicationManager. Other forms will inherit from this one with the mentioned services available. Take a look at example here:
```csharp
    /// <summary>
    /// Defines base class for any UI form.
    /// </summary>
    public abstract class Form
    {
        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="locator">Unique locator of the form.</param>
        /// <param name="name">Name of the form.</param>
        protected Form(By locator, string name)
        {
            Locator = locator;
            Name = name;
        }

        /// <summary>
        /// Locator of specified form.
        /// </summary>
        public By Locator { get; }

        /// <summary>
        /// Name of specified form.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Instance of logger <see cref="Logging.Logger">
        /// </summary>
        /// <value>Logger instance.</value>
        protected Logger Logger => AqualityServices.ServiceProvider.GetService<Logger>();

        /// <summary>
        /// Element factory <see cref="IElementFactory">
        /// </summary>
        /// <value>Element factory.</value>
        protected IElementFactory ElementFactory => AqualityServices.ServiceProvider.GetService<IElementFactory>();

        /// <summary>
        /// Return form state for form locator
        /// </summary>
        /// <value>True - form is opened,
        /// False - form is not opened.</value>
        public bool IsDisplayed => FormLabel.State.WaitForDisplayed();

        /// <summary>
        /// Gets size of form element defined by its locator.
        /// </summary>
        public Size Size => FormLabel.GetElement().Size;

        private Label FormLabel => ElementFactory.GetLabel(Locator, Name);
    }

```

### F.A.Q.

If you've got any questions, take a look at Aquality.Selenium.Core.Tests project - probably it already has an implementation of what you're trying to achieve.
Also feel free to ask any project's collaborator / to create an issue if needed.


### License
Library's source code is made available under the [Apache 2.0 license](https://github.com/aquality-automation/aquality-selenium-core-dotnet/blob/master/LICENSE).
