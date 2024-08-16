using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace Movie_Catalog_Selenium_Tests
{
    [TestFixture]
    public class MovieCatalogTests
    {
        protected IWebDriver driver;
        private static readonly string BaseUrl = "http://moviecatalog-env.eba-ubyppecf.eu-north-1.elasticbeanstalk.com/";
        private static string? lastMovieTitle;
        private static string? lastMovieDescription;
        private Actions actions;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddArgument("--disable-search-engine-choice-screen");
            driver = new ChromeDriver();

            actions = new Actions(driver);

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait=TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl(BaseUrl);
            driver.FindElement(By.XPath("//a[text()='Login']")).Click();

            var loginButton=driver.FindElement(By.XPath("//a[@class='btn warning']"));
            
            actions.MoveToElement(loginButton).Click().Perform();

            driver.FindElement(By.XPath("//input[@id='form2Example17']")).SendKeys("iv@test.com");
            driver.FindElement(By.XPath("//input[@id='form2Example27']")).SendKeys("123456");
            driver.FindElement(By.XPath("//button[@class='btn warning']")).Click();
        }

        [OneTimeTearDown]

        public void OneTimeTearDown()
        {
            driver.Quit();
            driver.Dispose();
        }


        [Test, Order(1)]
        public void Add_Movie_Without_Title_Test()
        {
             driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Add");
            //driver.FindElement(By.XPath("//a[text()='Add Movie']")).Click();
            var title ="";
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(title);
            var buttonAdd=driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(buttonAdd).Click().Perform();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            Assert.That(errorMessage, Is.EqualTo("The Title field is required."), "The title error message is not displayed as expected.");
        }
        [Test, Order(2)]

        public void Add_Movie_Without_Description_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Add");
            var title = GenerateRandomString(5);
            var description = "";
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(title);
            driver.FindElement(By.XPath("//textarea[@name='Description']")).SendKeys(description);
            var buttonAdd = driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(buttonAdd).Click().Perform();

            var errorMessageDescription = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            Assert.That(errorMessageDescription, Is.EqualTo("The Description field is required."), "The description error message is " +
                "not displayed as expected.");
        }
        [Test, Order(3)]

        public void Add_Movie_With_Random_Title_Test()
        {
            lastMovieTitle = GenerateRandomString(5);
            lastMovieDescription = GenerateRandomString(10);
            driver.Navigate().GoToUrl($"{BaseUrl}Catalog/Add");
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(lastMovieTitle);
            driver.FindElement(By.XPath("//textarea[@name='Description']")).SendKeys(lastMovieDescription);
            var buttonAdd = driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(buttonAdd).Click().Perform();

            var pages = driver.FindElements(By.XPath("//a[@class='page-link']"));
            var lastPage = pages.Last();
            lastPage.Click();

            var moviesAdded= driver.FindElements(By.CssSelector(".col-lg-4"));
            var lastMovie=moviesAdded.Last();

            var titleLast = lastMovie.FindElement(By.XPath("h2"));

            Assert.That(titleLast.Text.Trim(), Is.EqualTo(lastMovieTitle), "last movie title was not correct");

        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}