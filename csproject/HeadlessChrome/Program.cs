using System;
using System.IO;
using OpenQA.Selenium.Chrome;

namespace HeadlessChrome
{
    class Program
    {
        static string htmldir = "html";

        static void Main(string[] args)
        {
            if (!Directory.Exists(htmldir))
            {
                Directory.CreateDirectory(htmldir);
            }

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new string[] { "headless" });

            var browser = new ChromeDriver(chromeOptions);

            string datetimeOnRun = DateTime.Now.ToString("yyyyMMddHHmmss");

            string url = File.ReadAllText("url.txt");
            browser.Navigate().GoToUrl(url);

            browser.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

            string html = browser.PageSource;
            browser.Quit();

            File.WriteAllText(htmldir + "/" + datetimeOnRun + ".htm", html);
        }
    }
}
