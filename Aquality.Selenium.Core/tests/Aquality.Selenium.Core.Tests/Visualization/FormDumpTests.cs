using Aquality.Selenium.Core.Configurations;
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
        private static readonly Uri HoversURL = new($"{TestSite}/hovers");

        private static string PathToDumps => AqualityServices.ServiceProvider.GetRequiredService<IVisualizationConfiguration>().PathToDumps;

        [SetUp]
        public new void SetUp()
        {
            var form = new WebForm();
            GoToUrl(HoversURL);
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
            Assert.That(pathToDump, Does.Exist);
            Assert.That(pathToDump.GetFiles().Length, Is.GreaterThan(0), "Dump should contain some files");
        }

        [Test]
        public void Should_BePossibleTo_SaveFormDump_WithSubfoldersInName()
        {
            var form = new WebForm();
            var dumpName = $"SubFolder1{Path.DirectorySeparatorChar}SubFolder2";
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.DoesNotThrow(() => form.Dump.Save(dumpName));
            pathToDump.Refresh();
            Assert.That(pathToDump, Does.Exist);
            Assert.That(pathToDump.GetFiles().Length, Is.GreaterThan(0), "Dump should contain some files");
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
            form.Dump.Save("Non-zero diff");
            AqualityServices.Application.Driver.Navigate().Refresh();
            form.WaitUntilPresent();
            form.HoverAvatar();
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
        public void Should_BePossibleTo_SaveAndCompareWithDump_WithCustomName_WhenCurrentFormElementsSelected()
        {
            var customForm = new WebForm();
            customForm.SetElementsForDump(WebForm.ElementsFilter.CurrentFormElements);
            Assert.DoesNotThrow(() => customForm.Dump.Save("CurrentForm elements"));
            Assert.That(customForm.Dump.Compare("CurrentForm elements"), Is.EqualTo(0), "Some elements should be failed to take image, but difference should be around zero");
        }

        [Test]
        public void Should_BePossibleTo_SaveAndCompareWithDump_WithOverlengthDumpName_WhenAllElementsSelected()
        {
            var customForm = new WebForm();
            customForm.SetElementsForDump(WebForm.ElementsFilter.AllElements);

            var maxElementNameLength = (int)customForm.Dump.GetType().GetMethod("GetMaxNameLengthOfDumpElements", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(customForm.Dump, []);
            var imageExtensionLength = ((ImageFormat)customForm.Dump.GetType().GetProperty("ImageFormat", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customForm.Dump)).Extension.Length;
            var maxLength = (int)customForm.Dump.GetType().GetProperty("MaxFullFileNameLength", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customForm.Dump);
            var pathToDumpLength = PathToDumps.Length;

            var dumpName = new string('A', maxLength - pathToDumpLength - maxElementNameLength - imageExtensionLength);
            var overlengthDumpName = dumpName + "_BCDE";

            var overlengthPathToDump = CleanUpAndGetPathToDump(overlengthDumpName);
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.DoesNotThrow(() => customForm.Dump.Save(overlengthDumpName));

            overlengthPathToDump.Refresh();
            Assert.That(overlengthPathToDump, Does.Not.Exist);

            pathToDump.Refresh();
            Assert.That(pathToDump, Does.Exist);

            Assert.That(customForm.Dump.Compare(dumpName), Is.EqualTo(0), "Some elements should be failed to take image, but difference should be around zero");
        }

        [TestCase(".bmp")]
        [TestCase(".heif")]
        [TestCase(".avif")]
        [TestCase(".gif")]
        [TestCase(".ico")]
        [TestCase(".jpg")]
        [TestCase(".jpeg")]
        [TestCase(".wbmp")]
        [TestCase(".astc")]
        [TestCase(".dng")]
        [TestCase(".ktx")]
        [TestCase(".webp")]
        [TestCase(".pkm")]
        public void Should_BePossibleTo_SaveFormDump_WithValidExtension(string imageExtension)
        {
            var form = new LiteWebForm(imageExtension);
            var dumpName = $"Test {imageExtension} extension";
            var pathToDump = CleanUpAndGetPathToDump(dumpName);

            Assert.DoesNotThrow(() => form.Dump.Save(dumpName));
            pathToDump.Refresh();
            Assert.That(pathToDump, Does.Exist);
            Assert.That(pathToDump.GetFiles().Length, Is.GreaterThan(0), "Dump should contain some files");

            foreach (var file in pathToDump.GetFiles())
            {
                Assert.That(file.Extension, Is.EqualTo(imageExtension), "Image extension not equal to expected");
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
            Assert.That(pathToDump, Does.Not.Exist);
        }

        private static DirectoryInfo CleanUpAndGetPathToDump(string dumpName)
        {
            var pathToDump = new DirectoryInfo(Path.Combine(PathToDumps, dumpName));
            if (pathToDump.Exists)
            {
                pathToDump.Delete(true);
                pathToDump.Refresh();
            }
            Assert.That(pathToDump.Exists ? pathToDump.GetFiles().Length : 0, Is.EqualTo(0), "Dump directory should not contain any files before saving");
            return pathToDump;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Needed for proper dump collection")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needed for proper dump collection")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Needed for proper dump collection")]
        private class WebForm : Form<WebElement>
        {
            private static readonly By ContentLoc = By.XPath("//div[contains(@class,'example')]");
            private static readonly By HiddenElementsLoc = By.XPath("//h5");
            private static readonly By DisplayedElementsLoc = By.XPath("//div[contains(@class,'figure')]");

            private readonly Label displayedLabel = ElementFactory.GetCustomElement(
                (loc, name, state) => new Label(loc, name, state),
                DisplayedElementsLoc, "I'm displayed field");
            private readonly Label displayedButInitializedAsExist
                = new(DisplayedElementsLoc, "I'm displayed but initialized as existing", ElementState.ExistsInAnyState);
            protected Label DisplayedLabel
                => new(DisplayedElementsLoc, "I'm displayed property", ElementState.Displayed);
            private Label HiddenLabel
                => new(HiddenElementsLoc, "I'm hidden", ElementState.ExistsInAnyState);
            private Label HiddenLabelInitializedAsDisplayed
                => new(HiddenElementsLoc, "I'm hidden but mask as displayed", ElementState.Displayed);

            protected Label ContentLabel => new(ContentLoc, "Content", ElementState.Displayed);
            private Label ContentDuplicateLabel => new(ContentLoc, "Content", ElementState.Displayed);


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
                    case ElementsFilter.CurrentFormElements:
                        elementsToCheck = AllCurrentFormElements;
                        break;
                }
            }

            protected override IDictionary<string, WebElement> ElementsForVisualization
            {
                get
                {
                    elementsToCheck ??= base.ElementsForVisualization;
                    return elementsToCheck;
                }
            }

            protected override IVisualizationConfiguration VisualizationConfiguration => AqualityServices.ServiceProvider.GetRequiredService<IVisualizationConfiguration>();

            protected override ILocalizedLogger LocalizedLogger => AqualityServices.ServiceProvider.GetRequiredService<ILocalizedLogger>();

            public enum ElementsFilter
            {
                ElementsInitializedAsDisplayed,
                AllElements,
                DisplayedElements,
                CurrentFormElements
            }
        }

        private class LiteWebForm(string imageExtension) : WebForm
        {
            private readonly string ImageExtension = imageExtension;

            protected override IVisualizationConfiguration VisualizationConfiguration => new CustomVisualizationConfiguration(ImageExtension);

            private class CustomVisualizationConfiguration(string format) : VisualizationConfiguration(AqualityServices.ServiceProvider.GetRequiredService<ISettingsFile>())
            {
                private readonly string imageFormat = format;
                public override ImageFormat ImageFormat => ImageFormat.Parse(imageFormat);
            }
        }
    }
}
