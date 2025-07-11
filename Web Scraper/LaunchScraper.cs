﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Http;
using System.Threading;
using HtmlAgilityPack;
using Discord;

namespace Web_Scraper
{
    class LaunchScraper
    {
        private List<LaunchData> launches;
        private DateTime lastUpdate;

        public LaunchScraper() {
            UpdateData();
        }

        public EmbedBuilder PrintSchedule(string filter)
        {
            LaunchScraper scraper = new LaunchScraper();

            Thread.Sleep(1000);

            return GetDiscordMessageFormatting(scraper.GetLaunches(), filter);
        }

        public static EmbedBuilder GetDiscordMessageFormatting(List<LaunchData> launches, string filter) 
        {
            string subject = "";
            string description = "";

            //Process params
            int counter = 0;

            foreach (LaunchData localLaunch in launches)
            {
                if (counter >= 15) break;

                //Process params
                bool skipCurrent = false;
                if (filter.Equals("star_link")) 
                {
                    if (counter == 0) { subject = ":rocket:  **Upcoming SpaceX Star Link Launches**  :rocket: \n"; }

                    if (!localLaunch.GetLaunchName().Contains("Starlink")) 
                    {
                        skipCurrent = true;
                    }
                }
                if (filter.Equals("star_ship"))
                {
                    if (counter == 0) { subject = ":rocket:  **Upcoming SpaceX Star Ship Launches**  :rocket: \n"; }

                    if (!localLaunch.GetLaunchLocation().Contains("TX"))
                    {
                        skipCurrent = true;
                    }
                }
                if (filter.Equals("")) 
                {
                    if (counter == 0) { subject = ":rocket:  **Upcoming SpaceX Launches**  :rocket: \n"; }
                }

                if (!skipCurrent) 
                { 
                    DateTime launchDate = localLaunch.GetLaunchDate();

                    TimeSpan tMinus = launchDate.Subtract(DateTime.Now);
                    Console.WriteLine(launchDate.ToString() + " " + tMinus.TotalHours);

                    description += "__**" + localLaunch.GetLaunchName() + "**__";

                    if (localLaunch.IsLaunchDateProjected() == true)
                    {
                        description += " is **projected**";
                    }
                    else if (tMinus.TotalHours <= 48)
                    {
                        description += " is **launching soon**";
                    }
                    else if (tMinus.TotalHours <= 0)
                    {
                        description += " has **launched**";
                    }
                    else
                    {
                        description += " is currently **scheduled**";
                    }


                    if (localLaunch.IsLaunchDateProjected() == true && tMinus.TotalHours <= 0)
                    {
                        string monthString = Utilities.GetMonthString(localLaunch.GetLaunchDate().Month);
                        description += " for " + monthString + ", " + localLaunch.GetLaunchDate().Year;
                    }
                    else
                    {
                        description += " for " + localLaunch.GetLaunchDate() + " (PST)";
                    }

                    description += " at " + localLaunch.GetLaunchLocation();

                    description += "\n";
                }
                

                counter++; 
            }

            var embedBuilder = new EmbedBuilder()  
                    .WithTitle(subject)
                    .WithDescription(description)
                    .WithColor(Color.Green)
                    .WithCurrentTimestamp();

            return embedBuilder;
        }

        public static void SendEmail(List<LaunchData> launches, bool devMode)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential credential = new System.Net.NetworkCredential("<email>", "<password>");
                client.EnableSsl = true;
                client.Credentials = credential;

                MailMessage message;

                message = new MailMessage("<my email>", "<my email>");

                message.Subject = "Upcoming Spacex Launches";
                message.Body = GetHTMLFormatting(launches);
                message.IsBodyHtml = true;

                Console.WriteLine("Sending email!");

                client.Send(message);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetHTMLFormatting(List<LaunchData> launches)
        {
            string body = "";

            body += "<body><center><h1>The following is a list of upcoming SpaceX Launches</h1><br>";

            body += "<table>";
            body += "<tr style=\"text-align: center; \">";
            body += "<td><h3> Mission Name </h3></td>";
            body += "<td><h3> Launch Status </h3></td>";
            body += "<td><h3> Launch Date </h3></td>";
            body += "<td><h3> Launch Location </h3></td>";
            body += "</tr>";

            foreach (LaunchData localLaunch in launches)
            {
                DateTime launchDate = localLaunch.GetLaunchDate();

                TimeSpan tMinus = launchDate.Subtract(DateTime.Now);
                Console.WriteLine(launchDate.ToString() + " " + tMinus.TotalHours);


                if (localLaunch.IsLaunchDateProjected() == false && tMinus.TotalHours <= 48)
                {
                    body += "<tr style=\"text-align: center; color: darkgreen\">";
                }
                else
                {
                    body += "<tr style=\"text-align: center;\">";
                }

                body += "<td><b>" + localLaunch.GetLaunchName() + "</b></td>";

                if (localLaunch.IsLaunchDateProjected() == true)
                {
                    body += "<td style=\"color: Orange;\">Projected</td>";
                }
                else if (tMinus.TotalHours <= 48)
                {
                    body += "<td><b>Launching Soon!</b></td>";
                }
                else if (tMinus.TotalHours <= 0)
                {
                    body += "<td><b>Launched!</b></td>";
                }
                else
                {
                    body += "<td style=\"color: Green;\">Scheduled</td>";
                }


                if (localLaunch.IsLaunchDateProjected() == true)
                {
                    //Get month value as a string
                    int month = localLaunch.GetLaunchDate().Month;
                    string monthString = "null";

                    if (month == 1) monthString = "January";
                    if (month == 2) monthString = "February";
                    if (month == 3) monthString = "March";
                    if (month == 4) monthString = "April";
                    if (month == 5) monthString = "May";
                    if (month == 6) monthString = "June";
                    if (month == 7) monthString = "July";
                    if (month == 8) monthString = "August";
                    if (month == 9) monthString = "September";
                    if (month == 10) monthString = "October";
                    if (month == 11) monthString = "November";
                    if (month == 12) monthString = "December";

                    body += "<td>" + monthString + ", " + localLaunch.GetLaunchDate().Year + " </td>";
                }
                else
                {
                    body += "<td>" + localLaunch.GetLaunchDate() + " </td>";
                }

                body += "<td>" + localLaunch.GetLaunchLocation() + "</td>";

                body += "</tr>";
            }

            body += "</table>";
            body += "<center></body>";

            return body;
        }

        public DateTime GetLastUpdate()
        {
            return lastUpdate;
        }

        public void UpdateData()
        {
            launches = new List<LaunchData>();

            string[] pages = { "https://www.spacelaunchschedule.com/category/spacex/", "https://www.spacelaunchschedule.com/category/spacex/page/2/" };

            for (int i = 0; i < pages.Length; i++)
            {
                GetLaunchData(pages[i]);
                Thread.Sleep(500);
            }

            lastUpdate = DateTime.Now;
        }

       

        //Web scraping functionality
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

                string launchName = (string)launchAnnnouncement.Descendants("h2").FirstOrDefault().InnerHtml;
                launchName = launchName.Substring(0, launchName.IndexOf('<'));

                string launchRocket = (string)launchAnnnouncement.Descendants("span").FirstOrDefault().InnerHtml;

                string launchLocation = (string)launchAnnnouncement.SelectNodes("//div[@class=\"col h6 mb-0 pt-2\"]")[locationIndex].InnerHtml;
                locationIndex++;

                LaunchData localLaunchData = new LaunchData(launchName, launchRocket, launchTime, launchLocation);

                launches.Add(localLaunchData);
            }
        }

        public List<LaunchData> GetLaunches()
        {
            return launches;
        }
    }

    //Data capturing class
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
                string yearString = delimitedValues[1];

                Int32 year = 0;
                if (Int32.TryParse(yearString, out Int32 launchYear)) { year = launchYear; }

                Int32 month = Utilities.GetMonthINT(delimitedValues[0]);

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
