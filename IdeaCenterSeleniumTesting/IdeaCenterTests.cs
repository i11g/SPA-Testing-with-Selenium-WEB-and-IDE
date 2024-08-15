using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Text;

namespace IdeaCenterSeleniumTesting
{
    [TestFixture]
    public class IdeaCenterTests
    {
        private IWebDriver driver;
        protected static readonly string baseURL = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:83";
        protected readonly string email = "iv1@test.com";
        protected readonly string password = "123456";
        protected string randomTitle = "";
        protected string randomDescription = "";


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
        [Test, Order(2)] 

        public void Create_Random_Test_Idea()
        {
            driver.Navigate().GoToUrl($"{baseURL}/Ideas/Create");

            randomTitle = GenerateRandomString(5);
            randomDescription = GenerateRandomString(10);

            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(randomTitle);

            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys(randomDescription);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseURL}/Ideas/MyIdeas"));

            var ideas=driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
            var lastIdea = ideas.Last();

            var description = lastIdea.FindElement(By.CssSelector("p.card-text"));

            Assert.That(description.Text.Trim(), Is.EqualTo(randomDescription)); 

        }
        [Test, Order(3)] 

        public void View_Last_Cretaed_Idea_Test()
        {
            driver.Navigate().GoToUrl($"{baseURL}/Ideas/MyIdeas");
            var ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
            var lastIdea = ideas.Last();
            Actions actions = new Actions(driver);
            var view = lastIdea.FindElement(By.XPath(".//a[text()='View']"));
            actions.MoveToElement(view).Click().Perform(); 
            
            var title=driver.FindElement(By.CssSelector(".p-5.text-center.bg-light"));
            
            var lastideaTitle=title.Text.Trim();

            Assert.That(lastideaTitle, Is.EqualTo(randomTitle));
        }
        [Test, Order(4)] 

        public void Edit_LastCreatedIdea_Title_Test()
        {
            driver.Navigate().GoToUrl($"{baseURL}/Ideas/MyIdeas");
            var ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
            var lastIdea = ideas.Last();

            Actions actions = new Actions(driver);
            var view = lastIdea.FindElement(By.XPath(".//a[text()='Edit']"));
            actions.MoveToElement(view).Click().Perform();

            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys($"Edited Title:{randomTitle}");
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseURL}/Ideas/MyIdeas"));

             ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
             lastIdea = ideas.Last();
            view = lastIdea.FindElement(By.XPath(".//a[text()='View']"));
            actions.MoveToElement(view).Click().Perform();

            var title = driver.FindElement(By.CssSelector(".p-5.text-center.bg-light"));

            var lastideaTitle = title.Text.Trim();

            Assert.That(lastideaTitle, Is.EqualTo($"Edited Title:{randomTitle}"));

        }

        [Test, Order(5)] 

        public void Edit_LastCreatedIdea_Description_Test()
        {
            driver.Navigate().GoToUrl($"{baseURL}/Ideas/MyIdeas");
            var ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
            var lastIdea = ideas.Last();

            Actions actions = new Actions(driver);
            var view = lastIdea.FindElement(By.XPath(".//a[text()='Edit']"));
            actions.MoveToElement(view).Click().Perform();

            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).Clear();
            driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']")).SendKeys($"Edited Description{randomDescription}");
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseURL}/Ideas/MyIdeas"));

            ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
            lastIdea = ideas.Last();
            view = lastIdea.FindElement(By.XPath(".//a[text()='View']"));
            actions.MoveToElement(view).Click().Perform();

            var description = driver.FindElement(By.XPath("//p[@class='offset-lg-3 col-lg-6']"));

            Assert.That(description.Text.Trim(), Is.EqualTo($"Edited Description{randomDescription}"));
        }

        [Test, Order(6)] 

        public void Delete_LastCreatedIdea_Test()
        {
            driver.Navigate().GoToUrl($"{baseURL}/Ideas/MyIdeas");
            var ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));
            var lastIdea = ideas.Last();
            var numberOfIdeas = ideas.Count();

            Actions actions = new Actions(driver);
            var view = lastIdea.FindElement(By.XPath(".//a[text()='Delete']"));
            
            actions.MoveToElement(view).Click().Perform();
            ideas = driver.FindElements(By.CssSelector(".card.mb-4.box-shadow"));

            Assert.That(numberOfIdeas, Is.EqualTo(ideas.Count()+1 ));
        }

        private  static Random random = new Random();

        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}