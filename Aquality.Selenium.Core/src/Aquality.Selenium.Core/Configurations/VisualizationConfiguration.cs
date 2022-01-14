using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Visualization;
using System.Drawing.Imaging;
using System.IO;

namespace Aquality.Selenium.Core.Configurations
{
    /// <summary>
    /// Represents visualization configuration, used for image comparison.
    /// Uses <see cref="ISettingsFile"/> as source for configuration values.
    /// </summary>
    public class VisualizationConfiguration : IVisualizationConfiguration
    {
        private readonly ISettingsFile settingsFile;

        /// <summary>
        /// Instantiates class using <see cref="ISettingsFile"/> with visualization settings.
        /// </summary>
        /// <param name="settingsFile">Settings file.</param>
        public VisualizationConfiguration(ISettingsFile settingsFile)
        {
            this.settingsFile = settingsFile;
        }

        public virtual ImageFormat ImageExtension => ImageExtensions.ConvertImageFormat(settingsFile.GetValueOrDefault(".visualization.imageExtension", ".png"));

        public int MaxFullFileNameLength => settingsFile.GetValueOrDefault(".visualization.maxFullFileNameLength", 255);

        public float DefaultThreshold => settingsFile.GetValueOrDefault(".visualization.defaultThreshold", 0.012f);

        public int ComparisonWidth => settingsFile.GetValueOrDefault(".visualization.comparisonWidth", 16);

        public int ComparisonHeight => settingsFile.GetValueOrDefault(".visualization.comparisonHeight", 16);

        public string PathToDumps
        {
            get
            {
                var pathInConfiguration = settingsFile.GetValueOrDefault(".visualization.pathToDumps", "../../../Resources/VisualDumps/");
                return pathInConfiguration.Contains(".") ? Path.GetFullPath(pathInConfiguration) : pathInConfiguration;
            }
        }
    }
}
