using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace IdeaCenterSeleniumTesting
{
    [TestFixture]
    public class IdeaCenterTests
    {
        private IWebDriver driver;
        protected static readonly string baseURL = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:83";
        protected readonly string email = "iv1@test.com";
        protected readonly string password = "123456";


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(chromeOptions);

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait=TimeSpan.FromSeconds(10); 

            driver.Navigate().GoToUrl($"{baseURL}/Users/Login");
            driver.FindElement(By.XPath("//input[@name='Email']")).SendKeys(email);
            driver.FindElement(By.Id("typePasswordX-2")).SendKeys("123456");
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg btn-block']")).Click();

        }

        [OneTimeTearDown]
        
        public void OneTimeTearDown ()
        {
            driver.Quit();
            driver.Dispose();
        }

       [Test, Order(1)]
       public void Create_Idea_With_Invalid_Data_Test()
       {
            driver.Navigate().GoToUrl($"{baseURL}/Ideas/Create");

            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']//ul//li")).Text;

            Assert.That(errorMessage, Is.EqualTo("Unable to create new Idea!"));

        }
    }
}