using Aquality.Selenium.Core.Elements.Interfaces;
using OpenQA.Selenium;

namespace Aquality.Selenium.Core.Elements
{
    /// <summary>
    /// Delegate that defines constructor of element.
    /// </summary>
    /// <typeparam name="T">Type of element that has to implement IElement interface</typeparam>
    /// <param name="locator">Element locator</param>
    /// <param name="name">Element name</param>
    /// <param name="state">Element state</param>
    /// <returns>Element instance of type T</returns>
    public delegate T ElementSupplier<out T>(By locator, string name, ElementState state) where T : IElement;
}
