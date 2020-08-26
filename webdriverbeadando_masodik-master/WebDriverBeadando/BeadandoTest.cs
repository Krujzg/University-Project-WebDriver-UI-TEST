using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace WebDriverBeadando
{
    class BeadandoTest : TestBase
    {
        WebDriverWait wait;
        int pre_url_Length;
        double cheapest_mobile_price;
        double first_price;
        int post_url_Length;

        [Test, TestCaseSource("BrandData")]
        [Obsolete]
        public void TestWebPageArukereso(string brand)
        {
            DriverManagement();
            WaitSetup();
            WaitTillPageLoaded();
            BrandChoosing(brand);

            pre_url_Length = Driver.Url.Length;

            WaitTillTheTitleHasBeenChanged(pre_url_Length);
            cheapest_mobile_price = GetMobilePrice();


            first_price = GetFirstItemSPriceFromTheList();

            Assert.AreEqual(cheapest_mobile_price, first_price);
        }

        private void DriverManagement()
        {
            Driver.Manage().Window.Maximize();
            //Driver.Manage().Cookies.DeleteAllCookies(); // törlöm a cookie-kat , mert néha belezavarnak a kattintásba
            Driver.Url = "https://www.arukereso.hu/mobiltelefon-c3277/";
        }

        private void WaitSetup()
        {
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        //private void WaitTillPageLoaded()
        //{
        //    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("filter-bar")));
        //}

        public void WaitTillPageLoaded() => wait.Until(x => FilterBarIsDisplayed());

        private bool FilterBarIsDisplayed() => Driver.FindElement(By.Id("filter-bar")).Displayed;

        private void BrandChoosing(string brand)
        {
            String brandtitle_path = "//*[@title=' - Gyártó: " + brand + "']";
            var choosenbrand = Driver.FindElement(By.XPath(brandtitle_path));
            choosenbrand.Click();
        }

        private void WaitTillTheTitleHasBeenChanged(int pre_url_Length)
        {
            wait.Until(x => TitleLengthIsLongEnough(pre_url_Length));
        }

        private bool TitleLengthIsLongEnough(int pre_url_Length)
        {
            post_url_Length = Driver.Url.Length;
            return pre_url_Length < post_url_Length;
        }

        private double GetMobilePrice()
        {
            String pre_price_path = "/html/body/div[2]/div[2]/div[2]/div[2]/div[2]/div[1]/div[1]/div[3]/a[1]";
            var mobile_price_element = Driver.FindElement(By.XPath(pre_price_path));
            String mobile_price_unedited = mobile_price_element.Text;
            String mobile_price_edited = mobile_price_unedited.Substring(0, mobile_price_unedited.Length - 7);
            mobile_price_element.Click();
            return Convert.ToDouble(mobile_price_edited);
        }

        private double GetFirstItemSPriceFromTheList()
        {
            String price_class_path = "/html/body/div[1]/div[2]/div[3]/div[2]/span/span[1]";
            var lowest_price_element_second_page = Driver.FindElement(By.XPath(price_class_path));
            String mobile_price_unedited = lowest_price_element_second_page.GetAttribute("content");
            String mobile_price_edited = mobile_price_unedited.Substring(0, mobile_price_unedited.Length - 3);
            return Convert.ToDouble(mobile_price_edited);
        }

        static IEnumerable BrandData()
        {
            var doc = XElement.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory) + "\\Brands.xml");
            return
                from brands in doc.Descendants("brandData")
                let brand = brands.Attribute("text").Value
                select new object[] { brand };
        }
    }
}
