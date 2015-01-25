using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace WebDriverDemo
{
    internal class Program
    {
        private static void Main()
        {
            var startTime = DateTime.Now;

            LogResults("Checking operating system info...." + startTime.ToLocalTime());
            LogResults("....\n\n");
            GetSystemInformation();
            LogResults("\n\n Test started.....\n\n");
            IWebDriver driver = new FirefoxDriver();
            OpenBrowser(driver);
            WrongUserLogin(driver);
            RightUserLogin(driver);
            AssetSelection(driver);
            var endTime = DateTime.Now;
            LogResults("End of Test..." + endTime.ToLocalTime());

            LogResults("\n\nDuration= " + (endTime - startTime).Seconds + " seconds");

            LogResults("=============================== ");
            Console.ReadKey();
        }

        private static void GetSystemInformation()
        {
            var si = new SystemInfo(); //Create an object of SystemInfo class.
            si.getOperatingSystemInfo();
                //Call get operating system info method which will display operating system information.
            si.getProcessorInfo(); //Call get  processor info method which will display processor info.
        }

        //Create lo file for test results

        public static void LogResults(string p)
        {
            Console.WriteLine(p);
            using (
                var file =
                    new StreamWriter(@"C:\Users\itl\Desktop\Mythings\Testing\Demo\logFile.txt", true))
            {
                //file.WriteLine("======================================");
                file.WriteLine(p);
            }
        }

        //Firing  browser'
        private static void OpenBrowser(IWebDriver driver)
        {
            driver.Url = "http://m.itelematic.com";
            driver.Manage().Window.Maximize();

            //Test case 1 : Assertion: Log On page

            Assert.AreEqual("Log On", driver.Title);
        }

//Incorrect user anme/password
        private static void WrongUserLogin(IWebDriver driver)
        {
            //Assert: Log in page
            Assert.AreEqual("Log On", driver.Title);
            for (var n = 0; n < 2; n++)
            {
                //n can go to a 1000 ..(check if acount locks after a number of attempts)
                //Clear textboxes
                driver.FindElement(By.Id("username")).Clear();
                driver.FindElement(By.Id("password")).Clear();

                //wrong username and password
                driver.FindElement(By.Id("username")).SendKeys(UserName);
                driver.FindElement(By.Id("password")).SendKeys(RandomPassword.Generate(8, 10));
                driver.FindElement(By.Id("rememberMe")).Click();
                driver.FindElement(By.XPath("//input[@type='submit']")).Click();


                //Test case 2: When user name and/ or password incorrect, browser doesn't leave the log on page
                Assert.AreEqual("Log On", driver.Title);


                //Test case 3: Incorrect user name and/or password Error message displayed

                try
                {
                    Assert.AreEqual("The username or password provided is incorrect.",
                        driver.FindElement(By.CssSelector("li")).Text);
                }
                catch (AssertionException e)
                {
                    LogResults("TEST CASE 3: INVALID LOGIN TEST RESULTS : " + e.Message);
                }
                Assert.AreEqual("Log On", driver.Title);
            }
        }

//Valid user name and password
        private static void RightUserLogin(IWebDriver driver)
        {
            //Assert: Log in page
            Assert.AreEqual("Log On", driver.Title);

            //Clear textboxes
            driver.FindElement(By.Id("username")).Clear();
            driver.FindElement(By.Id("password")).Clear();

            //legit username and password
            driver.FindElement(By.Id("username")).SendKeys(UserName);
            driver.FindElement(By.Id("password")).SendKeys(ValidPassword);
            driver.FindElement(By.Id("rememberMe")).Click();
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            //Test case 4: When valid user name and password used, the ibE lite Page is opened

            Assert.AreEqual("ibE Lite", driver.Title);
        }

        private static void AssetSelection(IWebDriver driver)
        {
            for (var n = 0; n <= 1; n++)
            {
                driver.FindElement(By.CssSelector("img.img-responsive.center-block")).Click();
                driver.FindElement(By.Id("s2id_autogen1")).Click();

                var groups = new SelectElement(driver.FindElement(By.Id("groups")));

                groups.SelectByIndex(n);
                var assetType = groups.SelectedOption.Text;


                driver.FindElement(By.CssSelector("button.btn.gradient")).Click(); //click search

                //Groups dropdown
                IList<IWebElement> details = driver.FindElements(By.LinkText("Details"));

                Console.WriteLine("There are {0} {1} ", details.Count(), assetType);

                NavigateGroups(driver, details);
                // driver.FindElement(By.CssSelector("img.img-responsive.center-block")).Click();

                // driver.FindElement(By.CssSelector("a.select2-search-choice-close")).Click(); //clear
            }
            //Console.ReadKey();
        }

        //navigation into Assets

        private static void NavigateGroups(IWebDriver driver, IList<IWebElement> details)
        {
            if (details.Count == 0)
            {
                try
                {
                    //Test case 5: When a group is empty (eg; Drivers);  message displayed: "There are no assets to show"
                    Assert.AreEqual("There are no assets to show", driver.FindElement(By.CssSelector("h3")).Text);
                }
                catch (AssertionException e)
                {
                    Console.WriteLine(e.Message);
                }
                try
                {
                    driver.FindElement(By.CssSelector("a.select2-search-choice-close")).Click(); //clear
                    //  Assert.AreEqual("There are no assets to show", driver.FindElement(By.CssSelector("h3")).Text);
                }
                catch (Exception e)
                {
                    LogResults(e.Message);
                }
            }

            else
            {
                //OPEN EACH ASSET IN DIFFERENT TABS
                foreach (var item in details)
                {
                    //Assert.IsTrue(IsElementPresent(By.CssSelector("div.col-md-1.col-xs-2 > img")));
                    var motion1 = driver.FindElement(By.CssSelector("div.col-md-1.col-xs-2 > img"));


                    new Actions(driver)
                        .KeyDown(Keys.Control).KeyDown(Keys.Shift)
                        .Click(item)
                        .KeyUp(Keys.Control).KeyUp(Keys.Shift)
                        .Perform();

                    // Assert.IsTrue(IsElementPresent(By.CssSelector("div.col-md-1.col-xs-2 > img")));
                    var motion2 = driver.FindElement(By.CssSelector("div.col-md-1.col-xs-2 > img"));
                    try
                    {
                        //Test case 6: Check that STOPPED/MOVING button/image us the same for list view and Detail view
                        Assert.That(motion1.GetAttribute("src"), Is.EqualTo(motion2.GetAttribute("src")));
                    }
                    catch (Exception e)
                    {
                        LogResults("TEST CASE 6: MOTION IMAGE SIMILARITY: " + e.Message);
                    }
                }
                //driver.FindElement(By.CssSelector("img.img-responsive.center-block")).Click();
            }
        }

        #region FIELDS FOR LOGIN DETAILS

        private const string UserName = "bede@root@gsf24";
        private const string ValidPassword = "Hosapa01";

        #endregion
    }
}