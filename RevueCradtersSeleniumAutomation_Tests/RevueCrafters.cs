using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace RevueCradtersSeleniumAutomation_Tests
{
    [TestFixture]
    public class RevueCraftersTests
    {   
        protected IWebDriver driver;
        protected readonly string baseUrl = "https://d3s5nxhwblsjbi.cloudfront.net";
        protected readonly string eMail = "iv@test.com";
        protected readonly string password = "123456";
        Actions actions;

        private static string lastCreatedTitle="";
        private static string lastCreatedDescription="";



        [OneTimeSetUp]
        public void OneTimeSetup()
        {   
            var chromeOptions= new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl(baseUrl);
            
            driver.FindElement(By.XPath("//a[text()='Login']")).Click();

            var form=driver.FindElement(By.XPath("//form[@method='post']"));    

            actions=new Actions(driver);
            actions.MoveToElement(form).Click().Perform();

            driver.FindElement(By.XPath("//input[@name='Email']")).Click();
            driver.FindElement(By.XPath("//input[@name='Email']")).SendKeys(eMail);


            driver.FindElement(By.XPath("//input[@name='Password']")).Click();
            driver.FindElement(By.XPath("//input[@name='Password']")).SendKeys(password);

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block mb-4']")).Click();
        }

        [OneTimeTearDown] 
        public void OneTimeTearDown()
        {
            driver.Quit();
            driver.Dispose();
        }


        [Test, Order(1)]
        public void Create_RevueWith_InvalidData_Test()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Revue/Create#createRevue");

            var form=driver.FindElement(By.XPath("//form[@class='mx-1 mx-md-4']"));
            actions.ScrollToElement(form).Perform();

            var title ="";
            var description ="";

            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(title);
            driver.FindElement(By.XPath("//textarea[@name='Description']")).Clear();
            driver.FindElement(By.XPath("//textarea[@name='Description']")).SendKeys(description);
            
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseUrl}/Revue/Create#createRevue"));

            var textErrorMessage=driver.FindElement(By.XPath("//div[@class='text-danger validation-summary-errors']//li")).Text;

            Assert.That(textErrorMessage, Is.EqualTo("Unable to create new Revue!"));

        }
        [Test, Order(2)] 

        public void Create_Random_Revue_Test ()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Revue/Create#createRevue");

            var form = driver.FindElement(By.XPath("//form[@class='mx-1 mx-md-4']"));
            actions.ScrollToElement(form).Perform();

            lastCreatedTitle = GenerateRandomString(5);
            lastCreatedDescription = GenerateRandomString(10);
            driver.FindElement(By.XPath("//input[@name='Title']")).Clear();
            driver.FindElement(By.XPath("//input[@name='Title']")).SendKeys(lastCreatedTitle);
            driver.FindElement(By.XPath("//textarea[@name='Description']")).Clear();
            driver.FindElement(By.XPath("//textarea[@name='Description']")).SendKeys(lastCreatedDescription);
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseUrl}/Revue/MyRevues#createRevue"));

            var revues=driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var lastRevue = revues.Last();

            var titleLast = lastRevue.FindElement(By.XPath(".//div[@class='text-muted text-center']")).Text;

            Assert.That(titleLast, Is.EqualTo(lastCreatedTitle));

        }
        [Test, Order(3)] 

        public void Search_For_Revue_Title_Test()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Revue/MyRevues#myRevues");
            var search=driver.FindElement(By.XPath("//input[@id='keyword']"));
            actions.ScrollToElement(search).Perform();
            driver.FindElement(By.XPath("//input[@id='keyword']")).SendKeys(lastCreatedTitle);
            driver.FindElement(By.XPath("//button[@id='search-button']")).Click();

            var title=driver.FindElement(By.XPath("//div[@class='text-muted text-center']")).Text;

            Assert.That(title, Is.EqualTo(lastCreatedTitle));
        }
        [Test, Order(4)]

        public void Edit_Last_Created_Revue_Title_Test()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Revue/MyRevues#myRevues");
            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            
            Assert.IsTrue(revues.Count()>0, "The last revue is not present" );

            var lastRevue = revues.Last();
            actions.MoveToElement(lastRevue).Perform();

            var edit=lastRevue.FindElement(By.XPath(".//a[text()='Edit']"));
            actions.MoveToElement(edit).Click().Perform();

            var form= driver.FindElement(By.XPath("//form[@class='mx-1 mx-md-4']"));
            
            actions.MoveToElement(form).Perform();
            var editedTitle ="Edited" + lastCreatedTitle;
            
            driver.FindElement(By.XPath("//input[@id='form3Example1c']")).Clear();
            var title = driver.FindElement(By.XPath("//input[@id='form3Example1c']"));
                title.SendKeys(editedTitle); 
            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg']")).Click();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseUrl}/Revue/MyRevues"));
            
            revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            lastRevue = revues.Last();
            actions.MoveToElement(lastRevue).Perform();

            var lastEditTitle=lastRevue.FindElement(By.XPath(".//div[@class='text-muted text-center']")).Text;

            Assert.That(lastEditTitle, Is.EqualTo(editedTitle));
        }
        [Test, Order(5)] 

        public void Delete_last_Created_Revue_Test()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Revue/MyRevues#myRevues");
            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            var revuesBeforeDelation = revues.Count();

            Assert.IsTrue(revues.Count() > 0, "The last revue is not present");

            var lastRevue = revues.Last();
            actions.MoveToElement(lastRevue).Perform();

            var delete = lastRevue.FindElement(By.XPath(".//a[text()='Delete']"));
            actions.MoveToElement(delete).Click().Perform();

            var pageUrl = driver.Url;

            Assert.That(pageUrl, Is.EqualTo($"{baseUrl}/Revue/MyRevues"));

            revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));

            Assert.That(revuesBeforeDelation, Is.EqualTo(revues.Count()+1));
            lastRevue = revues.Last();
            var lasttitle = lastRevue.FindElement(By.XPath(".//div[@class='text-muted text-center']")).Text;

            Assert.That(lasttitle, Is.Not.EqualTo(lastCreatedTitle));
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