using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace IdeaCenterAppSeleniumWebTests_II
 {
    [TestFixture]
    public class IdeaCenetrSeleniumTests
    {
        protected IWebDriver driver;
        private static readonly string baseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:83";
        private static string lastCreatedIdeaTitle ="";
        private static string lastCreatedIdeaDescription="";
       

        [OneTimeSetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl(baseUrl);
            driver.FindElement(By.XPath("//a[@class='btn btn-outline-info px-3 me-2']")).Click();
            driver.FindElement(By.XPath("//input[@name='Email']")).SendKeys("iv11@test.com");
            driver.FindElement(By.XPath("//input[@name='Password']")).SendKeys("123456");
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg btn-block']")).Click();

        }
        [OneTimeTearDown]

        public void OneTeardown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test, Order(1)]
        public void Create_Idea_With_Invalid_Data_Test()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Ideas/Create");

            var title = "";
            var description = "";
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).SendKeys(title);
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys(description);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseUrl}/Ideas/Create"), "The page should remain on the creation page with invalid data.");

            var errorMessage = driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']//li")).Text;

            Assert.That(errorMessage, Is.EqualTo("Unable to create new Idea!"), "The error message is not correct");
        }

        [Test, Order(2)]
        public void Create_Idea_With_Valid_Data_Test()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Ideas/Create");
            lastCreatedIdeaTitle = GenerateRandomString(5);
            lastCreatedIdeaDescription = GenerateRandomString(10);

            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).SendKeys(lastCreatedIdeaTitle);
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys(lastCreatedIdeaDescription);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseUrl}/Ideas/MyIdeas"), "The page should redirect to the MyIdeas page.");

            var ideas = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastCreatedIdea = ideas.Last();

            var textDescription = lastCreatedIdea.FindElement(By.XPath(".//div[@class='card-body']//p")).Text;

            Assert.That(textDescription, Is.EqualTo(lastCreatedIdeaDescription));
        }

        [Test, Order(3)]

        public void View_Last_Idea_Test()
        {
            Assert.That(lastCreatedIdeaTitle, Is.Not.Null, "There is no ideas created");
            driver.Navigate().GoToUrl($"{baseUrl}/Ideas/MyIdeas");
            
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var ideas =wait.Until(driver=>driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));
            var lastCreatedIdea = ideas.Last();

            Assert.That(ideas.Count() > 0, "There is no ideas created");

            Actions actions = new Actions(driver);

            var viewButton = lastCreatedIdea.FindElement(By.XPath(".//a[text()='View']"));
            
            actions.MoveToElement(viewButton).Click().Perform();

            var title = driver.FindElement(By.XPath("//div[@id='intro']//h1")).Text;

            Assert.That(title, Is.EqualTo(lastCreatedIdeaTitle), "The title is not correct");

        }
        [Test, Order(4)]

        public void Edit_Last_Idea_Test()
        {
            Assert.That(lastCreatedIdeaTitle, Is.Not.Null, "There is no ideas created");
            
            driver.Navigate().GoToUrl($"{baseUrl}/Ideas/MyIdeas");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var ideas = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));
            var lastCreatedIdea = ideas.Last();

            Assert.That(ideas.Count() > 0, "There is no ideas created");

            Actions actions = new Actions(driver);

            var editButton = lastCreatedIdea.FindElement(By.XPath(".//a[text()='Edit']"));

            actions.MoveToElement(editButton).Click().Perform();

            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();
            var editTitle = $"Changed title: {lastCreatedIdeaTitle}";
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).SendKeys(editTitle);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            ideas = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));
            lastCreatedIdea = ideas.Last();

            var viewButton = lastCreatedIdea.FindElement(By.XPath(".//a[text()='View']"));

            actions.MoveToElement(viewButton).Click().Perform();

            var title = driver.FindElement(By.XPath("//div[@id='intro']//h1")).Text.Trim();

            Assert.That(title, Is.EqualTo(editTitle), "The title is not correct");

        }
        [Test, Order(5)]

        public void Edit_Last_Idea_Description_Test()
        {
            Assert.That(lastCreatedIdeaTitle, Is.Not.Null, "There is no ideas created");

            driver.Navigate().GoToUrl($"{baseUrl}/Ideas/MyIdeas");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var ideas = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));
            var lastCreatedIdea = ideas.Last();

            Assert.That(ideas.Count() > 0, "There is no ideas created");

            Actions actions = new Actions(driver);

            var editButton = lastCreatedIdea.FindElement(By.XPath(".//a[text()='Edit']"));

            actions.MoveToElement(editButton).Click().Perform();

            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            var editDescription = $"Changed title: {lastCreatedIdeaDescription}";
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys(editDescription);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            ideas = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));
            lastCreatedIdea = ideas.Last();

            var viewButton = lastCreatedIdea.FindElement(By.XPath(".//a[text()='View']"));

            actions.MoveToElement(viewButton).Click().Perform();

            var description = driver.FindElement(By.XPath("//p[@class='offset-lg-3 col-lg-6']"));
            actions.MoveToElement(description).Perform();

            Assert.That(description.Text.Trim(), Is.EqualTo(editDescription), "The edit description is not correct");

        }
        [Test, Order(6)]

        public void Delete_Last_Idea_Description_Test()
        {
            Assert.That(lastCreatedIdeaTitle, Is.Not.Null, "There is no ideas created");

            driver.Navigate().GoToUrl($"{baseUrl}/Ideas/MyIdeas");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var ideas = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));
            var lastCreatedIdea = ideas.Last();

            Assert.That(ideas.Count() > 0, "There is no ideas created");

            Actions actions = new Actions(driver);

            var deleteButton = driver.FindElement(By.XPath(".//a[text()='Delete']"));
            actions.MoveToElement(deleteButton).Click().Perform();

            ideas = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']")));

            bool isIdeaDeleted = ideas.All(cards => !cards.Text.Contains(lastCreatedIdeaTitle));

            Assert.IsTrue(isIdeaDeleted, "The idea  has  not been deleted");

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