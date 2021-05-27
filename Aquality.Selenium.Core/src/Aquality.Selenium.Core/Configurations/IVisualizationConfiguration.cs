namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Represents visualization configuration, used for image comparison.
    /// </summary>
    public interface IVisualizationConfiguration
    {
        /// <summary>
        /// Default threshold used for image comparison.
        /// </summary>
        float DefaultThreshold { get; }

        /// <summary>
        /// Width of the image resized for comparison.
        /// </summary>
        int ComparisonWidth { get; }

        /// <summary>
        /// Height of the image resized for comparison.
        /// </summary>
        int ComparisonHeight { get; }

        /// <summary>
        /// Path used to save and load page dumps.
        /// </summary>
        string PathToDumps { get; }
    }
}
