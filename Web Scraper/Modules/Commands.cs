using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Web_Scraper.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("next_launch")]
        public async Task next_launch()
        {
            LaunchScraper scraper = new LaunchScraper();
            await ReplyAsync(scraper.PrintSchedule());
        }
    }
}
