using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Forms;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Describes dump manager for the form that could be used for visualization purposes, such as saving and comparing dumps.
    /// </summary>
    public interface IDumpManager
    {
        /// <summary>
        /// Compares current form with the dump saved previously.
        /// </summary>
        /// <param name="dumpName">Custom name of the sub-folder where the dump was saved. 
        /// Form name <see cref="IForm.Name"/> is used by default.</param>
        /// <returns>The difference of comparing the page to the dump as a percentage (no difference is 0%).
        /// Calculated as sum of element differences divided by elements count.</returns>
        float CompareWithDump(string dumpName = null);

        /// <summary>
        /// Saves the dump of the current form 
        /// (a set of screenshots of selected form elements)
        /// into dump folder under the path <see cref="IVisualizationConfiguration.PathToDumps"/>.
        /// </summary>
        /// <param name="dumpName">Name of the sub-folder where to save the dump. 
        /// Form name <see cref="IForm.Name"/> is used by default.</param>
        void SaveDump(string dumpName = null);
    }
}