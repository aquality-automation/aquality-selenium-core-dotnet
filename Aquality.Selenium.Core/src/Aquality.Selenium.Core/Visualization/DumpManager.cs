using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Aquality.Selenium.Core.Visualization
{
    public class DumpManager<T> : IDumpManager where T : IElement
    {
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
        protected string ImageFormat => VisualizationConfiguration.ImageFormat;
        protected int MaxFullFileNameLength => VisualizationConfiguration.MaxFullFileNameLength;

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
            var existingElements = FilterElementsForVisualization().ToDictionary(el => el.Key, el => el.Value);
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
            FilterElementsForVisualization()
                .ForEach(element =>
                {
                    try
                    {
                        element.Value.Visual.Image.Save(Path.Combine(directory.FullName, $"{element.Key}{ImageFormat}"));
                    }
                    catch (Exception e)
                    {
                        LocalizedLogger.Fatal("loc.form.dump.imagenotsaved", e, element.Key, e.Message);
                    }
                });
        }

        protected virtual List<KeyValuePair<string, T>> FilterElementsForVisualization()
        {
            return ElementsForVisualization.Where(element => element.Value.State.IsDisplayed).ToList();
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

        protected virtual int GetMaxNameLengthOfDumpElements() => ElementsForVisualization.Max(element => element.Key == null ? 0 : element.Key.Length);

        protected virtual DirectoryInfo GetDumpDirectory(string dumpName = null)
        {
            // get the maximum length of the name among the form elements for the dump
            var maxNameLengthOfDumpElements = GetMaxNameLengthOfDumpElements() + ImageFormat.Length;

            // get array of subfolders in dump name
            string[] dumpSubfoldersNames = (dumpName ?? FormName).Split('\\');

            // get invalid characters that can not be in folder name
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            // create new dump name without invalid chars for each subfolder
            StringBuilder validDumpName = new StringBuilder();
            foreach (string folderName in dumpSubfoldersNames)
            {
                string copy_folderName = folderName;
                foreach (var character in invalid)
                {
                    copy_folderName = copy_folderName.Replace(character, ' ');
                }
                validDumpName.Append($"{copy_folderName}\\");
            }
            string s_validDumpName = validDumpName.ToString();

            // create full dump path
            var fullDumpPath = Path.Combine(DumpsDirectory, s_validDumpName);

            // cut off the excess length and log warn message
            if (fullDumpPath.Length + maxNameLengthOfDumpElements > MaxFullFileNameLength)
            {
                s_validDumpName = s_validDumpName.Substring(0, MaxFullFileNameLength - Path.GetFullPath(DumpsDirectory).Length - maxNameLengthOfDumpElements);
                LocalizedLogger.Warn("loc.form.dump.exceededdumpname", s_validDumpName);
            }

            return new DirectoryInfo(Path.Combine(DumpsDirectory, s_validDumpName));
        }
    }
}
