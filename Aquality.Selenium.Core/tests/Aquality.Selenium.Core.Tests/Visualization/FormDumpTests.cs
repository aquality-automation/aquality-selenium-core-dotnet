using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Forms;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
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
            var pathToDump = CleanUAndGetpPathToDump(form.Name.Replace("/", " "));

            Assert.DoesNotThrow(() => form.Dump.Save());
            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);
            Assert.Greater(pathToDump.GetFiles().Length, 0, "Dump should contain some files");
        }

        [Test]
        public void Should_BePossibleTo_SaveFormDump_WithSubfoldersInName()
        {
            var form = new WebForm();
            var dumpName = $"{form.Name.Replace("/", " ")}\\SubFolder1\\SubFolder2";
            var pathToDump = CleanUAndGetpPathToDump(dumpName);

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
            var imageFormatLength = customForm.Dump.GetType().GetProperty("ImageFormat", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customForm.Dump).ToString().Length;
            var maxLength = (int)customForm.Dump.GetType().GetProperty("MaxFullFileNameLength", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customForm.Dump);
            var pathToDumpLength = PathToDumps.Length;

            var dumpName = new string('A', maxLength - pathToDumpLength - maxElementNameLength - imageFormatLength);
            var overlengthDumpName = dumpName + "_BCDE";

            var overlengthPathToDump = CleanUAndGetpPathToDump(overlengthDumpName);
            var pathToDump = CleanUAndGetpPathToDump(dumpName);

            Assert.DoesNotThrow(() => customForm.Dump.Save(overlengthDumpName));

            overlengthPathToDump.Refresh();
            DirectoryAssert.DoesNotExist(overlengthPathToDump);

            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);

            Assert.That(customForm.Dump.Compare(dumpName), Is.EqualTo(0), "Some elements should be failed to take image, but difference should be around zero");
        }

        private DirectoryInfo CleanUAndGetpPathToDump(string dumpName)
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
            private Label DisplayedLabel
                => new Label(DisplayedElementsLoc, "I'm displayed property", ElementState.Displayed);
            private Label HiddenLabel
                => new Label(HiddenElementsLoc, "I'm hidden", ElementState.ExistsInAnyState);
            private Label HiddenLabelInitializedAsDisplayed
                => new Label(HiddenElementsLoc, "I'm hidden but mask as displayed", ElementState.Displayed);

            private Label ContentLabel => new Label(ContentLoc, "Content", ElementState.Displayed);
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
    }
}
