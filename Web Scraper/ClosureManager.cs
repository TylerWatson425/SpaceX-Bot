using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Web_Scraper
{
    class ClosureManager
    {
        private DateTime[] startDates;
        private DateTime[] endDates;
        public ClosureManager()
        {
            startDates = null;
            endDates = null;

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
            //*[@id="vc_row-62fa7529d42fc"]/div/div/div/div[1]/table/tbody/tr[1]

            Console.WriteLine("ROWCOUNT: " + tableRows.Count());

            int counter = 1;
            ClosureData localStore;
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

                        if ((counter % 4) == 0)
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

                        }
                        if ((counter % 4) == 1)
                        {
                            //Save date
                        }
                        if ((counter % 4) == 2)
                        {
                            //save date
                        }
                        if ((counter % 4) == 3)
                        {
                            //beach status
                        }
                        counter++;
                    } 
                }
            }
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
