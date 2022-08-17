using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using HtmlAgilityPack;

namespace Web_Scraper
{
    class ClosureManager
    {
        private List<ClosureData> closureManager;
        public ClosureManager()
        {
            closureManager = new List<ClosureData>();

            PopulateData();
        }

        public async void PopulateData()
        {
            var url = "https://www.cameroncountytx.gov/spacex/";

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(html);

            var tableRows = htmlDocument.DocumentNode.SelectNodes("//tbody/tr").Descendants();

            Console.WriteLine("ROWCOUNT: " + tableRows.Count());

            int counter = 1;
            ClosureData localStore = new ClosureData();
            DateTime time;
            string savedDateString = "";

            foreach (HtmlNode element in tableRows)
            {
                var rowContents = element.Descendants();

                string previousText = "";
                foreach (HtmlNode node in rowContents)
                {
                    if ((node.InnerText != previousText) && (node.InnerText != "\n"))
                    {
                        previousText = node.InnerText;
                        Console.WriteLine(counter + ": " + node.InnerText + ":");

                        if ((counter % 4) == 1)
                        {
                            //set status & create new object
                            localStore = new ClosureData();
                            if (node.InnerText.Equals("Primary Date")) 
                            {
                                localStore.SetStatus(ClosureStatus.PRIMARY_DATE);
                            }
                            if (node.InnerText.Equals("Alternative Date"))
                            {
                                localStore.SetStatus(ClosureStatus.ALTERNATE_DATE);
                            }
                            Console.WriteLine("!" + node.InnerText);

                        }
                        if ((counter % 4) == 2)
                        {
                            //Save date
                            savedDateString = node.InnerText;
                            Console.WriteLine("!" + node.InnerText);
                        }
                        if ((counter % 4) == 3)
                        {
                            //save date
                            Console.WriteLine("!" + node.InnerText);
                            Console.WriteLine("saved:" + savedDateString);
                            string[] splitDateComponents = savedDateString.Split(',');

                            Console.WriteLine("!!" + splitDateComponents[1].Split(' ')[1]);
                            Console.WriteLine("!!!" + splitDateComponents[1].Split(' ')[2]);

                            string monthString = splitDateComponents[1].Split(' ')[1];
                            int month = -1;

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


                            int date = Int32.Parse(splitDateComponents[1].Split(' ')[2]);
                            Console.WriteLine("YEAR:" + splitDateComponents[2]);
                            int year = Int32.Parse(splitDateComponents[2].Substring(1));

                            //TODO: add start and end time parsing

                            time = new DateTime(year, month, date);

                            localStore.SetStartTime(time);
                            localStore.SetEndTime(time);
                        }
                        if ((counter % 4) == 0)
                        {
                            //TODO: add beach status into ClosureStatus
                            Console.WriteLine("!" + node.InnerText);

                            closureManager.Add(localStore);
                        }
                        counter++;
                    } 
                }
            }
        }


        public EmbedBuilder GenerateDiscordReport()
        {
            Console.WriteLine("DISCORD MESSAGE FIRED");
            if (closureManager.Count == 0) return null;

            string description = "";
            bool activeClosure = false;

            foreach (ClosureData closure in closureManager) 
            {
                if ((closure.GetEndTime().Day >= DateTime.Now.Day) || (closure.GetEndTime().Month > DateTime.Now.Month))
                {
                    ClosureStatus status = closure.GetStatus();

                    if (status == ClosureStatus.ACTIVE)
                    {
                        description += "**There is currently an active closure - a static fire or launch might be imminent!** \n";
                        activeClosure = true;
                    }
                    if ((status == ClosureStatus.PRIMARY_DATE) || (status == ClosureStatus.ALTERNATE_DATE))
                    {
                        if (status == ClosureStatus.PRIMARY_DATE)
                        {
                            description += "Next **primary closure** is scheduled for ";
                        }
                        else
                        {
                            description += "Next **alternative closure** is scheduled for ";
                        }

                        int month = closure.GetStartTime().Month;
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

                        description += monthString + ", " + closure.GetStartTime().Day + ", " + closure.GetStartTime().Year + "\n";
                    }
                }
            }

            var embedBuilder = new EmbedBuilder()
                    .WithTitle("Upcoming StarBase Road Closures")
                    .WithDescription(description)
                    .WithColor(activeClosure ? Color.Red : Color.Green)
                    .WithCurrentTimestamp();

            return embedBuilder;
        }
    }

    enum ClosureStatus {ACTIVE, PRIMARY_DATE, ALTERNATE_DATE}
    class ClosureData 
    { 
        private ClosureStatus status;
        private DateTime startTime;
        private DateTime endTime;
        public ClosureData() 
        { 
            this.status = ClosureStatus.ACTIVE;
            this.startTime = DateTime.Now;
            this.endTime = DateTime.Now;
        }

        public DateTime GetStartTime() { return startTime; }
        public DateTime GetEndTime() { return endTime; }
        public ClosureStatus GetStatus() { return status; }

        public void SetStartTime(DateTime start) { this.startTime = start;  }
        public void SetEndTime(DateTime end) { this.endTime = end; }
        public void SetStatus(ClosureStatus status) { this.status = status; }
    } 
}
