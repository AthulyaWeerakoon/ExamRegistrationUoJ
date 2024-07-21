using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using System;
using OpenQA.Selenium.Support.UI;

namespace ExamRegistrationUoJ.Tests.AutomateUITest
{
    public class AdvisorHomeTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string _email = "sexymeoverallplatforms@outlook.com";
        private readonly string _password = "Sexyme12345";
        private readonly string _url = "http://hivesphere.software";

        public AdvisorHomeTests()
        {
            _driver = new ChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        }

        [Fact]
        public void TestAdvisorHomePage()
        {
            // Step 1: Log in
            _driver.Navigate().GoToUrl($"{_url}/Home/Login");
            var emailField = _wait.Until(d => d.FindElement(By.Id("email")));
            var passwordField = _driver.FindElement(By.Id("password"));
            var loginButton = _driver.FindElement(By.Id("loginButton"));

            emailField.SendKeys(_email);
            passwordField.SendKeys(_password);
            loginButton.Click();

            // Step 2: Navigate to Advisor Home page
            _wait.Until(d => d.FindElement(By.Id("advisorHomeLink"))).Click();

            // Step 3: Verify Advisor Home page elements
            var advisorHomeHeader = _wait.Until(d => d.FindElement(By.Id("advisorHomeHeader")));
            Assert.Equal("Advisor Home", advisorHomeHeader.Text);

            // Additional verifications can be added here
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
