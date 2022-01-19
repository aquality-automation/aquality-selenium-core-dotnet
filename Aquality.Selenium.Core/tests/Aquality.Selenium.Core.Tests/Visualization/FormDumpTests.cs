﻿using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Forms;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Aquality.Selenium.Core.Visualization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WebElement = Aquality.Selenium.Core.Tests.Applications.Browser.Elements.WebElement;

namespace Aquality.Selenium.Core.Tests.Visualization
{
    public class FormDumpTests : TestWithBrowser
    {
        private static readonly Uri HoversURL = new Uri($"{TestSite}/hovers");

        private string PathToDumps => AqualityServices.ServiceProvider.GetRequiredService<IVisualizationConfiguration>().PathToDumps;

        [SetUp]
        public new void SetUp()
        {
            var form = new WebForm();
            AqualityServices.Application.Driver.Navigate().GoToUrl(HoversURL);
            form.ClickOnContent();
            form.WaitUntilPresent();
        }

        [Test]
        public void Should_BePossibleTo_SaveFormDump_WithDefaultName()
        {
            var form = new WebForm();
            var pathToDump = CleanUpAndGetPathToDump(form.Name.Replace("/", " "));

            Assert.DoesNotThrow(() => form.Dump.Save());
            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);
            Assert.Greater(pathToDump.GetFiles().Length, 0, "Dump should contain some files");
        }

        [Test]
        public void Should_BePossibleTo_SaveFormDump_WithSubfoldersInName()
        {
            var form = new WebForm();
            var dumpName = $"SubFolder1\\SubFolder2";
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.DoesNotThrow(() => form.Dump.Save(dumpName));
            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);
            Assert.Greater(pathToDump.GetFiles().Length, 0, "Dump should contain some files");
        }

        [Test]
        public void Should_BePossibleTo_CompareWithDump_WithCustomName_WhenDifferenceIsZero()
        {
            var form = new WebForm();
            form.Dump.Save("Zero diff");
            Assert.That(form.Dump.Compare("Zero diff"), Is.EqualTo(0), "Difference with current page should be around zero");
        }

        [Test]
        public void Should_BePossibleTo_CompareWithDump_WithCustomName_WhenDifferenceIsNotZero()
        {
            var form = new WebForm();
            form.HoverAvatar();
            form.Dump.Save("Non-zero diff");
            AqualityServices.Application.Driver.Navigate().Refresh();
            form.WaitUntilPresent();
            Assert.That(form.Dump.Compare("Non-zero diff"), Is.GreaterThan(0), "Difference with current page should be greater than zero");
        }

        [Test]
        public void Should_BePossibleTo_CompareWithDump_WithCustomName_WhenElementSetDiffers()
        {
            var customForm = new WebForm();
            customForm.Dump.Save("Set differs");
            customForm.SetElementsForDump(WebForm.ElementsFilter.ElementsInitializedAsDisplayed);
            Assert.That(customForm.Dump.Compare("Set differs"), Is.GreaterThan(0), "Difference with current page should be greater than zero if element set differs");
        }

        [Test]
        public void Should_BePossibleTo_SaveAndCompareWithDump_WithCustomName_WhenAllElementsSelected()
        {
            var customForm = new WebForm();
            customForm.SetElementsForDump(WebForm.ElementsFilter.AllElements);
            Assert.DoesNotThrow(() => customForm.Dump.Save("All elements"));
            Assert.That(customForm.Dump.Compare("All elements"), Is.EqualTo(0), "Some elements should be failed to take image, but difference should be around zero");
        }

        [Test]
        public void Should_BePossibleTo_SaveAndCompareWithDump_WithOverlengthDumpName_WhenAllElementsSelected()
        {
            var customForm = new WebForm();
            customForm.SetElementsForDump(WebForm.ElementsFilter.AllElements);

            var maxElementNameLength = (int)customForm.Dump.GetType().GetMethod("GetMaxNameLengthOfDumpElements", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(customForm.Dump, new object[] { });
            var imageExtensioLength = ((ImageFormat)customForm.Dump.GetType().GetProperty("ImageFormat", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customForm.Dump)).Extension.Length;
            var maxLength = (int)customForm.Dump.GetType().GetProperty("MaxFullFileNameLength", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customForm.Dump);
            var pathToDumpLength = PathToDumps.Length;

            var dumpName = new string('A', maxLength - pathToDumpLength - maxElementNameLength - imageExtensioLength);
            var overlengthDumpName = dumpName + "_BCDE";

            var overlengthPathToDump = CleanUpAndGetPathToDump(overlengthDumpName);
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.DoesNotThrow(() => customForm.Dump.Save(overlengthDumpName));

            overlengthPathToDump.Refresh();
            DirectoryAssert.DoesNotExist(overlengthPathToDump);

            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);

            Assert.That(customForm.Dump.Compare(dumpName), Is.EqualTo(0), "Some elements should be failed to take image, but difference should be around zero");
        }

        [TestCase(".bmp")]
        [TestCase(".emf")]
        [TestCase(".exif")]
        [TestCase(".gif")]
        [TestCase(".ico")]
        [TestCase(".jpg")]
        [TestCase(".jpeg")]
        [TestCase(".memorybmp")]
        [TestCase(".png")]
        [TestCase(".tif")]
        [TestCase(".tiff")]
        [TestCase(".wmf")]
        public void Should_BePossibleTo_SaveFormDump_WithValidExtension(string imageExtension)
        {
            var form = new LiteWebForm(imageExtension);
            var dumpName = $"Test {imageExtension} extension";
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.DoesNotThrow(() => form.Dump.Save(dumpName));
            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);
            Assert.Greater(pathToDump.GetFiles().Length, 0, "Dump should contain some files");

            foreach (var file in pathToDump.GetFiles())
            {
                Assert.AreEqual(imageExtension, file.Extension, "Image extension not exual to expexted");
                Assert.That(file.Name.Contains(imageExtension), "Image name doesn't contain expected extension");
            }
        }

        [TestCase(".abc")]
        [TestCase("")]
        public void Should_BeImpossibleTo_SaveFormDump_WithInvalidExtension(string imageExtension)
        {
            var form = new LiteWebForm(imageExtension);
            var dumpName = $"Test {imageExtension} extension";
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.Throws<NotSupportedException>(() => form.Dump.Save(dumpName));

            pathToDump.Refresh();
            DirectoryAssert.DoesNotExist(pathToDump);
        }

        private DirectoryInfo CleanUpAndGetPathToDump(string dumpName)
        {
            var pathToDump = new DirectoryInfo(Path.Combine(PathToDumps, dumpName));
            if (pathToDump.Exists)
            {
                pathToDump.Delete(true);
                pathToDump.Refresh();
            }
            Assert.AreEqual(0, pathToDump.Exists ? pathToDump.GetFiles().Length : 0, "Dump directory should not contain any files before saving");
            return pathToDump;
        }

        private class WebForm : Form<WebElement>
        {
            private static readonly By ContentLoc = By.XPath("//div[contains(@class,'example')]");
            private static readonly By HiddenElementsLoc = By.XPath("//h5");
            private static readonly By DisplayedElementsLoc = By.XPath("//div[contains(@class,'figure')]");

            private readonly Label displayedLabel = ElementFactory.GetCustomElement(
                (loc, name, state) => new Label(loc, name, state),
                DisplayedElementsLoc, "I'm displayed field");
            private readonly Label displayedButInitializedAsExist
                = new Label(DisplayedElementsLoc, "I'm displayed but initialized as existing", ElementState.ExistsInAnyState);
            protected Label DisplayedLabel
                => new Label(DisplayedElementsLoc, "I'm displayed property", ElementState.Displayed);
            private Label HiddenLabel
                => new Label(HiddenElementsLoc, "I'm hidden", ElementState.ExistsInAnyState);
            private Label HiddenLabelInitializedAsDisplayed
                => new Label(HiddenElementsLoc, "I'm hidden but mask as displayed", ElementState.Displayed);

            protected Label ContentLabel => new Label(ContentLoc, "Content", ElementState.Displayed);
            private Label ContentDuplicateLabel => new Label(ContentLoc, "Content", ElementState.Displayed);


            private IDictionary<string, WebElement> elementsToCheck = null;
            public override string Name => "Web page/form";
            private static IElementFactory ElementFactory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

            public void ClickOnContent()
            {
                ContentLabel.Click();
            }

            public void WaitUntilPresent()
            {
                DisplayedLabel.State.WaitForClickable();
            }

            public void HoverAvatar()
            {
                AqualityServices.ServiceProvider.GetRequiredService<Logger>().Info("Hovering avatar");
                new Actions(AqualityServices.Application.Driver)
                    .MoveToElement(DisplayedLabel.GetElement())
                    .ClickAndHold()
                    .Build().Perform();
                HiddenLabel.State.WaitForDisplayed();
            }

            public void SetElementsForDump(ElementsFilter filter)
            {
                switch (filter)
                {
                    case ElementsFilter.ElementsInitializedAsDisplayed:
                        elementsToCheck = ElementsInitializedAsDisplayed;
                        break;
                    case ElementsFilter.AllElements:
                        elementsToCheck = AllElements;
                        break;
                    case ElementsFilter.DisplayedElements:
                        elementsToCheck = DisplayedElements;
                        break;
                }
            }

            protected override IDictionary<string, WebElement> ElementsForVisualization
            {
                get
                {
                    if (elementsToCheck == null)
                    {
                        elementsToCheck = base.ElementsForVisualization;
                    }
                    return elementsToCheck;
                }
            }

            protected override IVisualizationConfiguration VisualizationConfiguration => AqualityServices.ServiceProvider.GetRequiredService<IVisualizationConfiguration>();

            protected override ILocalizedLogger LocalizedLogger => AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>();

            public enum ElementsFilter
            {
                ElementsInitializedAsDisplayed,
                AllElements,
                DisplayedElements
            }
        }

        private class LiteWebForm : WebForm
        {
            private readonly string ImageExtension;

            protected override IVisualizationConfiguration VisualizationConfiguration => new CustomVisualizationConfiguration(ImageExtension);

            public LiteWebForm(string imageExtension)
            {
                ImageExtension = imageExtension;
            }

            private class CustomVisualizationConfiguration : VisualizationConfiguration
            {
                private readonly string imageFormat;
                public override ImageFormat ImageFormat => ImageFormat.Parse(imageFormat);

                public CustomVisualizationConfiguration(string format) : base(AqualityServices.ServiceProvider.GetRequiredService<ISettingsFile>())
                {
                    imageFormat = format;
                }
            }
        }
    }
}
