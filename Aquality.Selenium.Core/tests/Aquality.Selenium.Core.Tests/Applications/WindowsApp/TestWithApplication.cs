using NUnit.Framework;

namespace Aquality.Selenium.Core.Tests.Applications.WindowsApp
{
    [NonParallelizable]
    public class TestWithApplication
    {
        [TearDown]
        public void CleanUp()
        {
            if (AqualityServices.IsApplicationStarted)
            {
                AqualityServices.Application.Driver.Quit();
            }
        }
    }
}
