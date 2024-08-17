using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace RevueCraftersSeleniumTests_II
{
    [TestFixture]
    public class RevueCraftersTests
    {
        private IWebDriver driver;
        private static readonly string BaseUrl = "https://d3s5nxhwblsjbi.cloudfront.net";
        private static string? lastCreatedRevueTitle;
        private static string? lastCreatedRevueDescription;
        Actions actions;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            
            driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); 

            driver.Navigate().GoToUrl($"{BaseUrl}/Users/Login#loginForm");

            driver.FindElement(By.XPath("//input[@id='form3Example3']")).SendKeys("iv@test.com");
            driver.FindElement(By.XPath("//input[@id='form3Example4']")).SendKeys("123456");
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block mb-4']")).Click();

        }

        [OneTimeTearDown] 

        public void OneTimwTearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test, Order(1)]
        public void Create_Revue_With_Invalid_Data_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/Create#createRevue");

            actions = new Actions(driver);

            var form = driver.FindElement(By.XPath("//form[@class='mx-1 mx-md-4']"));
            actions.MoveToElement(form).Perform();

            var title = ""; 
            var description = "";

            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).SendKeys(title);
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys(description);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var page = driver.Url;

            Assert.That(page, Is.EqualTo($"{BaseUrl}/Revue/Create#createRevue"), "The pageUrl is not correct");

            var errorMessage = driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']//li")).Text.Trim();
            
            Assert.That(errorMessage, Is.EqualTo("Unable to create new Revue!"), "The errorMessage is not correct");
        }

        [Test, Order(2)]
        public void Create_Random_Revue_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/Create#createRevue");

            actions = new Actions(driver);

            var form = driver.FindElement(By.XPath("//form[@class='mx-1 mx-md-4']"));
            actions.MoveToElement(form).Perform();

            lastCreatedRevueTitle = GenerateRandomString(5);
           lastCreatedRevueDescription= GenerateRandomString(10);

            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).SendKeys(lastCreatedRevueTitle);
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys(lastCreatedRevueDescription);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var page = driver.Url;

            Assert.That(page, Is.EqualTo($"{BaseUrl}/Revue/MyRevues#createRevue"), "The pageUrl should redirect");

            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastRevue = revues.Last();

            var title = lastRevue.FindElement(By.XPath(".//div[@class='text-muted text-center']")).Text.Trim();

            Assert.That(title, Is.EqualTo(lastCreatedRevueTitle), "The title is not correct");

        }

        [Test, Order(3)]

        public void Search_For_Revue_Title_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues#myRevues");

            var search=driver.FindElement(By.XPath("//input[@id='keyword']"));
            actions.MoveToElement(search).Perform();
            search.SendKeys(lastCreatedRevueTitle);

            driver.FindElement(By.XPath("//button[@id='search-button']")).Click();

            var title=driver.FindElement(By.XPath("//div[@class='text-muted text-center']"));
            actions.MoveToElement(title).Perform();
            var textTitle=title.Text.Trim();


            Assert.That(textTitle, Is.EqualTo(lastCreatedRevueTitle), "The title is not correct");

        }

        [Test, Order(4)]

        public void Edit_Last_Revue_Title_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues#myRevues");

            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            Assert.That(revues.Count() > 0, "There are not revues present");
            var lastRevue = revues.Last();
            actions.MoveToElement(lastRevue).Perform();

            var editButton = lastRevue.FindElement(By.XPath(".//a[text()='Edit']"));
            editButton.Click();
            var form=driver.FindElement(By.XPath("//form[@class='mx-1 mx-md-4']"));
            actions.MoveToElement(form).Perform();

            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();

            lastCreatedRevueTitle = "Edited:" + lastCreatedRevueTitle;
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).SendKeys(lastCreatedRevueTitle);

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var page = driver.Url;

            Assert.That(page, Is.EqualTo($"{BaseUrl}/Revue/MyRevues"), "The pageUrl should not redirect");

            revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            Assert.That(revues.Count() > 0, "There are not revues present");
            lastRevue = revues.Last();
            actions.MoveToElement(lastRevue).Perform();

           var editedTitle = lastRevue.FindElement(By.XPath(".//div[@class='text-muted text-center']")).Text.Trim();

            Assert.That(editedTitle, Is.EqualTo(lastCreatedRevueTitle), "The title is not correct");

        }

        [Test, Order(5)]

        public void Delete_Last_Revue_Title_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues#myRevues");

            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));

            Assert.That(revues.Count() > 0, "There are not revues present");

            var revuesBeforedelation = revues.Count();
            
            var lastRevue = revues.Last();
            actions.MoveToElement(lastRevue).Perform();

            var deleteButton = lastRevue.FindElement(By.XPath(".//a[text()='Delete']"));
            actions.MoveToElement(deleteButton).Perform();
            deleteButton.Click();

            var page = driver.Url;

            Assert.That(page, Is.EqualTo($"{BaseUrl}/Revue/MyRevues"), "The pageUrl should not redirect");

            revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));

            Assert.That(revuesBeforedelation, Is.EqualTo(revues.Count() + 1));
            lastRevue = revues.Last();
            var title = lastRevue.FindElement(By.XPath(".//div[@class='text-muted text-center']")).Text.Trim();

            Assert.That(title, Is.Not.EqualTo(lastCreatedRevueTitle));
        }



        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}