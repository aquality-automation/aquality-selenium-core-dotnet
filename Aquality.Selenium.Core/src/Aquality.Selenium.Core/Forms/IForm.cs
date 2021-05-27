using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Visualization;

namespace Aquality.Selenium.Core.Forms
{
    /// <summary>
    /// Describes form that could be used for visualization purposes, such as saving and comparing dumps.
    /// </summary>
    public interface IForm
    {
        /// <summary>
        /// Name of the current form.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets dump manager for the current form that could be used for visualization purposes, such as saving and comparing dumps.
        /// </summary>
        IDumpManager Dump { get; }
    }
}
