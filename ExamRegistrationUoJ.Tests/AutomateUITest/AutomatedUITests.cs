using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using Xunit;
using System;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace ExamRegistrationUoJ.Tests.AutomateUITest
{
    public class AutomatedUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string _screenshotsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");

        public AutomatedUITests()
        {
            // Initialize the ChromeDriver. Ensure ChromeDriver.exe is in your path or specify its location.
            _driver = new ChromeDriver(); // You can use other drivers like FirefoxDriver or EdgeDriver.
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));

            // Create screenshots directory if it doesn't exist
            if (!Directory.Exists(_screenshotsDirectory))
            {
                Directory.CreateDirectory(_screenshotsDirectory);
            }
        }

        private void CaptureScreenshot(string testName)
        {
            try
            {
                Screenshot screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                string filePath = Path.Combine(_screenshotsDirectory, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                Console.WriteLine($"Screenshot saved: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing screenshot: {ex.Message}");
            }
        }

        [Fact]
        public void TestHomePageTitle()
        {
            try
            {
                _driver.Navigate().GoToUrl("http://hivesphere.software/home");
                var title = _driver.Title;
                Assert.Equal("Home", title);
            }
            catch (Exception ex)
            {
                CaptureScreenshot(nameof(TestHomePageTitle));
                throw;
            }
        }

        [Fact]
        public void TestSearchFunctionality()
        {
            try
            {
                _driver.Navigate().GoToUrl("http://hivesphere.software/home");

                var searchBox = _wait.Until(driver => driver.FindElement(By.Id("searchBox")));
                var searchButton = _wait.Until(driver => driver.FindElement(By.Id("searchButton")));

                searchBox.SendKeys("About");
                searchButton.Click();

                var searchResult = _wait.Until(driver => driver.FindElement(By.Id("searchResult"))).Text;
                Assert.Contains("About", searchResult);
            }
            catch (Exception ex)
            {
                CaptureScreenshot(nameof(TestSearchFunctionality));
                throw;
            }
        }

        [Fact]
        public void TestInitialSearchResult()
        {
            try
            {
                _driver.Navigate().GoToUrl("http://hivesphere.software/home");

                var searchResult = _wait.Until(driver => driver.FindElement(By.Id("searchResult"))).Text;
                Assert.Equal("About", searchResult);
            }
            catch (Exception ex)
            {
                CaptureScreenshot(nameof(TestInitialSearchResult));
                throw;
            }
        }

        [Fact]
        public void TestLoginButton()
        {
            try
            {
                _driver.Navigate().GoToUrl("http://hivesphere.software/home");

                var loginButton = _wait.Until(driver => driver.FindElement(By.Id("loginButton")));
                loginButton.Click();

                var loggedInElement = _wait.Until(driver => driver.FindElement(By.Id("welcomeMessage")));
                Assert.True(loggedInElement.Displayed);
            }
            catch (Exception ex)
            {
                CaptureScreenshot(nameof(TestLoginButton));
                throw;
            }
        }

        [Fact]
        public void TestAboutButton()
        {
            try
            {
                _driver.Navigate().GoToUrl("http://hivesphere.software/home");

                var aboutButton = _wait.Until(driver => driver.FindElement(By.Id("aboutButton")));
                aboutButton.Click();

                var aboutPageTitle = _driver.Title;
                Assert.Equal("About Us", aboutPageTitle);
            }
            catch (Exception ex)
            {
                CaptureScreenshot(nameof(TestAboutButton));
                throw;
            }
        }

        [Fact]
        public void TestContactButton()
        {
            try
            {
                _driver.Navigate().GoToUrl("http://hivesphere.software/home");

                var contactButton = _wait.Until(driver => driver.FindElement(By.Id("contactButton")));
                contactButton.Click();

                var contactPageTitle = _driver.Title;
                Assert.Equal("Contact Us", contactPageTitle);
            }
            catch (Exception ex)
            {
                CaptureScreenshot(nameof(TestContactButton));
                throw;
            }
        }

        public void Dispose()
        {
            _driver.Quit(); // Close the browser and clean up resources
        }
    }
}
