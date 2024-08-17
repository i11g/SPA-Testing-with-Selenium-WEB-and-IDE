using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace MovieCatalogSelenitmTests_II
{
    [TestFixture]
    public class MovieCatalogSeleniumTests
    {
        protected IWebDriver driver;
        private static readonly string BaseUrl = "http://moviecatalog-env.eba-ubyppecf.eu-north-1.elasticbeanstalk.com";
        private static string lastMovieTitle = "";
        private static string lastMovieDescription = "";
       Actions actions ;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl(BaseUrl);
            driver.FindElement(By.XPath("//a[text()='Login']")).Click();
            actions = new Actions(driver);
            var loginButton = driver.FindElement(By.XPath("//a[@id='loginBtn']"));
            actions.MoveToElement(loginButton).Click().Perform();

            driver.FindElement(By.XPath("//input[@id='form2Example17']")).SendKeys("iv@test.com");
            driver.FindElement(By.XPath("//input[@id='form2Example27']")).SendKeys("123456");
            driver.FindElement(By.XPath("//button[@class='btn warning']")).Click();

        }
        [OneTimeTearDown] 

        public void OneTimeTearDown ()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test, Order(1)]
        public void Add_Movie_Without_Title_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Catalog/Add");

            var title ="";

            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(title);
            var addButton=driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(addButton).Click().Perform();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            Assert.That(errorMessage, Is.EqualTo("The Title field is required."), "The error message is not correct");

        }
        [Test, Order(2)]
        public void Add_Movie_Without_Description_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Catalog/Add");

            var title = GenerateRandomString(5);
            var description ="";

            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(title);
            driver.FindElement(By.XPath("//textarea[@name='Description']")).Clear();
            driver.FindElement(By.XPath("//textarea[@name='Description']")).SendKeys(description);
            var addButton = driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(addButton).Click().Perform();

            var errorMessage = driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            Assert.That(errorMessage, Is.EqualTo("The Description field is required."), "The error message is not correct");
        }
        [Test, Order(3)]
        public void Add_Movie_With_RandomTitle_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Catalog/Add");

            lastMovieTitle = GenerateRandomString(5);
            lastMovieDescription = GenerateRandomString(10);

            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(lastMovieTitle);
            driver.FindElement(By.XPath("//textarea[@name='Description']")).Clear();
            driver.FindElement(By.XPath("//textarea[@name='Description']")).SendKeys(lastMovieDescription);
            var addButton = driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(addButton).Click().Perform();

            var pagination = driver.FindElements(By.XPath("//ul[@class='pagination']//li"));
            var lastPageItem = pagination.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastpage = lastPageItem.FindElement(By.XPath(".//a"));
            lastpage.Click();

            var movies = driver.FindElements(By.XPath("//div[@class='col-lg-4']"));
            var lastMovie = movies.Last();
            actions.MoveToElement(lastMovie).Perform();

            var titleMovie = lastMovie.FindElement(By.XPath(".//h2"));

            Assert.That(titleMovie.Text.Trim(), Is.EqualTo(lastMovieTitle), "The movie title is not correct");
        }

        [Test, Order(4)] 
        public void Edit_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Catalog/All");

            var pagination = driver.FindElements(By.XPath("//ul[@class='pagination']//li"));
            var lastPageItem = pagination.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastpage = lastPageItem.FindElement(By.XPath(".//a"));
            lastpage.Click();

            var movies = driver.FindElements(By.XPath("//div[@class='col-lg-4']"));
            var lastMovie = movies.Last();

            var editButton = lastMovie.FindElement(By.XPath(".//a[@class='btn btn-outline-success']"));
            actions.MoveToElement(editButton).Click().Perform();

            lastMovieTitle ="EDITED:" + lastMovieTitle;
            
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var title = wait.Until(driver => driver.FindElement(By.XPath("//input[@id='form2Example17' and @name='Title']")));
            title.Clear();
            title.SendKeys(lastMovieTitle);
            var addButton = driver.FindElement(By.XPath("//button[@class='btn warning']"));
            actions.MoveToElement(addButton).Click().Perform(); 

            var message= driver.FindElement(By.XPath("//div[@class='toast-message']")).Text;

            Assert.That(message, Is.EqualTo("The Movie is edited successfully!"), "The movie was not edited");
        }
        [Test, Order(5)]
        public void Mark_Last_Added_Movie_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Catalog/All");

            var pagination = driver.FindElements(By.XPath("//ul[@class='pagination']//li"));
            var lastPageItem = pagination.Last();
            actions.MoveToElement(lastPageItem).Perform();

            var lastpage = lastPageItem.FindElement(By.XPath(".//a"));
            lastpage.Click();

            var movies = driver.FindElements(By.XPath("//div[@class='col-lg-4']"));
            var lastMovie = movies.Last();

            var markButton = lastMovie.FindElement(By.XPath(".//a[@class='btn btn-info']"));
            actions.MoveToElement(markButton).Click().Perform();

            driver.Navigate().GoToUrl($"{BaseUrl}/Catalog/Watched");
            pagination = driver.FindElements(By.XPath("//ul[@class='pagination']//li"));
            lastPageItem = pagination.Last();
            actions.MoveToElement(lastPageItem).Perform();

            lastpage = lastPageItem.FindElement(By.XPath(".//a"));
            lastpage.Click();

            movies = driver.FindElements(By.XPath("//div[@class='col-lg-4']"));
            lastMovie = movies.Last();

            var titleMovie = LastMovie().FindElement(By.XPath(".//h2"));

            Assert.That(titleMovie.Text.Trim(), Is.EqualTo(lastMovieTitle), "The movie title is not correct");
           
        }



        private string GenerateRandomString(int length)
        {
            
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private IWebElement LastMovie ()
        {
            var pages = driver.FindElements(By.XPath("//a[@class='page-link']"));
            var lastPage = pages.Last();
            actions.MoveToElement(lastPage).Click().Perform();

            var movies = driver.FindElements(By.XPath("//div[@class='col-lg-4']"));
            var lastMovie = movies.Last();
            actions.MoveToElement(lastMovie).Perform();

            return lastMovie;
        }
    }
}