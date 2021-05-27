using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Visualization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aquality.Selenium.Core.Forms
{
    /// <summary>
    /// Describes form that could be used for visualization purposes (see <see cref="Dump"/>).
    /// </summary>
    /// <typeparam name="T">Base type(class or interface) of elements of this form.</typeparam>
    public abstract class Form<T> : IForm where T : IElement
    {
        /// <summary>
        /// Name of the current form.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Visualization configuration used by <see cref="Dump"/>.
        /// Could be get from AqualityServices.
        /// </summary>
        protected abstract IVisualizationConfiguration VisualizationConfiguration { get; }

        /// <summary>
        /// Localizer logger used by <see cref="Dump"/>.
        /// Could be get from AqualityServices.
        /// </summary>
        protected abstract ILocalizedLogger LocalizedLogger { get; }

        /// <summary>
        /// Gets dump manager for the current form that could be used for visualization purposes, such as saving and comparing dumps.
        /// Uses <see cref="ElementsForVisualization"/> as basis for dump creation and comparison.
        /// </summary>
        public virtual IDumpManager Dump => new DumpManager<T>(ElementsForVisualization, VisualizationConfiguration, LocalizedLogger);

        /// <summary>
        /// List of pairs uniqueName-element to be used for dump saving and comparing.
        /// By default, only currently displayed elements to be used (<see cref="ElementsInitializedAsDisplayed"/>).
        /// You can override this property with defined <see cref="AllElements"/>, <see cref="DisplayedElements"/> or your own element set.
        /// </summary>
        protected virtual IDictionary<string, T> ElementsForVisualization => DisplayedElements;

        /// <summary>
        /// List of pairs uniqueName-element from all fields and properties of type <typeparamref name="T"/>.
        /// </summary>
        protected IDictionary<string, T> AllElements
        {
            get
            {
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                var elementFields = GetType().GetFields(bindingFlags).Where(field => typeof(T).IsAssignableFrom(field.FieldType))
                    .ToDictionary(field => field.Name, field => (T) field.GetValue(this));
                var elementProperties = GetType().GetProperties(bindingFlags).Where(property => typeof(T).IsAssignableFrom(property.PropertyType))
                    .ToDictionary(property => property.Name, property => (T) property.GetValue(this));
                return elementFields.Concat(elementProperties)
                    .ToDictionary(el => el.Key, el => el.Value);
            }
        }

        /// <summary>
        /// List of pairs uniqueName-element from all fields and properties of type <typeparamref name="T"/>,
        /// which were initialized as <see cref="ElementState.Displayed"/>.
        /// </summary>
        protected IDictionary<string, T> ElementsInitializedAsDisplayed => AllElements
            .Where(element => element.Value is Element && (element.Value as Element).elementState == ElementState.Displayed)
            .ToDictionary(el => el.Key, el => el.Value);

        /// <summary>
        /// List of pairs uniqueName-element from all fields and properties of type <typeparamref name="T"/>,
        /// which are currently displayed (using <see cref="IElementStateProvider.IsDisplayed"/>).
        /// </summary>
        protected IDictionary<string, T> DisplayedElements => AllElements.Where(element => element.Value.State.IsDisplayed)
            .ToDictionary(el => el.Key, el => el.Value);
    }
}
