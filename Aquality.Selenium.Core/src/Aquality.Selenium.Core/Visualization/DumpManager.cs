using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        public virtual float Compare(string dumpName = null)
        {
            var directory = GetDumpDirectory(dumpName);
            LocalizedLogger.Info("loc.form.dump.compare", directory.Name);
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
            var absentOnFormElementNames = new List<string>();
            foreach (var imageFile in imageFiles)
            {
                var key = imageFile.Name.Replace(ImageFormat, string.Empty);
                if (!existingElements.ContainsKey(key))
                {
                    LocalizedLogger.Warn("loc.form.dump.elementnotfound", key);
                    countOfUnproceededElements++;
                    absentOnFormElementNames.Add(key);
                }
                else
                {
                    comparisonResult += existingElements[key].Visual.GetDifference(Image.FromFile(imageFile.FullName));
                    countOfUnproceededElements--;
                    countOfProceededElements++;
                    existingElements.Remove(key);
                }
            }
            if (countOfUnproceededElements > 0)
            {
                if (existingElements.Any())
                {
                    LocalizedLogger.Warn("loc.form.dump.elementsmissedindump", string.Join(", ", existingElements.Keys));
                }
                if (absentOnFormElementNames.Any())
                {
                    LocalizedLogger.Warn("loc.form.dump.elementsmissedonform", string.Join(", ", absentOnFormElementNames));
                }
                LocalizedLogger.Warn("loc.form.dump.unprocessedelements", countOfUnproceededElements);
            }
            // adding of countOfUnproceededElements means 100% difference for each element absent in dump or on page
            var result = (comparisonResult + countOfUnproceededElements) / (countOfProceededElements + countOfUnproceededElements);
            LocalizedLogger.Info("loc.form.dump.compare.result", result.ToString("P", CultureInfo.InvariantCulture));
            return result;
        }

        public virtual void Save(string dumpName = null)
        {
            var directory = CleanUpAndGetDumpDirectory(dumpName);
            LocalizedLogger.Info("loc.form.dump.save", directory.Name);
            ElementsForVisualization.Where(element => element.Value.State.IsExist).ToList()
                .ForEach(element =>
                {
                    try
                    {
                        element.Value.Visual.Image.Save(Path.Combine(directory.FullName, $"{element.Key}.png"));
                    }
                    catch (Exception e)
                    {
                        LocalizedLogger.Fatal("loc.form.dump.imagenotsaved", e, element.Key, e.Message);
                    }
                });
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
                if (!Directory.Exists(DumpsDirectory))
                {
                    Directory.CreateDirectory(DumpsDirectory);
                }
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
