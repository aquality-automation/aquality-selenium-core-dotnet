using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Aquality.Selenium.Core.Visualization
{
    public class DumpManager<T> : IDumpManager where T : IElement
    {
        private const string ImageFormat = ".png";
        public DumpManager(IDictionary<string, T> elementsForVisualization, string formName, IVisualizationConfiguration visualizationConfiguration, ILocalizedLogger localizedLogger)
        {
            ElementsForVisualization = elementsForVisualization;
            FormName = formName;
            VisualizationConfiguration = visualizationConfiguration;
            LocalizedLogger = localizedLogger;
        }

        protected IDictionary<string, T> ElementsForVisualization { get; }

        protected string FormName { get; }

        protected IVisualizationConfiguration VisualizationConfiguration { get; }

        protected ILocalizedLogger LocalizedLogger { get; }

        protected string DumpsDirectory => VisualizationConfiguration.PathToDumps;

        public virtual float CompareWithDump(string dumpName = null)
        {
            var directory = GetDumpDirectory(dumpName);
            LocalizedLogger.Info("loc.visualization.dump.compare", directory.Name);
            if (!directory.Exists)
            {
                throw new InvalidOperationException($"Dump directory [{directory.FullName}] does not exist.");
            }
            var imageFiles = directory.GetFiles($"*{ImageFormat}");
            if (imageFiles.Length == 0)
            {
                throw new InvalidOperationException($"Dump directory [{directory.FullName}] does not contain any [*{ImageFormat}] files.");
            }
            var existingElements = ElementsForVisualization.Where(element => element.Value.State.IsExist)
                .ToDictionary(el => el.Key, el => el.Value);
            var countOfUnproceededElements = existingElements.Count;
            var countOfProceededElements = 0;
            var comparisonResult = 0f;
            foreach (var imageFile in imageFiles)
            {
                var key = imageFile.Name.Replace(ImageFormat, string.Empty);
                if (!existingElements.ContainsKey(key))
                {
                    LocalizedLogger.Warn("loc.visualization.dump.keynotfound", key);
                    countOfUnproceededElements++;
                }
                else
                {
                    comparisonResult += existingElements[key].Visual.GetDifference(Image.FromFile(imageFile.FullName));
                    countOfUnproceededElements--;
                    countOfProceededElements++;
                }
            }
            // adding of countOfUnproceededElements means 100% difference for each element absent in dump or on page
            return (comparisonResult + countOfUnproceededElements) / (countOfProceededElements + countOfUnproceededElements);
        }

        public virtual void SaveDump(string dumpName = null)
        {
            var directory = CleanUpAndGetDumpDirectory(dumpName);
            ElementsForVisualization.Where(element => element.Value.State.IsExist).ToList()
                .ForEach(element => element.Value.Visual.Image.Save(Path.Combine(directory.FullName, $"{element.Key}.png")));
        }

        protected virtual DirectoryInfo CleanUpAndGetDumpDirectory(string dumpName = null)
        {
            var dirInfo = GetDumpDirectory(dumpName);
            if (dirInfo.Exists)
            {
                foreach (var file in dirInfo.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (var dir in dirInfo.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                dirInfo.Create();
            }

            return dirInfo;
        }

        protected virtual DirectoryInfo GetDumpDirectory(string dumpName = null)
        {
            const int maxNameLenght = 40;
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var name = dumpName ?? FormName;
            foreach (var character in invalid)
            {
                name = name.Replace(character, ' ');
            }
            name = name.Length > maxNameLenght ? name.Substring(0, maxNameLenght) : name;

            return new DirectoryInfo(Path.Combine(DumpsDirectory, name));
        }
    }
}
