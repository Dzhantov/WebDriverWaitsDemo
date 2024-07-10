using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace WebDriverWaitsDemo;

public class WebDriverWaitDemo
{
    IWebDriver driver;
    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/dynamic.html");

    }
    [TearDown]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }

    [Test]
    public void AddBoxWithoutWaitsFails()
    {
        IWebElement addButton = driver.FindElement(By.Id("adder"));
        addButton.Click();
        
        Assert.Throws<NoSuchElementException>(() =>
        {
            IWebElement newBox = driver.FindElement(By.Id("box0"));
        });
    }

    [Test]
    public void RevealInputWithoutWaitsFail()
    {
        IWebElement revealButton = driver.FindElement(By.Id("reveal"));
        revealButton.Click();

        Assert.Throws<ElementNotInteractableException>(() =>
        {
            IWebElement revealed = driver.FindElement(By.Id("revealed"));
            revealed.SendKeys("Displayed");
        });
    }

    [Test]
    public void AddBoxWithThreadSleep()
    {
        IWebElement addButton = driver.FindElement(By.Id("adder"));
        addButton.Click();
        Thread.Sleep(3000);

        IWebElement newBox = driver.FindElement(By.Id("box0"));

        Assert.IsTrue(newBox.Displayed);
    }

    [Test]
    public void AddBoxWithImplicitWait()
    {
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement addButton = driver.FindElement(By.Id("adder"));
        addButton.Click();
        IWebElement newBox = driver.FindElement(By.Id("box0"));

        Assert.IsTrue(newBox.Displayed);
    }

    [Test]
    public void RevealInputWithImplicitWaits()
    {
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        driver.FindElement(By.Id("reveal")).Click();

        IWebElement revealed = driver.FindElement(By.Id("revealed"));

        Assert.That(revealed.TagName, Is.EqualTo("input"));
    }

    [Test]
    public void RevealInputWithExplicitWaits()
    {
        driver.FindElement(By.Id("reveal")).Click();
        IWebElement revealed = driver.FindElement(By.Id("revealed"));

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("revealed")));

        Assert.That(revealed.Displayed);
    }

    [Test]
    public void AddBoxWithFluentWaitExpectedConditionsAndIgnoreExceptions()
    {
        driver.FindElement(By.Id("adder")).Click();

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.PollingInterval = TimeSpan.FromMilliseconds(500);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

        IWebElement newBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("box0")));

        Assert.IsTrue(newBox.Displayed);
    }

    [Test]
    public void RevealInputWithCustomFluentWait()
    {
        driver.FindElement(By.Id("reveal")).Click();
        
        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);

        fluentWait.Timeout = TimeSpan.FromSeconds(5);
        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(200);

        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        fluentWait.IgnoreExceptionTypes(typeof(ElementNotInteractableException));

        IWebElement finishDiv = fluentWait.Until(ExpectedConditions.ElementExists(By.Id("revealed")));
        
        Assert.That(finishDiv.TagName, Is.EqualTo("input"));
        Assert.That(finishDiv.GetAttribute("value"), Is.EqualTo(""));
    }
}
