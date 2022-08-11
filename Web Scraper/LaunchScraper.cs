using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Http;
using System.Threading;
using HtmlAgilityPack;

namespace Web_Scraper
{
    class LaunchScraper
    {
        private List<LaunchData> launches;

        public LaunchScraper() {
            launches = new List<LaunchData>();

            string[] pages = { "https://www.spacelaunchschedule.com/category/spacex/", "https://www.spacelaunchschedule.com/category/spacex/page/2/" };

            for (int i = 0; i < pages.Length; i++) {
                GetLaunchData(pages[i]);
                Thread.Sleep(2500);
            }

        }

        private async void GetLaunchData(string link)
        {
            var url = link;

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(html);

            var articles = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"main\"]").Descendants();

            List<string> launchArticleIDs = new List<string>();

            //Get article IDs for each launch
            foreach (HtmlNode article in articles)
            {
                if (article.GetAttributeValue("id", "") != "")
                {
                    launchArticleIDs.Add(article.GetAttributeValue("id", ""));
                }
            }

            //Parse each launch announcement and save information
            int locationIndex = 0;

            foreach (string launchID in launchArticleIDs)
            {
                //Get each piece of information
                HtmlNode launchAnnnouncement = htmlDocument.GetElementbyId(launchID);

                //Save it to a LaunchData object
                string launchStatus = (string)launchAnnnouncement.Descendants("time").FirstOrDefault().InnerHtml;
                string launchTime = "";

                if (launchStatus.Contains("Projected To Launch"))
                {
                    launchTime = "[PROJECTED] " + launchStatus.Substring(launchStatus.IndexOf('>')+1);
                }
                else {
                    launchTime = (string)launchAnnnouncement.Descendants("time").FirstOrDefault().GetAttributeValue("dateTime", "");
                }



                Console.WriteLine(launchTime);

                string launchName = (string)launchAnnnouncement.Descendants("h2").FirstOrDefault().InnerHtml;
                launchName = launchName.Substring(0, launchName.IndexOf('<'));
                Console.WriteLine(launchName);

                string launchRocket = (string)launchAnnnouncement.Descendants("span").FirstOrDefault().InnerHtml;
                Console.WriteLine(launchRocket);

                string launchLocation = (string)launchAnnnouncement.SelectNodes("//div[@class=\"col h6 mb-0 pt-2\"]")[locationIndex].InnerHtml;
                locationIndex++;
                Console.WriteLine(launchLocation);

                LaunchData localLaunchData = new LaunchData(launchName, launchRocket, launchTime, launchLocation);

                launches.Add(localLaunchData);
            }
        }

        public List<LaunchData> GetLaunches()
        {
            return launches;
        }
    }

    class LaunchData
    {
        private string launchName;
        private string rocketType;
        private DateTime launchDate;
        private bool projectedLaunchDate;
        private string launchLocation;

        public LaunchData(string launchName, string rocketType, string launchDate, string launchLocation)
        {
            this.launchName = launchName;
            this.rocketType = rocketType;
            this.launchLocation = launchLocation;

            DateTime time = DateTime.Now;

            //Parse launchDate and save in a DateTime object
            if (launchDate.Contains("[PROJECTED]")) {
                projectedLaunchDate = true;
                string[] delimitedValues = launchDate.Substring(launchDate.IndexOf('\n')+1).Split(',');
                string monthString = delimitedValues[0];
                string yearString = delimitedValues[1];

                Int32 year = 0;
                Int32 month = -1;
                if (Int32.TryParse(yearString, out Int32 launchYear)) { year = launchYear; }

                if (monthString.Equals("January")) month = 1;
                if (monthString.Equals("February")) month = 2;
                if (monthString.Equals("March")) month = 3;
                if (monthString.Equals("April")) month = 4;
                if (monthString.Equals("May")) month = 5;
                if (monthString.Equals("June")) month = 6;
                if (monthString.Equals("July")) month = 7;
                if (monthString.Equals("August")) month = 8;
                if (monthString.Equals("September")) month = 9;
                if (monthString.Equals("October")) month = 10;
                if (monthString.Equals("November")) month = 11;
                if (monthString.Equals("December")) month = 12;

                time = new DateTime(year, month, 1);

            } else
            {
                projectedLaunchDate = false;

                string[] spaceDelimitedString = launchDate.Split(' ');
                string[] dateMonthYearString = spaceDelimitedString[0].Split('-');
                string launchTime = spaceDelimitedString[1];
                string launchHourString = launchTime.Split(':')[0];
                string launchMinuteString = launchTime.Split(':')[1];


                Int32 year = 0;
                Int32 month = 0;
                Int32 day = 0;
                Int32 hour = 0;
                Int32 minute = 0;
                if (Int32.TryParse(dateMonthYearString[0], out Int32 launchYear)) { year = launchYear; }
                if (Int32.TryParse(dateMonthYearString[1], out Int32 launchMonth)) { month = launchMonth; }
                if (Int32.TryParse(dateMonthYearString[2], out Int32 launchDay)) { day = launchDay; }
                if (Int32.TryParse(launchHourString, out Int32 launchHour)) { hour = launchHour; }
                if (Int32.TryParse(launchMinuteString, out Int32 launchMinute)) { minute = launchMinute; }

                //dateTime military hour conversions
                if (hour < 12) hour += 12;
                if (hour == 12) hour = 0;

                //Timezoning
                time = new DateTime(year, month, day, hour, minute, 0);
                time = time.ToLocalTime();
            }

            this.launchDate = time;

        }

        public string GetLaunchName()
        {
            return launchName;
        }

        public string GetRocketType()
        {
            return rocketType;
        }

        public DateTime GetLaunchDate()
        {
            return launchDate;
        }

        public string GetLaunchLocation()
        {
            return launchLocation;
        }

        public bool IsLaunchDateProjected() 
        {
            return projectedLaunchDate;
        }
    }
}
