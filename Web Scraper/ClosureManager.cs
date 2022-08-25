using System;
using System.Collections.Generic;
using System.Net.Http;
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

                            int month = Utilities.GetMonthINT(splitDateComponents[1].Split(' ')[1]);
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
            if (closureManager.Count == 0) return null;

            string description = "";
            bool activeClosure = false;

            foreach (ClosureData closure in closureManager) 
            {
                if ((closure.GetEndTime().Day >= DateTime.Now.Day) || (closure.GetEndTime().Month > DateTime.Now.Month))
                {
                    ClosureStatus status = closure.GetStatus();

                    if ((status == ClosureStatus.ACTIVE) || ((DateTime.Now.Day == closure.GetStartTime().Day) && (DateTime.Now.Month == closure.GetStartTime().Month)))
                    {
                        description += "**There is currently an active closure - a static fire or launch might be imminent!** \n \n";
                        activeClosure = true;
                    } else if ((status == ClosureStatus.PRIMARY_DATE) || (status == ClosureStatus.ALTERNATE_DATE))
                    {
                        if (status == ClosureStatus.PRIMARY_DATE)
                        {
                            description += "Next **primary closure** is scheduled for ";
                        }
                        else
                        {
                            description += "Next **alternative closure** is scheduled for ";
                        }

                        string monthString = Utilities.GetMonthString(closure.GetStartTime().Month);

                        description += monthString + ", " + closure.GetStartTime().Day + ", " + closure.GetStartTime().Year + "\n";
                    }
                }
            }

            Color messageColor = Color.Green;

            if (activeClosure)
            {
                messageColor = Color.Red;
            }

            var embedBuilder = new EmbedBuilder()
                    .WithTitle("Upcoming StarBase Road Closures")
                    .WithDescription(description)
                    .WithColor(messageColor)
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
