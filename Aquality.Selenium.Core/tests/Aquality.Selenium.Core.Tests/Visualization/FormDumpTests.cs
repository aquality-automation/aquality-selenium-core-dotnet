using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Forms;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Aquality.Selenium.Core.Visualization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aquality.Selenium.Core.Tests.Visualization
{
    public class FormDumpTests : TestWithBrowser
    {

        private static readonly Uri HoversURL = new Uri($"{TestSite}/hovers");

        private readonly WebForm form = new WebForm();

        private string PathToDumps => AqualityServices.ServiceProvider.GetRequiredService<IVisualizationConfiguration>().PathToDumps;

        [SetUp]
        public new void SetUp()
        {
            AqualityServices.Application.Driver.Navigate().GoToUrl(HoversURL);
        }

        [Test]
        public void Should_BePossibleTo_SaveFormDump_WithCustomName()
        {
            var pathToDump = 
                new DirectoryInfo(Path.Combine(PathToDumps, form.Name.Replace("/", " ")));
            pathToDump.Delete(true);
            form.WaitUntilPresent();
            Assert.AreEqual(0, pathToDump.Exists ? pathToDump.GetFiles().Length : 0, "Dump directory should not contain any files before saving");
            Assert.DoesNotThrow(() => form.Dump.SaveDump());
            pathToDump.Refresh();
            DirectoryAssert.Exists(pathToDump);
            Assert.Greater(pathToDump.GetFiles().Length, 0, "Dump should contain some files");
        }

        private class WebForm : Form<WebElement>
        {
            private static readonly By ContentLoc = By.XPath("//div[contains(@class,'example')]");
            private static readonly By HiddenElementsLoc = By.XPath("//h5");
            private static readonly By DisplayedElementsLoc = By.XPath("//img[@alt='User Avatar']");

            private readonly Label displayedLabel = ElementFactory.GetCustomElement(
                (loc, name, state) => new Label(loc, name, state),
                DisplayedElementsLoc, "I'm displayed");
            private readonly Label displayedButInitializedAsExist
                = new Label(DisplayedElementsLoc, "I'm displayed but initialized as existing", ElementState.ExistsInAnyState);
            private Label DisplayedLabel
                => new Label(DisplayedElementsLoc, "I'm displayed", ElementState.Displayed);
            private Label HiddenLabel 
                => new Label(HiddenElementsLoc, "I'm hidden", ElementState.ExistsInAnyState);
            private Label HiddenLabelInitializedAsDisplayed 
                => new Label(HiddenElementsLoc, "I'm hidden but mask as displayed", ElementState.Displayed);

            private Label ContentLabel => new Label(ContentLoc, "Content", ElementState.Displayed);
            private Label ContentDuplicateLabel => new Label(ContentLoc, "Content", ElementState.Displayed);


            private IDictionary<string, WebElement> elementsToCheck = null;
            public override string Name => "Web page/form";
            private static IElementFactory ElementFactory => AqualityServices.ServiceProvider.GetRequiredService<IElementFactory>();

            public WebForm()
            {
            }

            public void WaitUntilPresent() => DisplayedLabel.State.WaitForClickable();

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
