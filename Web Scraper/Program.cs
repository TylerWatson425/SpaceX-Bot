using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading;

namespace Web_Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            bool devMode = true;


            LaunchScraper scraper = new LaunchScraper();

            Thread.Sleep(5000);

            SendEmail(scraper.GetLaunches(), devMode);
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

               
            } catch (Exception ex) {
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
                else {
                    body += "<tr style=\"text-align: center;\">";
                }

                body += "<td><b>" + localLaunch.GetLaunchName() + "</b></td>";

                if (localLaunch.IsLaunchDateProjected() == true) {
                    body += "<td style=\"color: Orange;\">Projected</td>";
                } else if (tMinus.TotalHours <= 48)
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

        public static bool SameDay(DateTime time) {
            DateTime today = DateTime.Now;
            return ((today.Day == time.Day) && (today.Month == time.Month) && (today.Year == time.Year));
        }
    }
}


