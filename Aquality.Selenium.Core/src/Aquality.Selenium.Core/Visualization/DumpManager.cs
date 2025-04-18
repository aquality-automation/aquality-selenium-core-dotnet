﻿using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Localization;
using System;
using System.Collections.Generic;
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

        protected ImageFormat ImageFormat => VisualizationConfiguration.ImageFormat;

        protected int MaxFullFileNameLength => VisualizationConfiguration.MaxFullFileNameLength;

        protected string DumpsDirectory => VisualizationConfiguration.PathToDumps;

        public virtual float Compare(string dumpName = null)
        {
            var directory = GetDumpDirectory(dumpName);
            LocalizedLogger.Info("loc.form.dump.compare", directory.Name);
            var imageFiles = GetImageFiles(directory);
            var existingElements = FilterElementsForVisualization().ToDictionary(el => el.Key, el => el.Value);
            var countOfUnproceededElements = existingElements.Count;
            var countOfProceededElements = 0;
            var comparisonResult = 0f;
            var absentOnFormElementNames = new List<string>();
            foreach (var imageFile in imageFiles)
            {
                var key = imageFile.Name.Replace(ImageFormat.Extension, string.Empty);
                if (!existingElements.ContainsKey(key))
                {
                    LocalizedLogger.Warn("loc.form.dump.elementnotfound", key);
                    countOfUnproceededElements++;
                    absentOnFormElementNames.Add(key);
                }
                else
                {
                    comparisonResult += existingElements[key].Visual.GetDifference(imageFile.ReadImage());
                    countOfUnproceededElements--;
                    countOfProceededElements++;
                    existingElements.Remove(key);
                }
            }
            LogUnproceededElements(countOfUnproceededElements, existingElements, absentOnFormElementNames);
            // adding of countOfUnproceededElements means 100% difference for each element absent in dump or on page
            var result = (comparisonResult + countOfUnproceededElements) / (countOfProceededElements + countOfUnproceededElements);
            LocalizedLogger.Info("loc.form.dump.compare.result", result.ToString("P", CultureInfo.InvariantCulture));
            return result;
        }

        private void LogUnproceededElements(int countOfUnproceededElements, IDictionary<string, T> existingElements, IList<string> absentOnFormElementNames)
        {
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
        }

        private FileInfo[] GetImageFiles(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                throw new InvalidOperationException($"Dump directory [{directory.FullName}] does not exist.");
            }
            var imageFiles = directory.GetFiles($"*{ImageFormat.Extension}");
            if (imageFiles.Length == 0)
            {
                throw new InvalidOperationException($"Dump directory [{directory.FullName}] does not contain any [*{ImageFormat.Extension}] files.");
            }
            return imageFiles;
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
                        element.Value.Visual.Image.Save(Path.Combine(directory.FullName, $"{element.Key}{ImageFormat.Extension}"), ImageFormat.Format);
                    }
                    catch (Exception e)
                    {
                        LocalizedLogger.Fatal("loc.form.dump.imagenotsaved", e, element.Key, e.Message);
                    }
                });
        }

        protected virtual List<KeyValuePair<string, T>> FilterElementsForVisualization()
        {
            return ElementsForVisualization.Where(element => element.Value.State.IsDisplayed && element.Value.Visual.Size.Width > 0).ToList();
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
            var maxNameLengthOfDumpElements = GetMaxNameLengthOfDumpElements() + ImageFormat.Extension.Length;

            // get array of subfolders in dump name
            var dumpSubfoldersNames = (dumpName ?? FormName).Split(Path.DirectorySeparatorChar);

            // get invalid characters that can not be in folder name
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            // create new dump name without invalid chars for each subfolder
            var validDumpName = new StringBuilder();
            foreach (string folderName in dumpSubfoldersNames)
            {
                string folderNameCopy = folderName;
                foreach (var character in invalid)
                {
                    folderNameCopy = folderNameCopy.Replace(character, ' ');
                }
                validDumpName.Append($"{folderNameCopy}{Path.DirectorySeparatorChar}");
            }
            var validDumpNameString = validDumpName.ToString();

            // create full dump path
            var fullDumpPath = Path.Combine(DumpsDirectory, validDumpNameString);

            // cut off the excess length and log warn message
            if (fullDumpPath.Length + maxNameLengthOfDumpElements > MaxFullFileNameLength)
            {
                validDumpNameString = validDumpNameString.Substring(0, MaxFullFileNameLength - Path.GetFullPath(DumpsDirectory).Length - maxNameLengthOfDumpElements);
                fullDumpPath = Path.Combine(DumpsDirectory, validDumpNameString);
                LocalizedLogger.Warn("loc.form.dump.exceededdumpname", fullDumpPath);
            }

            return new DirectoryInfo(fullDumpPath);
        }
    }
}
