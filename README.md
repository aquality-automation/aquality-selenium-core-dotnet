
# Aquality Selenium CORE for .NET

### Overview

This package is a library with core functions simplifying work with Selenium-controlled applications.

You've got to use this set of methods, related to most common actions performed with elements.

Most of performed methods are logged using NLog, so you can easily see a history of performed actions in your log. We support different logging languages.

We use interfaces where is possible, so you can implement your own version of target interface with no need to rewrite other classes.

We use Dependency Injection to simplify overriding of implementations.

### Quick start #todo

1. To start work with this package, simply add the nuget dependency Aquality.Selenium.Core to your project.

2. Setup DI container using Startup.cs

3. Extend your elements from Element class

4. Extend ElementFactory

5. ???

6. Profit!



### License
Library's source code is made available under the [Apache 2.0 license](https://github.com/aquality-automation/aquality-selenium-core-dotnet/blob/master/LICENSE).
