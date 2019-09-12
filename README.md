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

### Quick start

1. To start work with this package, simply add the nuget dependency Aquality.Selenium.Core to your project.

2. Setup DI container using Startup.cs. 

The simpliest way is to create your ApplicationManager class extended from abstract ApplicationManager with the following simple signature:
```csharp

        public class ApplicationManager : ApplicationManager<ApplicationManager, YourApplication>
        {
            public static IApplication Application => GetApplication(StartApplicationFunction);

            public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application);

            private static Func<IServiceProvider, IApplication> StartApplicationFunction => // your implementation here;
        }
```

Or, if you need to register your own services / rewrite the implementation, you can achieve it this way:

```csharp

        public class ApplicationManager : ApplicationManager<ApplicationManager, YourApplication>
        {
            public static YourApplication Application => GetApplication(StartApplicationFunction, , () => RegisterServices(services => Application));

            public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application, , () => RegisterServices(services => Application));

            private static IServiceCollection RegisterServices(Func<IServiceProvider, YourApplication> applicationSupplier)
            {
                var services = new ServiceCollection();
                var startup = new Startup();
                var settingsFile = startup.GetSettings();
                startup.ConfigureServices(services, applicationSupplier, settingsFile);
                services.AddSingleton<ITimeoutConfiguration>(new CustomTimeoutConfiguration(settingsFile));
                return services;
            }

            private static Func<IServiceProvider, YourApplication> StartApplicationFunction => // your implementation here;
        }
```
3. That's it! Work with Application via ApplicationManager or via element services.

All the services could be resolved from the DI container via ServiceProvider.

```csharp
            ApplicationManager.Application.Driver.FindElement(CalculatorWindow.OneButton).Click();
            ApplicationManager.ServiceProvider.GetRequiredService<ConditionalWait>().WaitFor(driver =>
            {
                return driver.FindElements(By.XPath("//*")).Count > 0;
            })
            ApplicationManager.ServiceProvider.GetRequiredService<IElementFinder>()
                .FindElement(CalculatorWindow.ResultsLabel, timeout: LittleTimeout)
```

4. Extend your elements from Element class:
```csharp
    public abstract class WindowElement : Element
    {
        protected WindowElement(By locator, string name, ElementState state) : base(locator, name, state)
        {
        }

        protected override ElementActionRetrier ActionRetrier => ApplicationManager.ServiceProvider.GetRequiredService<ElementActionRetrier>();

        protected override IApplication Application => ApplicationManager.Application;

        protected override ConditionalWait ConditionalWait => ApplicationManager.ServiceProvider.GetRequiredService<ConditionalWait>();

        protected override IElementFactory Factory => ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();

        protected override IElementFinder Finder => ApplicationManager.ServiceProvider.GetRequiredService<IElementFinder>();

        protected override LocalizationLogger LocalizationLogger => ApplicationManager.ServiceProvider.GetRequiredService<LocalizationLogger>();
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
(Don't forget to register it in the DI container at ApplicationManager!).

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
        protected Logger Logger => ApplicationManager.ServiceProvider.GetRequiredService<Logger>();

        /// <summary>
        /// Element factory <see cref="IElementFactory">
        /// </summary>
        /// <value>Element factory.</value>
        protected IElementFactory ElementFactory => ApplicationManager.ServiceProvider.GetRequiredService<IElementFactory>();

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
