using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Visualization;
using System;
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
        /// Could be obtained from AqualityServices.
        /// </summary>
        protected abstract IVisualizationConfiguration VisualizationConfiguration { get; }

        /// <summary>
        /// Localized logger used by <see cref="Dump"/>.
        /// Could be obtained from AqualityServices.
        /// </summary>
        protected abstract ILocalizedLogger LocalizedLogger { get; }

        /// <summary>
        /// Gets dump manager for the current form that could be used for visualization purposes, such as saving and comparing dumps.
        /// Uses <see cref="ElementsForVisualization"/> as basis for dump creation and comparison.
        /// </summary>
        public virtual IDumpManager Dump => new DumpManager<T>(ElementsForVisualization, Name, VisualizationConfiguration, LocalizedLogger);

        /// <summary>
        /// List of pairs uniqueName-element to be used for dump saving and comparing.
        /// By default, only currently displayed elements to be used (<see cref="DisplayedElements"/>).
        /// You can override this property with defined <see cref="AllElements"/>, <see cref="AllCurrentFormElements"/>, <see cref="ElementsInitializedAsDisplayed"/> or your own element set.
        /// </summary>
        protected virtual IDictionary<string, T> ElementsForVisualization => DisplayedElements;

        /// <summary>
        /// List of pairs uniqueName-element from all fields and properties of type <typeparamref name="T"/> from the current form and it's parent forms.
        /// </summary>
        protected IDictionary<string, T> AllElements
        {
            get
            {
                var elements = new Dictionary<string, T>();
                AddElementsToDictionary(elements, GetType());
                Type baseType = GetType().BaseType;
                while(baseType != null)
                {
                    AddElementsToDictionary(elements, baseType);
                    baseType = baseType.BaseType;
                }
                return elements;
            }
        }

        /// <summary>
        /// List of pairs uniqueName-element from all fields and properties of type <typeparamref name="T"/> from the current form.
        /// </summary>
        protected IDictionary<string, T> AllCurrentFormElements
        {
            get
            {
                var elements = new Dictionary<string, T>();
                AddElementsToDictionary(elements, GetType());
                return elements;
            }
        }

        /// <summary>
        /// Adds pairs uniqueName-element from the specified type to dictionary using the reflection.
        /// </summary>
        /// <param name="dictionary">Dictionary to save elements.</param>
        /// <param name="type">Type to extract element fields and properties from.</param>
        protected void AddElementsToDictionary(IDictionary<string, T> dictionary, Type type)
        {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            type.GetProperties(bindingFlags).Where(property => typeof(T).IsAssignableFrom(property.PropertyType) && !dictionary.ContainsKey(property.Name))
                .ToList().ForEach(property => dictionary.Add(property.Name, (T)property.GetValue(this)));
            type.GetFields(bindingFlags).Where(field => typeof(T).IsAssignableFrom(field.FieldType) && (!dictionary.ContainsKey(field.Name) || !dictionary.ContainsKey($"_{field.Name}")))
                .ToList().ForEach(field => dictionary.Add(dictionary.Keys.Any(
                    key => key.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)) ? $"_{field.Name}" : field.Name, (T)field.GetValue(this)));
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
